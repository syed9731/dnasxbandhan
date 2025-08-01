using DNAS.Application.Common.Interface;
using DNAS.Application.IRepository;
using DNAS.Domian.DTO.Template;
using MediatR;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;


namespace DNAS.Application.Features.Template
{
    public class SaveTemplateCommand(TemplateModel temp):IRequest<string>
    {
        public TemplateModel _template { get; set; } = temp;
    }
    internal sealed class SaveTemplateHandler(ISave isave, ICustomLogger logger, IHttpContextAccessor haccess) : IRequestHandler<SaveTemplateCommand, string>
    {
        private readonly ISave _Update = isave;
        public readonly ICustomLogger _logger = logger;
        private readonly string loginUserId = $"User_{haccess.HttpContext?.User.FindFirstValue("UserId")}";
        public async Task<string> Handle(SaveTemplateCommand request, CancellationToken cancellationToken)
        {

            string Response = "";
            try
            {                
                Response = await _Update.SaveTemplate(request._template);

                if (Response == "success")
                {
                    _logger.LogwriteInfo("Update Note Category command successfully done", loginUserId);
                }
                else
                {
                    _logger.LogwriteInfo("Update Note Category command failed", loginUserId);
                }
                return Response;
            }
            catch (Exception ex)
            {
                _logger.LogwriteInfo("exception occur during SaveTemplateCommand execution" +
                Environment.NewLine + "exception message-" + ex.Message + Environment.NewLine + ex.StackTrace, loginUserId);
                return Response;
            }

        }

    }
}
