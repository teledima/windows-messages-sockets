using System;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;

namespace HelperSockets
{
    [Serializable]
    public class AesService
    {
        public byte[] Key { get; set; }

        public byte[] IV { get; set; }

        public AesService()
        {
            var aesService = Aes.Create();
            aesService.KeySize = 256;
            aesService.GenerateKey();
            aesService.GenerateIV();

            Key = aesService.Key;
            IV = aesService.IV;
        }
        public AesService(byte[] key, byte[] iv)
        {
            Key = key;
            IV = iv;
        }

        public byte[] Encrypt(byte[] data)
        {
            if (data.Length == 0)
                return new byte[0];
            using var encryptor = Aes.Create().CreateEncryptor(Key, IV);
            using var mStream = new MemoryStream();
            using var cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write);
            cStream.Write(data, 0, data.Length);
            cStream.FlushFinalBlock();

            return mStream.ToArray();
        }

        public byte[] Decrypt(byte[] data)
        {
            using var decryptor = Aes.Create().CreateDecryptor(Key, IV);
            using var mStream = new MemoryStream(data);
            using var cStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read);

            var decoded_content = new byte[data.Length];
            cStream.Read(decoded_content, 0, decoded_content.Length);
            return decoded_content;
        }

        public byte[] Serialize()
        {
            var binaryFormatter = new BinaryFormatter();
            using var mStream = new MemoryStream();
            binaryFormatter.Serialize(mStream, this);
            return mStream.ToArray();
        }

        public static AesService FromBytes(byte[] data)
        {
            var binaryFormatter = new BinaryFormatter();
            using var mStream = new MemoryStream(data);
            return (AesService)binaryFormatter.Deserialize(mStream);
        }
    }
}
