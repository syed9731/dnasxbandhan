namespace DNAS.Domian.DTO.Confguration
{
    public class ConfigurationRespModel
    {
        public IEnumerable<ConfigurationResp> configurationResp { get; set; } = [];
    }
    public class ConfigurationResp
    {
        public int ConfigurationId { get; set; }
        public string ConfigurationFor { get; set; } = string.Empty;
        public string ConfigurationKey { get; set; } = string.Empty;
        public string ConfigurationValue { get; set; } = string.Empty;
    }
}
