using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using WebApplication1.Models;

namespace WebApplication1.Services
{
    public static class TestUsers
    {
        public static List<User> Users { get; } = new List<User>
        {
            new User { Username = "KINGICT", Password = HashPassword("KINGICT") },
            new User { Username = "KINGICT", Password = HashPassword("KINGICT") }
            // Add more test users as needed
        };

        private static string HashPassword(string password)
        {
            // Use ASP.NET Core's built-in PasswordHasher for secure hashing
            var passwordHasher = new PasswordHasher<string>();
            return passwordHasher.HashPassword(null, password);
        }

        public static User FindUser(string username)
        {
            return Users.Find(u => u.Username == username);
        }

        public static bool VerifyPassword(User user, string password)
        {
            // Use ASP.NET Core's built-in PasswordHasher to verify password
            var passwordHasher = new PasswordHasher<string>();
            var result = passwordHasher.VerifyHashedPassword(null, user.Password, password);
            return result == PasswordVerificationResult.Success;
        }
    }
}
