// <copyright file="Crypto.cs" company="moneycorp">
// Copyright @2025 moneycorp. All rights reserved.
// </copyright>

namespace McGlobalAzureFunctions.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Security.Cryptography;
    using System.Text;
    using System.Threading.Tasks;

    public class Crypto
    {
        private static readonly byte[] Salt = Encoding.ASCII.GetBytes("niK\\=29LPz`A;_%wtC4l'vw-x.F~sA[D(vRF:|dQd-8tz$)AQL[W,b2mY9XG.(P");

        private static readonly string SharedSecret = "o@+`_|>:-/F#+yJL6hcX2w-bkSCoF>Tglw1|Md>M}+\\XB/A.x[~O&ZGn:h*u<a5";

        public static string GetEncryptedText(string textToEncrypt)
        {
            string s = Encrypt(textToEncrypt);
            UTF8Encoding uTF8Encoding = new UTF8Encoding();
            byte[] bytes = uTF8Encoding.GetBytes(s);
            return new ZBase32Encoder().Encode(bytes);
        }

        public static string Encrypt(string plainText)
        {
            if (string.IsNullOrEmpty(plainText))
            {
                throw new ArgumentNullException("plainText");
            }

            RijndaelManaged rijndaelManaged = null;
            try
            {
                Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(SharedSecret, Salt);
                rijndaelManaged = new RijndaelManaged();
                rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
                rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
                ICryptoTransform transform = rijndaelManaged.CreateEncryptor(rijndaelManaged.Key, rijndaelManaged.IV);
                using MemoryStream memoryStream = new MemoryStream();
                using (CryptoStream stream = new CryptoStream(memoryStream, transform, CryptoStreamMode.Write))
                {
                    using StreamWriter streamWriter = new StreamWriter(stream);
                    streamWriter.Write(plainText);
                }

                return Convert.ToBase64String(memoryStream.ToArray());
            }
            finally
            {
                rijndaelManaged?.Clear();
            }
        }

        public static string Decrypt(string cipherText)
        {
            if (string.IsNullOrEmpty(cipherText))
            {
                throw new ArgumentNullException("cipherText");
            }

            string result = string.Empty;
            Rfc2898DeriveBytes rfc2898DeriveBytes = new Rfc2898DeriveBytes(SharedSecret, Salt);
            RijndaelManaged rijndaelManaged = new RijndaelManaged();
            rijndaelManaged.Key = rfc2898DeriveBytes.GetBytes(rijndaelManaged.KeySize / 8);
            rijndaelManaged.IV = rfc2898DeriveBytes.GetBytes(rijndaelManaged.BlockSize / 8);
            ICryptoTransform transform = rijndaelManaged.CreateDecryptor(rijndaelManaged.Key, rijndaelManaged.IV);
            byte[] buffer = Convert.FromBase64String(cipherText);
            using (MemoryStream stream = new MemoryStream(buffer))
            {
                using CryptoStream stream2 = new CryptoStream(stream, transform, CryptoStreamMode.Read);
                using StreamReader streamReader = new StreamReader(stream2);
                result = streamReader.ReadToEnd();
            }

            return result;
        }
    }
}
