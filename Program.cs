using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Crypto3
{
    static class User1
    {
        public static RSAParameters publicSignedKey;
        public static byte[] encryptedSimmetricKey;


        public static void StartCrypto()
        {
            Aes aes = Aes.Create();
            RSA rsa = RSA.Create();
            rsa.ImportParameters(User2.publicKey);
            encryptedSimmetricKey = rsa.Encrypt(aes.Key, RSAEncryptionPadding.Pkcs1);

            using (OpenFileDialog file = new OpenFileDialog())
            {
                file.Filter = "Текстовые файлы(*.txt)|*.txt|Все файлы(*.*)|*.*";

                if (file.ShowDialog() == DialogResult.OK)
                {
                    string content = File.ReadAllText(file.FileName);
                    SendToUser(aes.Key, content, out byte[] encryptedMessage, out byte[] IV, out byte[] signedEncryptedMessage);
                    User2.RecieveData(encryptedMessage, IV, signedEncryptedMessage);
                }
            }
        }

        private static void SendToUser(byte[] key, string content, out byte[] encryptedMessage, out byte[] iV, out byte[] signedEncryptedMessage)
        {
            Aes aes = Aes.Create();
            aes.Key = key;
            iV = aes.IV;

            using (MemoryStream memoryStream = new MemoryStream())
            {
                using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                                    aes.CreateEncryptor(),
                                                                    CryptoStreamMode.Write))
                {
                    byte[] message = Encoding.UTF8.GetBytes(content);
                    cryptoStream.Write(message, 0, message.Length);
                }
                encryptedMessage = memoryStream.ToArray();
                RSA signedRsa = RSA.Create();
                publicSignedKey = signedRsa.ExportParameters(false);
                signedEncryptedMessage = signedRsa.SignData(encryptedMessage, HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1);
            }
        }
    }
    static class User2
    {
        public static RSAParameters publicKey;
        static RSAParameters privateKey;
        static byte[] simmetricKey;

        static User2()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Был вызван статический конструктор!");
            Console.ResetColor();

            RSA rsa = RSA.Create();
            privateKey = rsa.ExportParameters(true);
            publicKey = rsa.ExportParameters(false);
        }

        internal static void RecieveData(byte[] encryptedMessage, byte[] iV, byte[] signedEncryptedMessage)
        {
            RSA signedRsa = RSA.Create();
            signedRsa.ImportParameters(User1.publicSignedKey);

            if (signedRsa.VerifyData(encryptedMessage, signedEncryptedMessage,
                                    HashAlgorithmName.SHA256, RSASignaturePadding.Pkcs1))
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("Цифровая подпись проверена!");
                Console.ResetColor();

                Aes aes = Aes.Create();
                RSA rsa = RSA.Create();

                rsa.ImportParameters(privateKey);
                simmetricKey = rsa.Decrypt(User1.encryptedSimmetricKey, RSAEncryptionPadding.Pkcs1);
                aes.Key = simmetricKey;
                aes.IV = iV;

                using (MemoryStream memoryStream = new MemoryStream())
                {
                    using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                                        aes.CreateDecryptor(),
                                                                        CryptoStreamMode.Write))
                    {
                        cryptoStream.Write(encryptedMessage, 0, encryptedMessage.Length);
                    }
                    string message = Encoding.UTF8.GetString(memoryStream.ToArray());

                    using (SaveFileDialog file = new SaveFileDialog())
                    {
                        file.Filter = "Текстовые файлы(*.txt)|*.txt";
                        file.FileName = "Расшифрованные данные";
                        file.DefaultExt = "txt";

                        if (file.ShowDialog() == DialogResult.OK)
                        {
                            File.WriteAllText(file.FileName, message);
                        }
                    }
                }
            }
            else
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("Цифровая подпись не проверена!");
                Console.ResetColor();
            }
        }
    }
    class Program
    {
        [STAThread] // Означает, что все потоки в этой программе выполняются в рамках одного процесса,
                    // а управление программой осуществляется одним главным потоком
        static void Main()
        {
            User1.StartCrypto();
        }
    }
}

