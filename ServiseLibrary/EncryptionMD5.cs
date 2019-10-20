using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ServiceLibrary
{
    public static class EncryptionMD5
    {
        // Logic of user's password encryption.
        public static string Encript(string input)
        {
            var strb = new StringBuilder();
            using (var md5 = MD5.Create())
            {
                byte[] data = md5.ComputeHash(Encoding.UTF8.GetBytes(input));
                foreach (var elem in data)
                {
                    strb.Append(elem.ToString("x2"));
                }
            }

            return strb.ToString();
        }
    }
}
