using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Template;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.Template
{
    public class UpdateTemplateCommand(TemplateModel note):IRequest<bool>
    {
        public TemplateModel _tempmod { get; set; } = note;
    }
    internal sealed class UpdateTemplateHandler(IUpdate iupdate, ICustomLogger logger, IHttpContextAccessor haccess) : IRequestHandler<UpdateTemplateCommand, bool>
    {
        private readonly IUpdate _Update = iupdate;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";

        public async Task<bool> Handle(UpdateTemplateCommand request, CancellationToken cancellationToken)
        {

            bool Response = false;
            try
            {
                TemplateModel template =new();
                template.TemplateName=request._tempmod.TemplateName;
                template.TemplateBody = request._tempmod.TemplateBody;
                template.TemplateId = request._tempmod.TemplateId;
                template.DateOfCreation=request._tempmod.DateOfCreation;
                Response = await _Update.UpdateTemplateData(template);
                if (Response)
                {
                    _logger.LogwriteInfo("Update Template command successfully done", loginUserId);
                }
                else
                {
                    _logger.LogwriteInfo("Update Template command failed", loginUserId);
                }
                return Response;
            }
            catch (Exception ex)
            {                
                _logger.LogwriteInfo("exception occur during UpdateTemplateCommand execution" +
                Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, loginUserId);
                return Response;
            }

        }

    }
}
