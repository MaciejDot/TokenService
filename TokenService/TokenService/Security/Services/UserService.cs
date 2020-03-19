
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TokenService.Configuration;
using TokenService.Data;
using TokenService.Data.Entities;
using TokenService.Security.Models;

namespace TokenService.Security.Services
{
    public class UserService : IUserService
    {
        private readonly Random _random;
        private readonly TokenServiceContext _context;
        private readonly RSA _rsaKey;

        public UserService(Random random,
            TokenServiceContext context,
            IOptions<PrivateRSAOptions> optionsRSA)
        {
            _context = context;
            _random = random;
            var options = optionsRSA.Value;
            _rsaKey = RSA.Create(new RSAParameters
            {
                Modulus = Convert.FromBase64String(options.Modulus),
                Exponent = Convert.FromBase64String(options.Exponent),
                DP = Convert.FromBase64String(options.DP),
                DQ = Convert.FromBase64String(options.DQ),
                D = Convert.FromBase64String(options.D),
                Q = Convert.FromBase64String(options.Q),
                InverseQ = Convert.FromBase64String(options.InverseQ),
                P = Convert.FromBase64String(options.P)
            });
        }

        public RSAParameters GetRSAPublicKey()
        {
            return _rsaKey.ExportParameters(false);
        }

        public string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            string characters = "qwertyuiopasdfghjklzxcvbn1234567890QWERTYUIOPASDFGHJKLZXCVBNM";
            for (int i = 0; i < size; i++)
            {
                builder.Append(characters[_random.Next(characters.Length)]);
            }
            return builder.ToString();
        }

        public async Task<bool> AddUser(RegisterUser user, CancellationToken cancellationToken)
        {
            if (await _context.Users.AnyAsync(x => x.Email == user.Email || x.Username == user.Username, cancellationToken))
            {
                return false;
            }
            var stamp = RandomString(100);
            var passwordHash = await GetHash(stamp, user.Password);
            var id = RandomString(100);
            await _context.Users.AddAsync(new User { Id = id, Email = user.Email, Username = user.Username, SecurityStamp = stamp, PasswordHash = passwordHash }, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<string> GetTokenForUser(string id, CancellationToken cancellationToken)
        {
            var userData = await _context.Users.Select(x => new { User = x, Roles = x.UserRoles.Select(x => x.Role.Name).ToList() }).SingleAsync(x => x.User.Id == id, cancellationToken);
            var user = userData.User;
            var roles = userData.Roles;
            var tokenHandler = new JwtSecurityTokenHandler();

            var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim("Name", user.Username));
            claims.Add(new Claim("Id", user.Id));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(_rsaKey), SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<string> Authenticate(AuthenticationModel authenticationModel, CancellationToken cancellationToken)
        {
            var userData = await _context.Users
                .Select(x => new { User = x, Roles = x.UserRoles.Select(x => x.Role.Name).ToList() })
                .SingleAsync(x => x.User.Email == authenticationModel.Email, cancellationToken);
            var user = userData.User;
            if (await GetHash(user.SecurityStamp, authenticationModel.Password) != user.PasswordHash)
            {
                throw new Exception("not valid model");
            }

            var roles = userData.Roles;
            var tokenHandler = new JwtSecurityTokenHandler();
            var claims = roles.Select(role => new Claim(ClaimTypes.Role, role)).ToList();
            claims.Add(new Claim(ClaimTypes.Email, user.Email));
            claims.Add(new Claim("Id", user.Id));
            claims.Add(new Claim("Name", user.Username));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddMinutes(5),
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(_rsaKey), SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public Task<List<UserDTO>> GetUsers(CancellationToken token)
        {
            return _context.Users
                .Select(x => new UserDTO
                {
                    Id = x.Id,
                    Username = x.Username
                }).ToListAsync(token);
        }
        private static Task<string> GetHash(string stamp, string password)
        {
            var sha256 = SHA256.Create();
            var stampBytes = Encoding.UTF8.GetBytes(stamp);
            var passwordBytes = Encoding.UTF8.GetBytes(password);
            var finalHash = passwordBytes;
            for (int i = 0; i < 10000; i++)
            {
                finalHash = sha256.ComputeHash(finalHash.Union(stampBytes).ToArray());
            }
            return Task.FromResult(Convert.ToBase64String(finalHash));
        }

    }
}
