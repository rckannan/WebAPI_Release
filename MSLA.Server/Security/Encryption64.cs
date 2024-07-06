using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Security.Cryptography;
using System.IO;

namespace MSLA.Server.Security
{
    /// <summary>The Encryption Class</summary>
    [System.Diagnostics.DebuggerStepThrough()]
    public class Encryption64
    {
        //   ****    Use TripleDES CryptoService with Private key pair
        // *****    This key should be the same as that in MSLA.Security.Encryption64
        private static Byte[] _key = System.Text.Encoding.ASCII.GetBytes("dtsQ~JwCLhll;N$GmLMre'bu");

        /// <summary>Decryptor</summary>
        /// <param name="stringToDecrypt">The Encrypted String</param>
        /// <param name="IV">The Initialization Vector</param>
        public static string DecryptFromBase64String(String stringToDecrypt)
        {
            if (stringToDecrypt == String.Empty)
            {
                return String.Empty;
            }

            if (!stringToDecrypt.Contains(":true"))
            {
                return stringToDecrypt;
            }
            else
            {
                stringToDecrypt = stringToDecrypt.Replace(":true", "");
            }

            // the last 8 characters contain the IV
            Byte[] IV = System.Text.Encoding.ASCII.GetBytes(stringToDecrypt.Substring(stringToDecrypt.Length - 8, 8));
            stringToDecrypt = stringToDecrypt.Substring(0, stringToDecrypt.Length - 8);

            //   *****   Create a Byte array for the String to decrypt
            Byte[] inputByteArray = new Byte[stringToDecrypt.Length];

            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            //   ****  we have a base 64 encoded string so first must decode to regular unencoded (encrypted) string
            inputByteArray = Convert.FromBase64String(stringToDecrypt);
            //   ****  now decrypt the regular string
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(_key, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();
            //   ****    Fetch original string from the Decrypted Memory Stream
            System.Text.Encoding encoding = System.Text.Encoding.UTF8;
            return encoding.GetString(ms.ToArray());
        }

        /// <summary>Encryptor</summary>
        /// <param name="stringToEncrypt">The Decrypted String</param>
        /// <param name="IV">The Initialization Vector of 8 character length</param>
        public static String EncryptToBase64String(String stringToEncrypt, string iv)
        {
            Byte[] IV = System.Text.Encoding.ASCII.GetBytes(iv);
            TripleDESCryptoServiceProvider des = new TripleDESCryptoServiceProvider();
            //   ****    convert our input string to a byte array
            Byte[] inputByteArray = System.Text.Encoding.UTF8.GetBytes(stringToEncrypt);
            //   ****    now encrypt the bytearray
            MemoryStream ms = new MemoryStream();
            CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(_key, IV), CryptoStreamMode.Write);
            cs.Write(inputByteArray, 0, inputByteArray.Length);
            cs.FlushFinalBlock();

            //   ****    now return the byte array as a "safe for XMLDOM" Base64 String
            return Convert.ToBase64String(ms.ToArray()) + System.Text.Encoding.ASCII.GetString(IV) + ":true";
        }        
    }
}
