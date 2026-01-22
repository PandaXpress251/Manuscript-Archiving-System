using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Thesis_Capstone_Archive.Models;

namespace Thesis_Capstone_Archive.Helpers
{
    public class PasswordService
    {
        public string HashPassword(string password)
        {
            var hasher = new PasswordHasher<User>();
            return hasher.HashPassword(null, password);  // null because you don't need a user object here
        }

        public bool VerifyPassword(string hashedPassword, string plainPassword)
        {
            var hasher = new PasswordHasher<User>();
            var result = hasher.VerifyHashedPassword(null, hashedPassword, plainPassword);
            return result == PasswordVerificationResult.Success;
        }
    }
}
