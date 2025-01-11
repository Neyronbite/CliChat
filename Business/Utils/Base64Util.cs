using System.Text;

namespace Business.Utils
{
    public static class Base64Util
    {
        public  static string Encode(this string s)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(s);
            return Convert.ToBase64String(plainTextBytes);
        }
        public static string Decode(this string s)
        {
            var base64EncodedBytes = Convert.FromBase64String(s);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
