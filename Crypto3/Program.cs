using System;
using System.Security.Cryptography;

namespace Crypto3
{
    class Program
    {
        static void Main()
        {
            Menu();
        }

        private static void Menu()
        {
            Console.WriteLine("Выберите действие:" +
                "1 - Генерация ключей RSA" +
                "2 - Шифрование и расшифрование документа симметричным криптоалгоритмом" +
                "3 - Шифрование и расшифрование сеансового ключа симметричного алгоритма при помощи ключей RSA" +
                "4 - Формирование и проверку цифровой подписи документа" +
                "0 - Выход\n");
            while (true)
            {
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
            throw new NotImplementedException();
        }

        private static void GenerateRSAKey()
        {
            throw new NotImplementedException();
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
