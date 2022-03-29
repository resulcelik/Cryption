using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace ResCryption
{
    public class EncryptionManager
    {
        private static byte[] saltBytes = new byte[] { 1, 2, 3, 4, 5, 6, 7, 8 };

        public delegate byte[] encrytDelegate(byte[] data, string key, string cipherMode);
        public delegate byte[] decryptDelegate(byte[] data, string key, string cipherMode);

        private static Dictionary<string, Tuple<encrytDelegate, decryptDelegate>> EncryptionDictonary;
        private static Dictionary<string, CipherMode> CipherModeDictonary;

        public static void Init()
        {
            EncryptionDictonary = new Dictionary<string, Tuple<encrytDelegate, decryptDelegate>>();
            EncryptionDictonary.Add("Rijndael", new Tuple<encrytDelegate, decryptDelegate>(EncryptWithRijndael, DecryptWithRijndael));
            EncryptionDictonary.Add("Scrypt", new Tuple<encrytDelegate, decryptDelegate>(EncryptWithScrypt, DecryptWithScrypt));
            EncryptionDictonary.Add("DES", new Tuple<encrytDelegate, decryptDelegate>(EncryptWithDes, DecryptWithDes));

            CipherModeDictonary = new Dictionary<string, CipherMode>();
            var values = Helper.GetEnumValues<CipherMode>();
            foreach (var value in values)
                CipherModeDictonary.Add(value.ToString(), value);
        }

        public static string[] GetEncryptionKeys()
        {
            return EncryptionDictonary.Keys.ToArray();
        }

        public static string[] GetCipherModeKeys()
        {
            return CipherModeDictonary.Keys.ToArray();
        }

        public static encrytDelegate GetEncrypt(string key)
        {
            return EncryptionDictonary[key].Item1;
        }

        public static decryptDelegate GetDecrypt(string key)
        {
            return EncryptionDictonary[key].Item2;
        }

        public static byte[] EncryptWithScrypt(byte[] data, string key, string cipherMode)
        {
            Scrypt scrypt = new Scrypt();
            scrypt.mode = CipherModeDictonary[cipherMode];
            return scrypt.Encrypt(data, key);
        }
        public static byte[] DecryptWithScrypt(byte[] data, string key, string cipherMode)
        {
            Scrypt scrypt = new Scrypt();
            scrypt.mode = CipherModeDictonary[cipherMode];
            return scrypt.Decrypt(data, key);
        }

        public static byte[] EncryptWithRijndael(byte[] data, string key, string cipherMode)
        {
            return EncryptWithRijndael(data, Encoding.UTF8.GetBytes(key), CipherModeDictonary[cipherMode]);
        }

        public static byte[] DecryptWithRijndael(byte[] data, string key, string cipherMode)
        {
            return DecryptWithRijndael(data, Encoding.UTF8.GetBytes(key), CipherModeDictonary[cipherMode]);
        }

        public static byte[] EncryptWithDes(byte[] data, string key, string cipherMode)
        {
            return EncryptWithDes(data, Encoding.UTF8.GetBytes(key), CipherModeDictonary[cipherMode]);
        }

        public static byte[] DecryptWithDes(byte[] data, string key, string cipherMode)
        {
            return DecryptWithDes(data, Encoding.UTF8.GetBytes(key), CipherModeDictonary[cipherMode]);
        }

        private static RijndaelManaged GetAESManaged(byte[] passwordBytes, CipherMode cipherMode)
        {
            RijndaelManaged AES = new RijndaelManaged();

            AES.KeySize = 256;
            AES.BlockSize = 128;

            var key = new Rfc2898DeriveBytes(passwordBytes, EncryptionManager.saltBytes, 1000);
            AES.Key = key.GetBytes(AES.KeySize / 8);
            AES.IV = key.GetBytes(AES.BlockSize / 8);
            AES.Padding = PaddingMode.PKCS7;

            AES.Mode = cipherMode;

            return AES;
        }

        private static DESCryptoServiceProvider GetDESManaged(byte[] passwordBytes, CipherMode cipherMode)
        {
            DESCryptoServiceProvider DES = new DESCryptoServiceProvider();

            byte[] IV = { 10, 20, 30, 40, 50, 60, 70, 80 };
            DES.KeySize = 64;

            var key = new Rfc2898DeriveBytes(passwordBytes, EncryptionManager.saltBytes, 1000);
            DES.Key = key.GetBytes(DES.KeySize / 8);
            DES.IV = IV;
            DES.Padding = PaddingMode.PKCS7;

            DES.Mode = cipherMode;

            return DES;
        }
        
        private static byte[] EncryptWithRijndael(byte[] data, byte[] passwordBytes, CipherMode cipherMode)
        {
            byte[] encrypted = null;
            try
            {
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, GetAESManaged(passwordBytes, cipherMode).CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }
            catch (Exception ex) 
            {
                Console.WriteLine(ex.Message);
                throw new Exception("ERR_ENC");
            }

            return encrypted;
        }

        private static byte[] DecryptWithRijndael(byte[] data, byte[] passwordBytes, CipherMode cipherMode)
        {
            List<byte> decryptedData = new List<byte>();
            try
            {
                using (MemoryStream msDecrypt = new MemoryStream(data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, GetAESManaged(passwordBytes, cipherMode).CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        int val;
                        while ((val = csDecrypt.ReadByte()) != -1)
                            decryptedData.Add((byte)val);
                    }
                }
            }
            catch(Exception ex) 
            {
                Console.WriteLine(ex.Message);
                throw new Exception("ERR_WRONG_PASS");
            }

            return decryptedData.ToArray();
        }

        private static byte[] EncryptWithDes(byte[] data, byte[] passwordBytes, CipherMode cipherMode)
        {
            byte[] encrypted = null;

            try
            {
                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, GetDESManaged(passwordBytes, cipherMode).CreateEncryptor(), CryptoStreamMode.Write))
                    {
                        csEncrypt.Write(data, 0, data.Length);
                    }

                    encrypted = msEncrypt.ToArray();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("ERR_ENC");
            }

            return encrypted;
        }

        private static byte[] DecryptWithDes(byte[] data, byte[] passwordBytes, CipherMode cipherMode)
        {
            List<byte> decryptedData = new List<byte>();
            try
            {
                using (MemoryStream msDecrypt = new MemoryStream(data))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, GetDESManaged(passwordBytes, cipherMode).CreateDecryptor(), CryptoStreamMode.Read))
                    {
                        int val;
                        while ((val = csDecrypt.ReadByte()) != -1)
                            decryptedData.Add((byte)val);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                throw new Exception("ERR_WRONG_PASS");
            }

            return decryptedData.ToArray();
        }
    }
}
