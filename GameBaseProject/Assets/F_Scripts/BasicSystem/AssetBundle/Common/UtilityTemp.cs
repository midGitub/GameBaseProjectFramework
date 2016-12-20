using System;
using System.Collections;
using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using UnityEngine;

namespace Best
{
    public class UtilityTemp
    {
        public static string GetFileHash(string path)
        {
            string _hexStr = null;

            HashAlgorithm ha = HashAlgorithm.Create();
            FileStream fs = new FileStream(path,
                FileMode.Open,
                FileAccess.Read,
                FileShare.Read);
            byte[] bytes = ha.ComputeHash(fs);
            _hexStr = ToHexString(bytes);
            fs.Close();

            return _hexStr;
        }

        private static string ToHexString(byte[] bytes)
        {
            string hexString = string.Empty;
            if (bytes != null)
            {
                StringBuilder strB = new StringBuilder();

                for (int i = 0; i < bytes.Length; i++)
                {
                    strB.Append(bytes[i].ToString("X2"));
                }
                hexString = strB.ToString();
            }
            return hexString;
        }
    }
}