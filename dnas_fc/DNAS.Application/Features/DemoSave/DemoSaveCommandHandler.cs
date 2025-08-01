using DNAS.Application.IRepository;
using DNAS.Domian.Common;
using DNAS.Domian.DTO.CommonResponse;
using DNAS.Domian.DTO.FYI;
using MediatR;



namespace DNAS.Application.Features.DemoSave
{
    public class DemoSaveCommand(FyiModel _fyi) : IRequest<CommonResponse<CommonResp>>
    {
        public FyiModel Fyi { get; set; } = _fyi;
    }
    internal class DemoSaveCommandHandler(ISave iSaveData) : IRequestHandler<DemoSaveCommand, CommonResponse<CommonResp>>
    {
        private readonly ISave _iSaveData = iSaveData;

        public async Task<CommonResponse<CommonResp>> Handle(DemoSaveCommand Request, CancellationToken cancellationToken)
        {
            try
            {
                return await _iSaveData.SavedData(Request.Fyi);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                return new CommonResponse<CommonResp>();
            }
        }
    }
}
