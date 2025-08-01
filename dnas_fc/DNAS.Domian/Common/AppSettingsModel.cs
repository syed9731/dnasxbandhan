namespace DNAS.Domian.Common
{
    public static class AllDefaultConstants
    {
        public const string AppConfig = nameof(AppConfig);
        public const string APIServerUrl = nameof(APIServerUrl);
        public const string ConnectionStrings = nameof(ConnectionStrings);
    }

    public class AppConfig
    {
        public string LogWritePaths { get; set; } = string.Empty;
        public string BaseUrl { get; set; } = string.Empty;
        public string EnSheKeValue { get; set; } = string.Empty;
        public string RecoveryMailReSendTimeInMinutes {  get; set; } = string.Empty;
        public string MailOtpValidateTime {  get; set; } = string.Empty;
        public bool IsProduction { get; set; }
        public string FileUploadPath { get; set; } = string.Empty;
        public string AdPath { get; set; } = string.Empty;
        public string Ad_Domain { get; set; } = string.Empty;
        public string ByPassOtp { get; set; } = string.Empty;
        public string AllowedCharacter { get; set; } = string.Empty;
        public string AllowedCharacterForJs { get; set; } = string.Empty;
    }
    
    public class ConnectionStrings
    {
        public string SQLConnection { get; set; } = string.Empty;
    }
}
