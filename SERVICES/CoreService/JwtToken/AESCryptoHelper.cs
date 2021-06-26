﻿using System;
using System.Security.Cryptography;
using System.Text;

namespace CoreService.JwtToken
{
    /// <summary>
    ///     AES加密解密
    /// </summary>
    public class AESCryptoHelper
    {
        /// <summary>
        ///     AES加密解密Key，Key必须十六位
        /// </summary>
        private static readonly string AESKey = "1111111111111111";

        /// <summary>
        ///     AES 加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <returns></returns>
        public static string Encrypt(string plainText)
        {
            return Encrypt(plainText, AESKey);
        }

        /// <summary>
        ///     AES 加密
        /// </summary>
        /// <param name="plainText"></param>
        /// <param name="key">密码必须是16位，否则会报错哈</param>
        /// <returns></returns>
        public static string Encrypt(string plainText, string key)
        {
            string result = null;
            if (string.IsNullOrEmpty(plainText)) return result;
            var plainTextArray = Encoding.UTF8.GetBytes(plainText);
            using (var provider = new MD5CryptoServiceProvider())
            {
                using (var rijndaelManaged = new RijndaelManaged
                {
                    Key = provider.ComputeHash(Encoding.UTF8.GetBytes(key)),
                    //Key = Encoding.UTF8.GetBytes(key),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                })

                using (var cryptoTransform = rijndaelManaged.CreateEncryptor())
                {
                    var resultArray = cryptoTransform.TransformFinalBlock(plainTextArray, 0, plainTextArray.Length);
                    result = Convert.ToBase64String(resultArray, 0, resultArray.Length);
                    Array.Clear(resultArray, 0, resultArray.Length);
                    resultArray = null;
                }
            }

            Array.Clear(plainTextArray, 0, plainTextArray.Length);
            plainTextArray = null;
            return result;
        }

        /// <summary>
        ///     AES 解密
        /// </summary>
        /// <param name="encryptText"></param>
        /// <returns></returns>
        public static string Decrypt(string encryptText)
        {
            return Decrypt(encryptText, AESKey);
        }

        /// <summary>
        ///     AES 解密
        /// </summary>
        /// <param name="encryptText"></param>
        /// <param name="key">密码必须是16位，否则会报错哈</param>
        /// <returns></returns>
        public static string Decrypt(string encryptText, string key)
        {
            string result = null;
            if (string.IsNullOrEmpty(encryptText)) return result;
            var encryptTextArray = Convert.FromBase64String(encryptText);
            using (var provider = new MD5CryptoServiceProvider())
            {
                using (var rijndaelManaged = new RijndaelManaged
                {
                    Key = provider.ComputeHash(Encoding.UTF8.GetBytes(key)),
                    //Key = Encoding.UTF8.GetBytes(key),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                })
                {
                    using (var cryptoTransform = rijndaelManaged.CreateDecryptor())
                    {
                        var resultArray =
                            cryptoTransform.TransformFinalBlock(encryptTextArray, 0, encryptTextArray.Length);
                        result = Encoding.UTF8.GetString(resultArray);
                        Array.Clear(resultArray, 0, resultArray.Length);
                        resultArray = null;
                    }
                }
            }

            Array.Clear(encryptTextArray, 0, encryptTextArray.Length);
            encryptTextArray = null;
            return result;
        }
    }
}