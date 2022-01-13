using System;
using System.Linq;
using System.Security.Cryptography;

namespace AEDDES.Model
{
    public class CryptoKey 
    {
        public byte[] Key { get; private set; }
        public byte[] IV { get; private set; }
        private int LengthKey;
        public CryptoKey(int lengthKey)
        {
            LengthKey = lengthKey;
            GenerateKey();
            GenerateIV();
        }
        public void GenerateKey()
        {
            Aes aes = Aes.Create();
            aes.GenerateKey();
            Key = aes.Key.Take(LengthKey / 8).ToArray();
        }
        public void GenerateIV()
        {
            Aes aes = Aes.Create();
            aes.GenerateIV();
            IV = aes.IV;

        }
    }
}
