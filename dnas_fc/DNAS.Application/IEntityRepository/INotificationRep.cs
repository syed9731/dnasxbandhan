using DNAS.Domian.Common;
using DNAS.Domian.DTO.Draft;

namespace DNAS.Application.IRepository
{
    public interface INotificationRep
    {
        Task<CommonResponse<HederNotificationsList>> HeaderNotificationList(object inparam);
    }
}
