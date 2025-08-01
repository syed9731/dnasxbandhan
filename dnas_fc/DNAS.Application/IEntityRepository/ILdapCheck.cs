namespace DNAS.Application.IEntityRepository
{
    public interface ILdapCheck
    {
        public Task<bool> CheckLdapUser(string adPath, string username, string password, string domain);
    }
}