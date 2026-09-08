using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.Application.Security
{
    // Managing Passwords
    public class PasswordHasher
    {
        private readonly IPasswordHasher<object> _hasher;


        public PasswordHasher()
        {
            _hasher = new Microsoft.AspNetCore.Identity.PasswordHasher<object>();
        }



        public string Hash(string password)
        {
            return _hasher.HashPassword(null!, password);
        }



        public bool Verify(string hash, string password)
        {
            var result =
                _hasher.VerifyHashedPassword(
                    null!,
                    hash,
                    password
                );

            return result == PasswordVerificationResult.Success;
        }
    }
}
