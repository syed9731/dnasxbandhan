using DNAS.Domain.DTO.SendBack;
using DNAS.Domian.DTO.DelegateAsign;
using DNAS.Domian.DTO.MailSend;

namespace DNAS.Application.Business.Interface
{
    interface IEmailService
    {
        Task<MailSender> GetMailConfiguration();
        Task<DeligateBodySubject> GetMailNoteCreatorMailForDelegate(DeligateMail deligateMail);
        Task<DeligateBodySubject> GetMailNoteCreatorMailForWhome(DeligateMail deligateMail);
        Task<DeligateBodySubject> GetMailNoteCreatorQueryInitiate(DeligateMail deligateMail);
        Task<DeligateBodySubject> GetMailNextNoteApprover(DeligateMail deligateMail);
        Task<DeligateBodySubject> GetMailNextNoteApproverAfterSkip(DeligateMail deligateMail);
        Task<DeligateBodySubject> GetMailApproverApprovedToCreator(DeligateMail deligateMail);
        Task<DeligateBodySubject> GetMailApproverApprovedToCreatorFinal(DeligateMail deligateMail);
        Task<DeligateBodySubject> GetMailNoteQueryReply(DeligateMail deligateMail);
        Task<DeligateBodySubject> GetMailSendBackToCreator(DeligateMail deligateMail);
        Task<DeligateBodySubject> GetMailNoteFyiToUser(DeligateMail deligateMail);
        Task<DeligateBodySubject> GetMailToApproverForNoteAsign(DeligateMail deligateMail);
        Task<DeligateBodySubject> GetMailToCreatorDelegate(DeligateMail deligateMail);
        Task<DeligateBodySubject> GetMailToSkip(DeligateMail deligateMail);
        Task<MailBodySubject> GetMailConfigurationData(MailModel deligateMail, string MailKey);
        Task<DeligateBodySubject> GetMailToApproverForReSubmitNote(ApproverDtlModel approverDtlModel);


    }
}
