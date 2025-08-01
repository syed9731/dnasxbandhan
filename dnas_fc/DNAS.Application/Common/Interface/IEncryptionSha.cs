namespace DNAS.Application.Common.Interface
{
    internal interface IEncryptionSha
    {
        public string EncryptionSha256Hash(string PlainText);
    }
}
