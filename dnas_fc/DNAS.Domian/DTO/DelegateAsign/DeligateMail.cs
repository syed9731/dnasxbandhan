namespace DNAS.Domian.DTO.DelegateAsign
{
    public class DeligateMail
    {
        public string notecreator { get; set; }=string.Empty;
        public string delegateSender { get; set; } = string.Empty;
        public string delegateReceiver { get; set; } = string.Empty;
        public string NoteTitle { get; set; } = string.Empty;
        public string noteApprover { get; set; }= string.Empty;
        public string nextApprover {  get; set; }=string.Empty;
        public string FyiSender { get; set; } = string.Empty;
        public string FyiReceiver { get; set; } = string.Empty;
        public string notecomment {  get; set; } = string.Empty;
        public string noteId {  get; set; } = string.Empty;
		public string Comment { get; set; } = string.Empty;
	}

    public class DeligateBodySubject
    {
        public string Subject { get; set; } = string.Empty;
        public string Body { get; set; } = string.Empty;
    }

	public class MailModel
	{
		public string notecreator { get; set; } = string.Empty;
		public string delegateSender { get; set; } = string.Empty;
		public string delegateReceiver { get; set; } = string.Empty;
		public string NoteTitle { get; set; } = string.Empty;
		public string noteApprover { get; set; } = string.Empty;
		public string nextApprover { get; set; } = string.Empty;
		public string FyiSender { get; set; } = string.Empty;
		public string FyiReceiver { get; set; } = string.Empty;
		public string notecomment { get; set; } = string.Empty;
		public string noteId { get; set; } = string.Empty;
		public string Comment { get; set; } = string.Empty;
		public string All_Approvers { get; set; } = string.Empty;
	}
	public class MailBodySubject
	{
		public string Subject { get; set; } = string.Empty;
		public string Body { get; set; } = string.Empty;
	}
}
