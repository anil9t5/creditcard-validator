using System;
using System.Data;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Collections.Generic;

namespace mfiles
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                if (args.Length == 0)
                {
                    Console.WriteLine("Please enter a file path as an argument!");
                    return;
                }

                switch (args[0])
                {
                    case "store":
                        // code block
                        // Console.WriteLine("Inside Store...");
                        ReadTextFile(args[1]);
                        break;
                    case "fetch":
                        // Console.WriteLine("Inside Fetch...");
                        ReadTmpFile(args[1]);
                        // code block
                        break;
                    default:
                        // code block
                        break;
                }
            }
            catch (FileNotFoundException e)
            {
                Console.WriteLine($"The file was not found: '{e}'");
            }
            catch (IOException e)
            {
                Console.WriteLine($"The file could not be opened: '{e}'");
            }
        }

        public static void ReadTextFile(string filePath)
        {
            int counter = 0;
            string line;
            List<string> encryptedDataList = new List<string>();

            // Read the file and display it line by line.  
            System.IO.StreamReader file =
                new System.IO.StreamReader(filePath);
            while ((line = file.ReadLine()) != null)
            {
                if (ValidateCardNumber(line))
                {
                    string encrypted = Encrypt(line);
                    encryptedDataList.Add(encrypted);
                }

                counter++;
            }

            string tmpFile = CreateTmpFile();
            UpdateTmpFile(tmpFile, encryptedDataList);

            file.Close();
        }
        public static bool ValidateCardNumber(string input)
        {
            string cleanInput = input.Replace(" ", String.Empty);
            int[] cardNum = new int[cleanInput.Length];

            for (int i = 0; i < cleanInput.Length; i++)
            {
                cardNum[i] = (int)(cleanInput[i] - '0'); //Converting character into integer...
            }

            for (int i = cardNum.Length - 2; i >= 0; i = i - 2)
            {
                int temp = cardNum[i];
                temp = temp * 2;
                if (temp > 9) //Check for two digit number after doubling...
                {
                    temp = temp % 10 + 1;
                }
                cardNum[i] = temp;
            }


            int sum = 0; //Adding up all the digits after doubling..
            for (int i = 0; i < cardNum.Length; i++)
            {
                sum += cardNum[i];
            }
            return (sum % 10 == 0) ? true : false; //If the number is a multiple of 10, then only the number is valid

        }



        static string CreateTmpFile()
        {
            string fileName = string.Empty;

            try
            {
                // Get the full name of the newly created tmp file. 
                fileName = Path.GetTempFileName();

                // FileInfo object sets the file's attributes
                FileInfo fileInfo = new FileInfo(fileName);

                // Setting the Attribute property of this file to Temporary. 
                fileInfo.Attributes = FileAttributes.Temporary;
            }
            catch (Exception ex)
            {
                Console.WriteLine("Unable to create TEMP file or set its attributes: " + ex.Message);
            }

            return fileName;
        }

        static void UpdateTmpFile(string tmpFile, List<string> list)
        {
            try
            {
                // Write to the temp file.
                StreamWriter streamWriter = File.AppendText(tmpFile);
                foreach (string item in list)
                {
                    streamWriter.WriteLine(item);
                }
                streamWriter.Flush();
                streamWriter.Close();

                Console.WriteLine("Encrypted data stored here: " + tmpFile);

            }
            catch (Exception ex)
            {
                Console.WriteLine("Error writing to TEMP file: " + ex.Message);
            }
        }


        static void ReadTmpFile(string tmpFile)
        {
            try
            {
                // Read from the temp file.
                string line;
                int counter = 0;

                System.IO.StreamReader file =
                new System.IO.StreamReader(tmpFile);
                Console.WriteLine("File contained the following credit cards: \n");
                while ((line = file.ReadLine()) != null)
                {

                    Console.WriteLine(Decrypt(line));
                    counter++;

                }
                file.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error reading TEMP file: " + ex.Message);
            }
        }

        static string Encrypt(string value)
        {
            string hash = "credit@2021$";
            byte[] data = UTF8Encoding.UTF8.GetBytes(value);

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();  //Returns the hash as an array of 16 bytes
            TripleDESCryptoServiceProvider tripDES = new TripleDESCryptoServiceProvider(); //Creates an object, later used to encrypt or decrypt data.

            tripDES.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            tripDES.Mode = CipherMode.ECB;

            ICryptoTransform transform = tripDES.CreateEncryptor();
            byte[] result = transform.TransformFinalBlock(data, 0, data.Length);

            return Convert.ToBase64String(result);
        }

        static string Decrypt(string value)
        {
            string hash = "credit@2021$";
            byte[] data = Convert.FromBase64String(value);

            MD5CryptoServiceProvider md5 = new MD5CryptoServiceProvider();
            TripleDESCryptoServiceProvider tripDES = new TripleDESCryptoServiceProvider();

            tripDES.Key = md5.ComputeHash(UTF8Encoding.UTF8.GetBytes(hash));
            tripDES.Mode = CipherMode.ECB;

            ICryptoTransform transform = tripDES.CreateDecryptor();
            byte[] result = transform.TransformFinalBlock(data, 0, data.Length);

            return UTF8Encoding.UTF8.GetString(result);
        }
    }
}

