using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Utils
{
    public static class StringUtil
    {
        public static string GenerateString(int length = 8)
        {
            const string supportedSymbols = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%&*()abcdefghijklmnopqrstuvwxyz";
            var sb = new StringBuilder();
            var rand = new Random();

            for (int i = 0; i < length; i++)
            {
                sb.Append(supportedSymbols[rand.Next(0, supportedSymbols.Length)]);
            }
            return sb.ToString();
        }
    }
}
