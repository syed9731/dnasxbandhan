using DNAS.Application.Common.Interface;
using System.Security.Cryptography;
using System.Text;


namespace DNAS.Application.Common.Implementation
{
    public class EncryptionSha : IEncryptionSha
    {
        public string EncryptionSha256Hash(string PlainText)
        {
            byte[] inputBytes = Encoding.UTF8.GetBytes(PlainText);
            byte[] inputHash = SHA256.HashData(inputBytes);
            return Convert.ToHexString(inputHash);
        }
    }
}
