using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace AEDDES.Model
{
    public enum TypeCrypt : short
    {
        AES,
        DES
    }
    class Encrypting 
    {
        private CryptoKey Key;
        public Encrypting(CryptoKey key)
        {
            Key = key;
        }
        public void Encrypt(string fileInput, string fileOutput, TypeCrypt type)
        {
            if (type == TypeCrypt.AES)
                File.WriteAllBytes(fileOutput, EncryptAES(File.ReadAllText(fileInput)));
            else
                EncryptDES(fileInput, fileOutput);
        }
        public void Decrypt(string fileInput, string fileOutput , TypeCrypt type)
        {
            byte[] inputByte = File.ReadAllBytes(fileInput);

            if (type == TypeCrypt.AES)
                File.WriteAllText(fileOutput, DecryptAES(inputByte));
           else
                DecryptDES(fileInput, fileOutput);
        }
        private byte[] EncryptAES(string plainText)
        {;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key.Key;
                aesAlg.IV = Key.IV;

                ICryptoTransform encryptor = aesAlg.CreateEncryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msEncrypt = new MemoryStream())
                {
                    using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
                    {
                        using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
                        {
                            swEncrypt.Write(plainText);
                        }
                        return msEncrypt.ToArray();
                    }
                }
            }
        }

        private string DecryptAES(byte[] cipherText)
        {
            if (cipherText == null || cipherText.Length <= 0)
                throw new ArgumentNullException("cipherText");

            string plaintext = null;
            using (Aes aesAlg = Aes.Create())
            {
                aesAlg.Key = Key.Key;
                aesAlg.IV = Key.IV;

                ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

                using (MemoryStream msDecrypt = new MemoryStream(cipherText))
                {
                    using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                    {
                        using (StreamReader srDecrypt = new StreamReader(csDecrypt))
                        {
                            plaintext = srDecrypt.ReadToEnd();
                        }
                    }
                }
            }
            return plaintext;
        }
        private void EncryptDES(string inName, string outName)
        {
            //Create the file streams to handle the input and output files.
            byte[] input = File.ReadAllBytes(inName);
            FileStream fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            //Create variables to help with read and write.
            int rdlen = 0;              //This is the total number of bytes written.
            int totlen = input.Length;    //This is the total length of the input file.
            int len;                     //This is the number of bytes to be written at a time.

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(fout, tdes.CreateEncryptor(Key.Key, Key.IV), CryptoStreamMode.Write);

            //Read from the input file, then encrypt and write to the output file.
            while (rdlen < totlen)
            {
                byte[] bin = input.Skip(rdlen).Take(100).ToArray();
                len = bin.Length;
                encStream.Write(bin, 0, len);
                rdlen = rdlen + len;
            }
            encStream.Close();
        }
        public void DecryptDES(string inName, string outName)
        {
            //Create the file streams to handle the input and output files.
            byte[] input = File.ReadAllBytes(inName);
            FileStream fout = new FileStream(outName, FileMode.OpenOrCreate, FileAccess.Write);
            fout.SetLength(0);

            //Create variables to help with read and write.
            int rdlen = 0;              //This is the total number of bytes written.
            int totlen = input.Length;    //This is the total length of the input file.
            int len;                     //This is the number of bytes to be written at a time.

            TripleDESCryptoServiceProvider tdes = new TripleDESCryptoServiceProvider();
            CryptoStream encStream = new CryptoStream(fout, tdes.CreateDecryptor(Key.Key, Key.IV), CryptoStreamMode.Write);

            //Read from the input file, then encrypt and write to the output file.
            while (rdlen < totlen)
            {
                byte[] bin = input.Skip(rdlen).Take(100).ToArray();
                len = bin.Length;
                encStream.Write(bin, 0, len);
                rdlen = rdlen + len;
            }
            if(totlen % 48 != 0)
            {
                fout.Close();
                File.WriteAllBytes(outName, input);
                throw new Exception();
            }
            encStream.Close();
        }     
    }
}

