
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Threading;
using System.Threading.Tasks;
using TokenService.Security.Models;

namespace TokenService.Security.Services
{
    public interface IUserService
    {
        RSAParameters GetRSAPublicKey();
        Task<string> Authenticate(AuthenticationModel authenticationModel, CancellationToken token);
        Task<string> GetTokenForUser(string id, CancellationToken token);
        Task<bool> AddUser(RegisterUser user, CancellationToken token);
    }
}
