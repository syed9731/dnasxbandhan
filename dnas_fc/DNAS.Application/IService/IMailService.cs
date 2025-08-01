using DNAS.Domian.DTO.MailSend;

namespace DNAS.Application.IService
{
    public interface IMailService
    {
        Task<bool> EmailSend(MailSender Request);
    }
}
