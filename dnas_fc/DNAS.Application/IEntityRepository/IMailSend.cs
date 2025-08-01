using DNAS.Domian.DTO.Confguration;

namespace DNAS.Application.IRepository
{
    public interface IMailSend
    {
        public Task<IEnumerable<ConfigurationRespModel>> fetchConfiguration(object inparam);
    }
}
