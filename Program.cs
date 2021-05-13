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
    class Program
    {
        static byte[] rsaPublicKey;
        static byte[] rsaPriveteKey;

        [STAThread] // Означает, что все потоки в этой программе выполняются в рамках одного процесса,
                    // а управление программой осуществляется одним главным потоком
        static void Main()
        {
            Menu();
        }

        private static void Menu()
        {
            while (true)
            {
                Console.Write("1 - Генерация ключей RSA\n" +
                                "2 - Шифрование и расшифрование документа симметричным криптоалгоритмом\n" +
                                "3 - Шифрование и расшифрование сеансового ключа симметричного алгоритма при помощи ключей RSA\n" +
                                "4 - Формирование и проверку цифровой подписи документа\n" +
                                "0 - Выход\n" +
                                "--> ");
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
                    case "0":
                        Environment.Exit(0);
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

        /// <summary>
        /// Шифрование и расшифрование документа симметричным криптоалгоритмом (Aes256)
        /// </summary>
        private static void SimmetricCryptography()
        {
            Aes aes = Aes.Create();

            StringBuilder key = new StringBuilder();
            foreach (var el in aes.Key)
            {
                key.Append(el);
            }

            StringBuilder iv = new StringBuilder();
            foreach (var el in aes.IV)
            {
                iv.Append(el);
            }


            Console.WriteLine($"Создан ключ для симметричного шифрования {key} и вектор инициализации {iv}" +
                $"\nЕсли Вы покините это меню, то при следующем заходе ключи будут сгенерированы заново");
            while (true)
            {
                Console.Write("1 - Зашифровать файл\n" +
                            "2 - Расшифровать файл\n" +
                            "0 - Назад\n" +
                            "--> ");
                switch (Console.ReadLine())
                {
                    case "1":
                        SimmetricEncrypt(aes);
                        break;
                    case "2":
                        SimmetricDecrypt(aes);
                        break;
                    case "0":
                        return;
                }
            }
        }

        /// <summary>
        /// Расшифрование документа симметричным алгоритмом
        /// </summary>
        /// <param name="aes">Объект класса Aes, содержащий ключ шифрования и вектор инициализации</param>
        private static void SimmetricDecrypt(Aes aes)
        {
            using (OpenFileDialog file = new OpenFileDialog())
            {
                file.Filter = "Текстовые файлы(*.txt)|*.txt|Все файлы(*.*)|*.*";

                if (file.ShowDialog() == DialogResult.OK)
                {
                    string result;
                    byte[] fileContent = File.ReadAllBytes(file.FileName);  // Считываем шифротекст из файла
                    ICryptoTransform cryptoTransform = aes.CreateDecryptor(aes.Key, aes.IV);  // Создаем объект дешифровщика

                    using (MemoryStream memoryStream = new MemoryStream(fileContent))  // Поток памяти для записи результата
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream, 
                                                                            cryptoTransform, 
                                                                            CryptoStreamMode.Read))  // Поток, который будет передавать шифротекст дешифровальщику с последующим расшифрованием
                        {
                            using(StreamReader streamReader = new StreamReader(cryptoStream))  // Объект, который будет передавать шифротекст потоку для дешифрования
                            {
                                result = streamReader.ReadToEnd();
                            }
                        }
                    }

                    using (SaveFileDialog saveFile = new SaveFileDialog())
                    {
                        saveFile.Filter = "Текстовые файлы(*.txt)|*.txt|Все файлы(*.*)|*.*";
                        saveFile.FileName = "Расшифрованные данные";
                        saveFile.DefaultExt = "txt";

                        if (saveFile.ShowDialog() == DialogResult.OK)
                        {
                            //File.WriteAllText(file.FileName, result.ToString());
                            File.WriteAllText(saveFile.FileName, result);  // Записываем расшифрованный текст в файл
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Шифрование документа симметричным алгоритмом
        /// </summary>
        /// <param name="aes">Объект класса Aes, содержащий ключ шифрования и вектор инициализации</param>
        private static void SimmetricEncrypt(Aes aes)
        {
            using (OpenFileDialog file = new OpenFileDialog())
            {
                file.Filter = "Текстовые файлы(*.txt)|*.txt|Все файлы(*.*)|*.*";

                if (file.ShowDialog() == DialogResult.OK)
                {
                    string fileContent = File.ReadAllText(file.FileName);  // Считываем с выбранного файла весь текст

                    byte[] encrypted;
                    ICryptoTransform cryptoTransform = aes.CreateEncryptor(aes.Key,
                                                                           aes.IV);  // Создаем объект шифровальщика

                    using (MemoryStream memoryStream = new MemoryStream())  // Поток памяти для записи результата
                    {
                        using (CryptoStream cryptoStream = new CryptoStream(memoryStream,
                                                                            cryptoTransform,
                                                                            CryptoStreamMode.Write))  // Поток, который будет передавать текст шифровальщику с последующим шифрованием
                        {
                            using (StreamWriter streamWriter = new StreamWriter(cryptoStream))  // Объект, который будет передавать символы потоку для шифрования
                            {
                                streamWriter.Write(fileContent);
                            }
                        }

                        encrypted = memoryStream.ToArray();
                    }

                    /* StringBuilder result = new StringBuilder();
                     foreach (var el in encrypted)
                    {
                        result.Append(el);
                    }

                    Console.WriteLine($"Результат шифрования: {result}");*/

                    using (SaveFileDialog saveFile = new SaveFileDialog())
                    {
                        saveFile.Filter = "Текстовые файлы(*.txt)|*.txt|Все файлы(*.*)|*.*";
                        saveFile.FileName = "Файл с шифротекстом";
                        saveFile.DefaultExt = "txt";

                        if (saveFile.ShowDialog() == DialogResult.OK)
                        {
                            //File.WriteAllText(file.FileName, result.ToString());
                            File.WriteAllBytes(saveFile.FileName, encrypted);  // Записываем зашифрованный текст в файл
                        }
                    }
                }
            }
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
