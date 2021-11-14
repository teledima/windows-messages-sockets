using System.IO;
using System.Security.Cryptography;

namespace HelperSockets
{

    public class DesService
    {
        private readonly DES desService;

        public byte[] Key { 
            get { return desService.Key; } 
            set { desService.Key = value; } 
        }

        public byte[] IV
        {
            get { return desService.IV; }
            set { desService.IV = value; }
        }

        public DesService()
        {
            desService = DES.Create();
            desService.GenerateKey();
            desService.GenerateIV();
        }
        public DesService(byte[] key, byte[] iv)
        {
            desService = DES.Create();
            Key = key;
            IV = iv;
        }

        public byte[] Encrypt(byte[] data)
        {
            if (data.Length == 0)
                return new byte[0];
            using var encryptor = desService.CreateEncryptor(Key, IV);
            using var mStream = new MemoryStream();
            using var cStream = new CryptoStream(mStream, encryptor, CryptoStreamMode.Write);
            cStream.Write(data, 0, data.Length);
            cStream.FlushFinalBlock();

            return mStream.ToArray();
        }

        public byte[] Decrypt(byte[] data)
        {
            using var decryptor = desService.CreateDecryptor(Key, IV);
            using var mStream = new MemoryStream(data);
            using var cStream = new CryptoStream(mStream, decryptor, CryptoStreamMode.Read);

            var decoded_content = new byte[data.Length];
            cStream.Read(decoded_content, 0, decoded_content.Length);
            return decoded_content;
        }
    }
}
