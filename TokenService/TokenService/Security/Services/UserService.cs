
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
using TokenService.Data;
using TokenService.Data.Entities;
using TokenService.Security.Models;

namespace TokenService.Security.Services
{
    public class UserService : IUserService
    {
        private readonly Random _random;
        private readonly TokenServiceContext _context;
        public static RSA RSAKey { get; set; }

        public UserService( Random random,
            TokenServiceContext context)
        {
            _context = context;
            _random = random;
        }

        public RSAParameters GetRSAPublicKey()
        {
            return RSAKey.ExportParameters(false);
        }

        public string RandomString(int size)
        {
            StringBuilder builder = new StringBuilder();
            char ch;
            for (int i = 0; i < size; i++)
            {
                ch = Convert.ToChar(26 * _random.Next(26) + 65);
                builder.Append(ch);
            }
            return builder.ToString();
        }

        public async Task<bool> AddUser(RegisterUser user, CancellationToken cancellationToken)
        {
            if (await _context.Users.AnyAsync(x=>x.Email == user.Email|| x.Username == user.Username , cancellationToken))
            {
                return false;
            }
            var stamp = RandomString(100);
            var passwordHash = GetHash(stamp, user.Password);
            var id = RandomString(100);
            await _context.Users.AddAsync(new User { Id = id, Email = user.Email, Username = user.Username, SecurityStamp = stamp, PasswordHash = passwordHash }, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
            return true;
        }

        public async Task<string> GetTokenForUser(string id, CancellationToken cancellationToken)
        {
            var userData = await _context.Users.Select(x=> new {User = x, Roles = x.UserRoles.Select(x => x.Role.Name).ToList() }).SingleAsync(x => x.User.Id == id, cancellationToken);
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
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(RSAKey), SecurityAlgorithms.RsaSsaPssSha256)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }

        public async Task<string> Authenticate(AuthenticationModel authenticationModel, CancellationToken cancellationToken)
        {
            var userData = await _context.Users
                .Select(x => new { User = x , Roles = x.UserRoles.Select(x => x.Role.Name).ToList()})
                .SingleAsync( x=>x.User.Email == authenticationModel.Email, cancellationToken);
            var user = userData.User;
            if (GetHash(user.SecurityStamp, authenticationModel.Password) != user.PasswordHash)
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
                SigningCredentials = new SigningCredentials(new RsaSecurityKey(RSAKey), SecurityAlgorithms.RsaSha256Signature, SecurityAlgorithms.Sha256Digest)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        private string GetHash(string stamp, string password)
        {
            var sha256 = SHA256.Create();
            var stampBytes = Encoding.ASCII.GetBytes(stamp);
            var passwordBytes = Encoding.ASCII.GetBytes(password);
            var finalHash = passwordBytes;
            for(int i = 0; i < 10000; i++)
            {
                finalHash = sha256.ComputeHash(finalHash.Union(stampBytes).ToArray());
            }
            return Convert.ToBase64String(finalHash);
        }

    }
}
