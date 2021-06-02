using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml;

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

        public static void UploadRSA()
        {
            privateKey = Program.uploadedRSA.ExportParameters(true);
            publicKey = Program.uploadedRSA.ExportParameters(false);
        }

        public static void RecieveData(byte[] encryptedMessage, byte[] iV, byte[] signedEncryptedMessage)
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
        public static Aes uploadedAes = Aes.Create("AES");
        public static RSA uploadedRSA = RSA.Create();

        public static string BynaryWriter { get; private set; }

        [STAThread] // Означает, что все потоки в этой программе выполняются в рамках одного процесса,
                    // а управление программой осуществляется одним главным потоком
        static void Main()
        {
            while (true)
            {
                Console.Write("Выберите действие:\n" +
                                "1 - Сгенерировать сеансовый ключ и сохранить его в файл\n" +
                                "2 - Сгенерировать ключи RSA и сохранить его в файл\n" +
                                "3 - Загрузить сеансовый ключ из файла\n" +
                                "4 - Загрузить ключи RSA из файла\n" +
                                "5 - Запустить криптосистему\n" +
                                "0 - Выход\n" +
                                "--> ");
                switch (Console.ReadLine())
                {
                    case "1":
                        GenerateSessionKey();
                        break;
                    case "2":
                        GenerateRSAKeys();
                        break;
                    case "3":
                        UploadSessionKey();
                        break;
                    case "4":
                        UploadRSAKeys();
                        break;
                    case "5":
                        StartCryptosystem();
                        break;
                    case "0":
                        Environment.Exit(0);
                        break;
                }
            }
        }

        private static void StartCryptosystem()
        {
            throw new NotImplementedException();
        }

        private static void UploadRSAKeys()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Загрузка сеансового ключа...");

            using (OpenFileDialog rsaKeys = new OpenFileDialog())
            {
                rsaKeys.Filter = "Ключи RSA(*.xml)|*.xml";

                if (rsaKeys.ShowDialog() == DialogResult.OK)
                {

                    uploadedRSA.FromXmlString(File.ReadAllText(rsaKeys.FileName));
                }
            }

            Console.WriteLine("Загрузка окончена");
            Console.ResetColor();
        }

        private static void UploadSessionKey()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Загрузка сеансового ключа...");

            using (OpenFileDialog aesKeyFile = new OpenFileDialog())
            {
                aesKeyFile.Filter = "Ключ Aes(*.aesKey)|*.aesKey";

                if (aesKeyFile.ShowDialog() == DialogResult.OK)
                {
                    uploadedAes.Key = File.ReadAllBytes(aesKeyFile.FileName);
                }
            }

            using (OpenFileDialog aesIVFile = new OpenFileDialog())
            {
                aesIVFile.Filter = "Вектор инициализации Aes(*.aesIV)|*.aesIV";

                if (aesIVFile.ShowDialog() == DialogResult.OK)
                {
                    uploadedAes.IV = File.ReadAllBytes(aesIVFile.FileName);
                }
            }

            Console.WriteLine("Загрузка окончена");
            Console.ResetColor();
        }

        private static void GenerateRSAKeys()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Генерация сеансового ключа...");

            using (RSACryptoServiceProvider rsa = new RSACryptoServiceProvider())
            {
                using (SaveFileDialog rsaKeys = new SaveFileDialog())
                {
                    rsaKeys.Filter = "Ключи RSA(*.xml)|*.xml";
                    rsaKeys.FileName = "Ключи RSA";
                    rsaKeys.DefaultExt = "xml";

                    if (rsaKeys.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllText(rsaKeys.FileName, rsa.ToXmlString(true));
                    }
                }
            }

            Console.WriteLine("Генерация окончена");
            Console.ResetColor();
        }

        private static void GenerateSessionKey()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Генерация сеансового ключа...");

            using (Aes aes = Aes.Create("AES"))
            {
                using (SaveFileDialog aesKeyFile = new SaveFileDialog())
                {
                    aesKeyFile.Filter = "Ключ Aes(*.aesKey)|*.aesKey";
                    aesKeyFile.FileName = "Ключ Aes";
                    aesKeyFile.DefaultExt = "aesKey";

                    if (aesKeyFile.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(aesKeyFile.FileName, aes.Key);
                    }
                }

                using (SaveFileDialog aesIVFile = new SaveFileDialog())
                {
                    aesIVFile.Filter = "Вектор инициализации Aes(*.aesIV)|*.aesIV";
                    aesIVFile.FileName = "Вектор инициализации Aes";
                    aesIVFile.DefaultExt = "aesIV";

                    if (aesIVFile.ShowDialog() == DialogResult.OK)
                    {
                        File.WriteAllBytes(aesIVFile.FileName, aes.Key);
                    }
                }
            }

            Console.WriteLine("Генерация окончена");
            Console.ResetColor();
        }
    }
}

