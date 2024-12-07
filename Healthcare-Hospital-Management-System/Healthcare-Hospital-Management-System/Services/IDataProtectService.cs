using System.Threading;
using System.Threading.Tasks;

namespace Healthcare_Hospital_Management_System.Services
{
    public interface IDataProtectService
    {
        string ExportPublicKey();
        void ImportPublicKey(string publicKey);
        string ExportPrivateKey();
        void ImportPrivateKeyFromSecrets();
        Task<byte[]> EncryptAsync(string publicKey, string plainText, CancellationToken cancellationToken);
        Task<string> DecryptAsync(byte[] encryptedData, CancellationToken cancellationToken);
    }
}