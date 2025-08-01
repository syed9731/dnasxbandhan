using DNAS.Domian.Common;
using DNAS.Domian.DTO.Confguration;

namespace DNAS.Application.IRepository
{
    public interface IConfiguration
    {
        public Task<CommonResponse<ConfigurationRespModel>> FetchConfiguration(object inparam);
    }
}