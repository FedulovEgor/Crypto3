using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crypto3
{
    class Program
    {
        static byte[] rsaPublicKey;
        static byte[] rsaPriveteKey;

        static void Main()
        {
            Menu();
        }

        private static void Menu()
        {
            Console.WriteLine("Выберите действие:\n" +
                "1 - Генерация ключей RSA\n" +
                "2 - Шифрование и расшифрование документа симметричным криптоалгоритмом\n" +
                "3 - Шифрование и расшифрование сеансового ключа симметричного алгоритма при помощи ключей RSA\n" +
                "4 - Формирование и проверку цифровой подписи документа\n" +
                "0 - Выход\n");
            while (true)
            {
                Console.Write("--> ");
                switch (Console.ReadLine())
                {
                    case "1":
                        GenerateRSAKey();
                        break;
                    case "2":
                        SimmetricCryptography();
                        break;
                    case "3":
                        SimmetricSeanseKeyRSA();
                        break;
                    case "4":
                        DigitalSignature();
                        break;
                }
            }
        }

        private static void DigitalSignature()
        {
            throw new NotImplementedException();
        }

        private static void SimmetricSeanseKeyRSA()
        {
            throw new NotImplementedException();
        }

        private static void SimmetricCryptography()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
        }

        /// <summary>
        /// Генерация ключевой пары RSA
        /// </summary>
        private static void GenerateRSAKey()
        {
            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                rsaPublicKey = rsa.ExportCspBlob(false);
                rsaPriveteKey = rsa.ExportCspBlob(true);
            }

            StringBuilder keysToString = new StringBuilder();
            keysToString.AppendLine("Открытый ключ: ");

            foreach (var el in rsaPublicKey)
            {
                keysToString.Append(el);
            }

            keysToString.AppendLine("\nЗакрытый ключ: ");

            foreach (var el in rsaPriveteKey)
            {
                keysToString.Append(el);
            }

            Console.WriteLine("Ключи сгенерированы");
            Console.WriteLine(keysToString.ToString());
        }

        public static byte[] EncryptionRSA(byte[] Data, RSAParameters RSAKey, bool DoOAEPPadding)
        {
            try
            {
                byte[] encryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKey);
                    encryptedData = RSA.Encrypt(Data, DoOAEPPadding);
                }
                return encryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        public static byte[] DecryptionRSA(byte[] Data, RSAParameters RSAKey, bool DoOAEPPadding)
        {
            try
            {
                byte[] decryptedData;
                using (RSACryptoServiceProvider RSA = new RSACryptoServiceProvider())
                {
                    RSA.ImportParameters(RSAKey);
                    decryptedData = RSA.Decrypt(Data, DoOAEPPadding);
                }
                return decryptedData;
            }
            catch (CryptographicException e)
            {
                Console.WriteLine(e.ToString());
                return null;
            }
        }
    }
}
