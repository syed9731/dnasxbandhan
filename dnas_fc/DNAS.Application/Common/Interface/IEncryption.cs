namespace DNAS.Application.Common.Interface
{
    public interface IEncryption
    {
        public string AesEncrypt(string PlainText);
        public string AesEncryptForEmail(string PlainText);
        public string AesEncrypt(string PlainText, string enckey);
        public string AesDecrypt(string CipherText);
        public string AesDecryptForEmail(string CipherText);
        public string AesDecrypt(string CipherText, string enckey);
        public string DecryptStringAES(string CifherText);
        public string DecryptStringAES(string CifherText, string key);
    }
}
