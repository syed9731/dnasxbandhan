using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Template;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.Template
{
    public class DeleteTemplateCommand(TemplateModel note):IRequest<string>
    {
        public TemplateModel _tempmod { get; set; } = note;
    }
    internal sealed class DeleteTemplateHandler(IUpdate iupdate, ICustomLogger logger, IHttpContextAccessor haccess) : IRequestHandler<DeleteTemplateCommand, string>
    {
        private readonly IUpdate _Update = iupdate;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<string> Handle(DeleteTemplateCommand request, CancellationToken cancellationToken)
        {

            string Response = "";
            try
            {
                TemplateModel template =new();                
                template.TemplateId = request._tempmod.TemplateId;
                template.IsActive=request._tempmod.IsActive;
                Response = await _Update.DeleteTemplateData(template);

                if (Response == "success")
                {
                    _logger.LogwriteInfo("Delete Template command successfully done", loginUserId);
                }
                else
                {
                    _logger.LogwriteInfo("Delete Template command failed", loginUserId);
                }
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during DeleteTemplateCommand execution" +
                    Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, loginUserId);
                return Response;
            }

        }

    }
}
