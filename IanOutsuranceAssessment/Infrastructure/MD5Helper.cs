using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure
{
    public static class MD5Helper
    {
        public static string GetMD5TextHashCode(string fileContentAsString)
        {
            //Read the plain text into byte[]
            byte[] fileContentAsBytes = new byte[fileContentAsString.Length];
            fileContentAsBytes = Encoding.ASCII.GetBytes(fileContentAsString);

            MD5CryptoServiceProvider myMD5 = new MD5CryptoServiceProvider();

            byte[] myHash = myMD5.ComputeHash(fileContentAsBytes);
            System.Text.StringBuilder hashCode = new System.Text.StringBuilder();

            foreach (byte b in myHash)
            {
                hashCode.Append(b.ToString("x2").ToLower());
            }

            return hashCode.ToString();
        }
    }
}
