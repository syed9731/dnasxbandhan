namespace DNAS.Domain.Common
{
    public enum MailBodyConcentData
    {
        Creators_Name,
        Approvers_Name,
        Note_Name,
        Delegates_Name,
        Link,
        Query_provided_by_Approver,
        Comment_provided_by_Approver,
        ApproverA_name,
        ApproverB_Name,
        Users_Name,
		Creator_Comment,
		All_Approvers
	}
    public static class MailBodyLink
    {
        public const string ApprovalRequest = "/Note/ApprovalRequest?m=";
        public const string NoteView = "/Note/Views?m=";
        public const string NoteViewFyi = "/Note/ViewFyi?m=";
        public const string NoteWithdraw = "/Note/Withdraw?m=";
    }


}
