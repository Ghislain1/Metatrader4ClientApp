

namespace Metatrader4ClientApp.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;
    public class HashManager
    {
        public static string HashPassword(string password)
        {
            var salt = RandomNumberGenerator.GetBytes(16);
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000);
            var hash = pbkdf2.GetBytes(20);
            var hashBytes = new byte[36];
            Array.Copy(salt, 0, hashBytes, 0, salt.Length);
            Array.Copy(hash, 0, hashBytes, salt.Length, hash.Length);

            var savedPasswordHash = Convert.ToBase64String(hashBytes);
            return savedPasswordHash;
        }

        public static bool VerifyPassword(string enteredPassword, string savedPasswordHash)
        {
            var hashBytes = Convert.FromBase64String(savedPasswordHash);
            var salt = new byte[16];
            Array.Copy(hashBytes, 0, salt, 0, salt.Length);

            var pbkdf2 = new Rfc2898DeriveBytes(enteredPassword, salt, 10000);
            var hash = pbkdf2.GetBytes(20);

            for (var i = 0; i < hash.Length; i++)
            {
                if (hash[i] != hashBytes[salt.Length + i])
                {
                    return false;
                }
            }

            return true;
        }
    }
}
