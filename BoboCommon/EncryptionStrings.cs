using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;


namespace BoboCommon
{
    public class EncryptionStrings
    {
        /// <summary>
        /// 解密
        /// </summary>
        /// <param name="action"></param>
        /// <returns>请求的串</returns>
        public static string DecryptParameter(string action)
        {

            var str = string.Empty;
            try
            {
                str = DecryptString2(action.Substring(8, action.Length - 32), action.Substring(0, 8) + action.Substring(action.Length - 24) + "00200");
                str = HttpUtility.UrlDecode(str);
            }
            catch { }

            return str;
        }


        /// <summary>
        /// 结果加密
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        public static string EncryptResult(string str)
        {
            string guid = Guid.NewGuid().ToString().Replace("-", "");
            str = HttpUtility.UrlEncode(str, Encoding.UTF8);
            return guid.Substring(0, 8) + EncryptString2(str, guid + "00200") + guid.Substring(8);
        }



        /// <summary>
        ///  Encrpyts the sourceString, returns this result as an Aes encrpyted, BASE64 encoded string  加密
        /// </summary>
        /// <param name="plainSourceStringToEncrypt">a plain, Framework string (ASCII, null terminated)</param>
        /// <param name="passPhrase">The pass phrase.</param>
        /// <returns>returns an Aes encrypted, BASE64 encoded string</returns>
        private static string EncryptString(string plainSourceStringToEncrypt, string passPhrase)
        {
            try
            {
                //Set up the encryption objects
                using (AesCryptoServiceProvider acsp = GetProvider(Encoding.Default.GetBytes(passPhrase)))
                {
                    byte[] sourceBytes = Encoding.ASCII.GetBytes(plainSourceStringToEncrypt);
                    ICryptoTransform ictE = acsp.CreateEncryptor();

                    //Set up stream to contain the encryption
                    MemoryStream msS = new MemoryStream();

                    //Perform the encrpytion, storing output into the stream
                    CryptoStream csS = new CryptoStream(msS, ictE, CryptoStreamMode.Write);
                    csS.Write(sourceBytes, 0, sourceBytes.Length);
                    csS.FlushFinalBlock();

                    //sourceBytes are now encrypted as an array of secure bytes
                    byte[] encryptedBytes = msS.ToArray(); //.ToArray() is important, don't mess with the buffer

                    //return the encrypted bytes as a BASE64 encoded string
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
            catch (Exception)
            {
                return "";
            }

        }


        /// <summary>
        ///  Encrpyts the sourceString, returns this result as an Aes encrpyted, BASE64 encoded string  加密
        /// </summary>
        /// <param name="plainSourceStringToEncrypt">a plain, Framework string (ASCII, null terminated)</param>
        /// <param name="passPhrase">The pass phrase.</param>
        /// <returns>returns an Aes encrypted, BASE64 encoded string</returns>
        private static string EncryptString2(string plainSourceStringToEncrypt, string passPhrase)
        {
            try
            {
                //Set up the encryption objects
                using (AesCryptoServiceProvider acsp = GetProvider(Encoding.Default.GetBytes(passPhrase)))
                {
                    acsp.Padding = PaddingMode.PKCS7;
                    acsp.Mode = CipherMode.ECB;
                    byte[] sourceBytes = Encoding.ASCII.GetBytes(plainSourceStringToEncrypt);
                    ICryptoTransform ictE = acsp.CreateEncryptor();

                    //Set up stream to contain the encryption
                    MemoryStream msS = new MemoryStream();

                    //Perform the encrpytion, storing output into the stream
                    CryptoStream csS = new CryptoStream(msS, ictE, CryptoStreamMode.Write);
                    csS.Write(sourceBytes, 0, sourceBytes.Length);
                    csS.FlushFinalBlock();

                    //sourceBytes are now encrypted as an array of secure bytes
                    byte[] encryptedBytes = msS.ToArray(); //.ToArray() is important, don't mess with the buffer

                    //return the encrypted bytes as a BASE64 encoded string
                    return Convert.ToBase64String(encryptedBytes);
                }
            }
            catch (Exception)
            {
                return "";
            }

        }


        /// <summary>
        /// Decrypts a BASE64 encoded string of encrypted data, returns a plain string 解密
        /// </summary>
        /// <param name="base64StringToDecrypt">an Aes encrypted AND base64 encoded string</param>
        /// <param name="passphrase">The passphrase.</param>
        /// <returns>returns a plain string</returns>
        private static string DecryptString(string base64StringToDecrypt, string passphrase)
        {
            try
            {
                //Set up the encryption objects
                using (AesCryptoServiceProvider acsp = GetProvider(Encoding.Default.GetBytes(passphrase)))
                {
                    byte[] RawBytes = Convert.FromBase64String(base64StringToDecrypt);
                    ICryptoTransform ictD = acsp.CreateDecryptor();

                    //RawBytes now contains original byte array, still in Encrypted state

                    //Decrypt into stream
                    MemoryStream msD = new MemoryStream(RawBytes, 0, RawBytes.Length);
                    CryptoStream csD = new CryptoStream(msD, ictD, CryptoStreamMode.Read);
                    //csD now contains original byte array, fully decrypted

                    //return the content of msD as a regular string
                    return (new StreamReader(csD)).ReadToEnd();
                }
            }
            catch (System.Exception ex)
            {
                return "";
            }

        }

        /// <summary>
        /// Decrypts a BASE64 encoded string of encrypted data, returns a plain string 解密
        /// </summary>
        /// <param name="base64StringToDecrypt">an Aes encrypted AND base64 encoded string</param>
        /// <param name="passphrase">The passphrase.</param>
        /// <returns>returns a plain string</returns>
        private static string DecryptString2(string base64StringToDecrypt, string passphrase)
        {
            try
            {
                //Set up the encryption objects
                using (AesCryptoServiceProvider acsp = GetProvider(Encoding.Default.GetBytes(passphrase)))
                {
                    acsp.Mode = CipherMode.ECB;
                    acsp.Padding = PaddingMode.PKCS7;
                    byte[] RawBytes = Convert.FromBase64String(base64StringToDecrypt);
                    ICryptoTransform ictD = acsp.CreateDecryptor();

                    //RawBytes now contains original byte array, still in Encrypted state

                    //Decrypt into stream
                    MemoryStream msD = new MemoryStream(RawBytes, 0, RawBytes.Length);
                    //csD now contains original byte array, fully decrypted
                    CryptoStream csD = new CryptoStream(msD, ictD, CryptoStreamMode.Read);
                    //return the content of msD as a regular string
                    return (new StreamReader(csD)).ReadToEnd();
                }
            }
            catch (Exception)
            {
                return "";
            }

        }

        private static AesCryptoServiceProvider GetProvider(byte[] key)
        {
            AesCryptoServiceProvider result = new AesCryptoServiceProvider();
            result.BlockSize = 128;
            result.KeySize = 128;
            result.Mode = CipherMode.CBC;
            result.Padding = PaddingMode.PKCS7;

            result.GenerateIV();
            result.IV = new byte[] { 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0 };

            byte[] RealKey = GetKey(key, result);
            result.Key = RealKey;
            // result.IV = RealKey;
            return result;
        }

        //private static byte[] GetKey(byte[] suggestedKey, SymmetricAlgorithm p)
        //{
        //    byte[] kRaw = suggestedKey;
        //    List<byte> kList = new List<byte>();

        //    for (int i = 0; i < p.LegalKeySizes[0].MinSize; i += 8)
        //    {
        //        kList.Add(kRaw[(i / 8) % kRaw.Length]);
        //    }
        //    byte[] k = kList.ToArray();
        //    return k;
        //}

        private static byte[] GetKey(byte[] suggestedKey, SymmetricAlgorithm p)
        {
            var kRaw = suggestedKey;
            var kList = new List<byte>();

            for (var i = kRaw.Length - 1; i > -1; i--)
            {
                kList.Add(kRaw[i]);
                if (kList.Count == 16)
                    break;
            }

            var k = kList.ToArray();
            return k;
        }
    }
}
