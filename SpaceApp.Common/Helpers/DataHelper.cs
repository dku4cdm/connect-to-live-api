using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;

namespace SpaceApp.Common.Helpers
{
    public static class DataHelper
    {
        public static string EncodePass(this string rawData)
        {
            using (SHA256 sha256Hash = SHA256.Create())
            {
                byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

                StringBuilder builder = new StringBuilder();
                for (int index = 0; index < bytes.Length; index++)
                {
                    builder.Append(bytes[index].ToString("x2"));
                }
                return builder.ToString();
            }
        }
    }
}
