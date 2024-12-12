using System.Security.Cryptography;
using System.Text;

namespace Healthcare_Hospital_Management_System.Services
{
    public class DataProtectService: IDataProtectService
    {
        private readonly RSA _rsa;
        private readonly IConfiguration _configuration;

        public DataProtectService(IConfiguration configuration)
        {
            _rsa = RSA.Create();
            _configuration = configuration;
        }

        public string ExportPublicKey()
        {
            return Convert.ToBase64String(_rsa.ExportRSAPublicKey());
        }

        public void ImportPublicKey(string publicKey)
        {
            byte[] keyBytes = Convert.FromBase64String(publicKey);
            _rsa.ImportRSAPublicKey(keyBytes, out _);
        }

        public void ImportPrivateKeyFromSecrets()
        {
            string privateKey = _configuration["EncryptionKeys:PrivateKey"];
            if (string.IsNullOrEmpty(privateKey))
            {
                throw new InvalidOperationException("Private key not found in .NET Secrets.");
            }

            byte[] keyBytes = Convert.FromBase64String(privateKey);
            _rsa.ImportRSAPrivateKey(keyBytes, out _);
        }

        public async Task<byte[]> EncryptAsync(string publicKey, string plainText, CancellationToken cancellationToken)
        {
            ImportPublicKey(publicKey);
            byte[] plainBytes = Encoding.UTF8.GetBytes(plainText);
            return await Task.FromResult(_rsa.Encrypt(plainBytes, RSAEncryptionPadding.Pkcs1));
        }

        public async Task<string> DecryptAsync(byte[] encryptedData, CancellationToken cancellationToken)
        {
            ImportPrivateKeyFromSecrets();

            byte[] decryptedBytes = _rsa.Decrypt(encryptedData, RSAEncryptionPadding.Pkcs1);
            return await Task.FromResult(Encoding.UTF8.GetString(decryptedBytes));
        }
    }

}
