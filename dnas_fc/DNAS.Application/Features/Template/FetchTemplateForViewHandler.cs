using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Template;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace DNAS.Application.Features.Login
{
    public class FetchTemplateForViewCommand(TemplateModel templateModel) : IRequest<TemplateModel>
    {
        public TemplateModel _template { get; set; } = templateModel;
    }
    internal sealed class FetchTemplateForViewHandler(ITemplate iTemplate, ICustomLogger logger, IEncryption enc, IHttpContextAccessor haccess) : IRequestHandler<FetchTemplateForViewCommand, TemplateModel>
    {
        private readonly ITemplate _iTemplate = iTemplate;
        public readonly ICustomLogger _logger = logger;
        public readonly IEncryption _encryption = enc;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<TemplateModel> Handle(FetchTemplateForViewCommand Request, CancellationToken cancellationToken)
        {
            TemplateModel Response = new();
            try
            {
                var inparam = new
                {
                    @templateid = Request._template.TemplateId,
                    @UserId= Request._template.UserId
                };

                Response = await _iTemplate.ViewTemplate(inparam);

                if (Response != null)
                {
                    Response.TemplateId = _encryption.AesEncrypt(Response.TemplateId);
                    _logger.LogwriteInfo("Template data Fetch successfully", loginUserId);
                    return Response;
                }
                else
                {
                    _logger.LogwriteInfo("No Data Found in the Table", loginUserId);
                    return Response = new();
                }
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during FetchTemplateForViewCommand execution" +
                Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, loginUserId);
                return Response;
            }
        }
    }
}