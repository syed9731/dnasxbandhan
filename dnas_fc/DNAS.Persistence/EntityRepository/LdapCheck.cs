using DNAS.Application.Common.Interface;
using DNAS.Application.IEntityRepository;
using DNAS.Domian.Common;
using Microsoft.Extensions.Options;
using System.DirectoryServices;
using DirectoryEntry = System.DirectoryServices.DirectoryEntry;
namespace DNAS.Persistence.EntityRepository
{
    internal class LdapCheck(ICustomLogger customLogger, IOptions<AppConfig> appConfig) : ILdapCheck
    {
        private readonly ICustomLogger _logger = customLogger;
        public async Task<bool> CheckLdapUser(string adPath, string username, string password, string domain)
        {
            try
            {
                _logger.LogwriteInfo("Insert CheckLdapUser method for username-" + username + " password- " + password + " adpath-" + adPath + " domain-" + domain, "Ldap");
                string domainAndUsername = domain;
                // Create a new DirectoryEntry object with the specified credentials
                using (var entry = new DirectoryEntry(adPath, domainAndUsername, password))
                {
                    // Create a DirectorySearcher object with the DirectoryEntry
                    using (var searcher = new DirectorySearcher(entry))
                    {
                        // Set the search filter and properties to load
                        searcher.Filter = "(sAMAccountName=" + username + ")";
                        searcher.PropertiesToLoad.Add("cn");
                        // Perform the search
                        SearchResult result = searcher.FindOne();
                        // Check if the result is null
                        if (result == null)
                        {
                            _logger.LogwriteInfo("LDAP Failure Username: " + username, "Ldap");
                            return false;
                        }
                        else
                        {
                            _logger.LogwriteInfo("LDAP Success Username: " + username, "Ldap");
                            return true;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogwriteInfo("exception occur during CheckLdapUser------ " + e.Message + Environment.NewLine + e.StackTrace, "Ldap");
                //if (username == "277230")
                //{
                //    _logger.LogwriteInfo("User id is------ 277230 so pass true", "Ldap");
                //    return true;
                //}
                return false;
            }
        }
    }
}