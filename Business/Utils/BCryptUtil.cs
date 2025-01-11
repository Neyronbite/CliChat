using BCrypt.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Utils
{
    internal static class BCryptUtil
    {
        internal static string Hash(this string plainText)
        {
            return BCrypt.Net.BCrypt.HashPassword(plainText);
        }
        internal static bool CheckHash(this string hash, string plainText)
        {
            return BCrypt.Net.BCrypt.Verify(plainText, hash);
        }
    }
}
