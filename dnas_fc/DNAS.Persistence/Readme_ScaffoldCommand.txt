dotnet ef dbcontext scaffold "Server=10.0.3.105;Database=DNASDB;User ID=sa;Password=bfsdb@634;TrustServerCertificate=True;" Microsoft.EntityFrameworkCore.SqlServer --context-dir DataAccessContents -c DataContext -o DataAccessContents -p DNAS.Persistence -f

using Microsoft.Extensions.Configuration;
protected readonly IConfiguration Configuration;
    public DataContext(IConfiguration configuration)
    {
        Configuration = configuration;
    }
=> optionsBuilder.UseSqlServer(Configuration.GetConnectionString("SQLConnection"));

dotnet user-secrets init -- For setup the user secret
dotnet user-secrets list -- For check list of secret keys
dotnet user-secrets set "ConnectionStrings:SQLConnection" "Server=10.0.3.105;Database=BB_DNAS_PROD;User ID=sa;Password=bfsdb@634;TrustServerCertificate=True"; -- Add DB keys
dotnet user-secrets set "AppConfig:BaseURL" "https://localhost:7072"; -- Add baseurl keys
dotnet user-secrets remove "AuthorApiKey" -- Remove specific keys
dotnet user-secrets clear -- Clear all keys
