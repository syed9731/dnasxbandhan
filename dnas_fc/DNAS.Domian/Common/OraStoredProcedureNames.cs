namespace DNAS.Domian.Common
{
    public static class OraStoredProcedureNames
    {
        public const string ProcUserLogin = nameof(OraStoredProcedureNames.ProcUserLogin);
        public const string ProcNotificationIsRead = nameof(OraStoredProcedureNames.ProcNotificationIsRead);
        public const string ProcGetSearchNote = nameof(OraStoredProcedureNames.ProcGetSearchNote);
        public const string ProcGetApprovedNote = nameof(OraStoredProcedureNames.ProcGetApprovedNote);
        public const string ProcFetchApprover = nameof(OraStoredProcedureNames.ProcFetchApprover);
        public const string ProcFetchCategory = nameof(OraStoredProcedureNames.ProcFetchCategory);
        public const string ProcFetchConfiguration = nameof(OraStoredProcedureNames.ProcFetchConfiguration);
        public const string ProcFetchRecoverPassword = nameof(OraStoredProcedureNames.ProcFetchRecoverPassword);
        public const string ProcVerifyRecoverPasswordUser = nameof(OraStoredProcedureNames.ProcVerifyRecoverPasswordUser);
        public const string ProcCheckPredefinedPassword = nameof(OraStoredProcedureNames.ProcCheckPredefinedPassword);
        public const string ProcGetRowCounts = nameof(OraStoredProcedureNames.ProcGetRowCounts);
        public const string ProcGetNoteWithCategory = nameof(OraStoredProcedureNames.ProcGetNoteWithCategory);
        public const string ProcGetNoteWithApproval = nameof(OraStoredProcedureNames.ProcGetNoteWithApproval);
        public const string ProcGetNote_Counts_Category = nameof(OraStoredProcedureNames.ProcGetNote_Counts_Category);
        public const string ProcGetNotificationByNote = nameof(OraStoredProcedureNames.ProcGetNotificationByNote);
        public const string ProcNotificationTable = nameof(OraStoredProcedureNames.ProcNotificationTable);
        public const string ProcGetFYI = nameof(OraStoredProcedureNames.ProcGetFYI);
        public const string ProcGetPending = nameof(OraStoredProcedureNames.ProcGetPending);
        public const string ProcGetSendBack = nameof(OraStoredProcedureNames.ProcGetSendBack);
        public const string ProcGetDashboardData = nameof(OraStoredProcedureNames.ProcGetDashboardData);
        public const string ProcGetNotificationByUser = nameof(OraStoredProcedureNames.ProcGetNotificationByUser);
        public const string ProcFetchExpenseIncurredAt = nameof(OraStoredProcedureNames.ProcFetchExpenseIncurredAt);
        public const string ProcFetchNatureOfExpenses = nameof(OraStoredProcedureNames.ProcFetchNatureOfExpenses);
        public const string ProcFetchUserData = nameof(OraStoredProcedureNames.ProcFetchUserData);
        public const string ProcFetchSaveNoteData = nameof(OraStoredProcedureNames.ProcFetchSaveNoteData);
        public const string ProcFetchNonFinancialApprover = nameof(OraStoredProcedureNames.ProcFetchNonFinancialApprover);
        public const string ProcFetchFinancialApprover = nameof(OraStoredProcedureNames.ProcFetchFinancialApprover);
        public const string ProcFetchTemplate = nameof(OraStoredProcedureNames.ProcFetchTemplate);
        public const string ProcFetchTemplateByTemplateId = nameof(OraStoredProcedureNames.ProcFetchTemplateByTemplateId);
        public const string ProcFetchDraftList = nameof(OraStoredProcedureNames.ProcFetchDraftList);
        public const string DeleteDraftNote = nameof(OraStoredProcedureNames.DeleteDraftNote);
        public const string FetchUserDataAsPerUserId = nameof(OraStoredProcedureNames.FetchUserDataAsPerUserId);
        public const string ProcFetchPendingNoteData = nameof(OraStoredProcedureNames.ProcFetchPendingNoteData);
        public const string FetchNoteStatus = nameof(OraStoredProcedureNames.FetchNoteStatus);
        public const string ProcFetchUserAsPerEmailOrEmpId = nameof(OraStoredProcedureNames.ProcFetchUserAsPerEmailOrEmpId);
        public const string ProcFetchTopApprover = nameof(OraStoredProcedureNames.ProcFetchTopApprover);
        public const string ProcFetchNoteApproversAndCreator = nameof(OraStoredProcedureNames.ProcFetchNoteApproversAndCreator);
        public const string ProcFetchUserAndApproverForDelegate = nameof(OraStoredProcedureNames.ProcFetchUserAndApproverForDelegate);
        public const string ProcFetchDataForFYIMailSend = nameof(OraStoredProcedureNames.ProcFetchDataForFYIMailSend);
        public const string ProcFetchMailConfiguration = nameof(OraStoredProcedureNames.ProcFetchMailConfiguration);
        public const string ProcFetchCreatorApproverNote = nameof(OraStoredProcedureNames.ProcFetchCreatorApproverNote);
        public const string ProcFetchForDelegateMail = nameof(OraStoredProcedureNames.ProcFetchForDelegateMail);
        public const string ProcFetchApproverForMailSend = nameof(OraStoredProcedureNames.ProcFetchApproverForMailSend);
        public const string ProcFetchApproverAndCreator = nameof(OraStoredProcedureNames.ProcFetchApproverAndCreator);
        public const string ProcFetchApproverByNoteId = nameof(OraStoredProcedureNames.ProcFetchApproverByNoteId);
        public const string ProcGetWithdrawList = nameof(OraStoredProcedureNames.ProcGetWithdrawList);
        public const string ProcSaveToDraftFromWithdraw = nameof(OraStoredProcedureNames.ProcSaveToDraftFromWithdraw);
        public const string ProcFetchCreatorApproverNoteForQuery = nameof(OraStoredProcedureNames.ProcFetchCreatorApproverNoteForQuery);
        public const string ProcFetchFyiNoteData = nameof(OraStoredProcedureNames.ProcFetchFyiNoteData);
        public const string ProcFetchApprovalRequestNoteData = nameof(OraStoredProcedureNames.ProcFetchApprovalRequestNoteData);
        public const string ProcFetchUserTracking = nameof(OraStoredProcedureNames.ProcFetchUserTracking);
        public const string ProcDeleteUserTracking = nameof(OraStoredProcedureNames.ProcDeleteUserTracking);
        public const string ProcFetchApproverListForDelegate = nameof(OraStoredProcedureNames.ProcFetchApproverListForDelegate);
        public const string ProcApproverDashboard = nameof(OraStoredProcedureNames.ProcApproverDashboard);

        public const string ProcSearchApproverNote = nameof(OraStoredProcedureNames.ProcSearchApproverNote);
        public const string ProcCreatorDashboard = nameof(OraStoredProcedureNames.ProcCreatorDashboard);

        public const string ProcFetchRecomendedApprover = nameof(OraStoredProcedureNames.ProcFetchRecomendedApprover);
        public const string ProcFetchRecomendedApproverByNoteId = nameof(OraStoredProcedureNames.ProcFetchRecomendedApproverByNoteId);
        public const string ProcDeleteLastApprover = nameof(OraStoredProcedureNames.ProcDeleteLastApprover);
        public const string ProcFetchViewsNoteData = nameof(OraStoredProcedureNames.ProcFetchViewsNoteData);
        public const string ProcFetchWithdrawNoteDetails = nameof(OraStoredProcedureNames.ProcFetchWithdrawNoteDetails);
        public const string ProcFetchDelegateNoteData = nameof(OraStoredProcedureNames.ProcFetchDelegateNoteData);

        public const string FetchApproverListByNoteId = nameof(OraStoredProcedureNames.FetchApproverListByNoteId);
        public const string DeleteDuplicateApprover = nameof(OraStoredProcedureNames.DeleteDuplicateApprover);
        public const string getAttachmentDetails = nameof(OraStoredProcedureNames.getAttachmentDetails);

        public const string FetchAttachmentByNoteId = nameof(OraStoredProcedureNames.FetchAttachmentByNoteId);
        public const string ProcUserTracking = nameof(OraStoredProcedureNames.ProcUserTracking);
        public const string ProcCheckUserExists = nameof(OraStoredProcedureNames.ProcCheckUserExists);

        public const string FetchNoteDetailsByNoteId = nameof(OraStoredProcedureNames.FetchNoteDetailsByNoteId);
        public const string ProcFetchDelegateByCreator = nameof(OraStoredProcedureNames.ProcFetchDelegateByCreator);
        public const string FetchDetailsForFyiByCreator = nameof(OraStoredProcedureNames.FetchDetailsForFyiByCreator);
        public const string ProcFetchForSkippByCreator = nameof(OraStoredProcedureNames.ProcFetchForSkippByCreator);
        public const string ProcUserDataByUserId = nameof(OraStoredProcedureNames.ProcUserDataByUserId);

        public const string FetchAmendmentDetails=nameof(OraStoredProcedureNames.FetchAmendmentDetails);
        public const string UpdateNoteVersion = nameof(OraStoredProcedureNames.UpdateNoteVersion);
        public const string InsertNoteApprovedAfterNoteApproved = nameof(OraStoredProcedureNames.InsertNoteApprovedAfterNoteApproved);
        public const string RevartNoteStatus=nameof(OraStoredProcedureNames.RevartNoteStatus);
        public const string ProcFetchAmendmentData = nameof(OraStoredProcedureNames.ProcFetchAmendmentData);
        public const string ProcMyApprovedNoteData = nameof(OraStoredProcedureNames.ProcMyApprovedNoteData);
        public const string ResetBeforeAmendment= nameof(OraStoredProcedureNames.ResetBeforeAmendment);
        
        public const string ProcFetchReviewerOrApprover = nameof(OraStoredProcedureNames.ProcFetchReviewerOrApprover);

        public const string FetchNoteVersion=nameof(OraStoredProcedureNames.FetchNoteVersion);
        public const string ProcFetchNoteVersionListByNoteId = nameof(OraStoredProcedureNames.ProcFetchNoteVersionListByNoteId);
        public const string ProcFetchCurrentNoteVersion = nameof(OraStoredProcedureNames.ProcFetchCurrentNoteVersion);
        public const string ProcFetchPreviousNoteVersion = nameof(OraStoredProcedureNames.ProcFetchPreviousNoteVersion);
        public const string ProcFetchChildNoteVersion = nameof(OraStoredProcedureNames.ProcFetchChildNoteVersion);

        public const string DeleteAttachmentByNoteId = nameof(OraStoredProcedureNames.DeleteAttachmentByNoteId);
        public const string UpdateCurrentApproverAsigntime = nameof(OraStoredProcedureNames.UpdateCurrentApproverAsigntime);
        public const string ProcApprovedNoteData = nameof(OraStoredProcedureNames.ProcApprovedNoteData);
        public const string Proc_SaveUserTracking = nameof(OraStoredProcedureNames.Proc_SaveUserTracking);
        public const string FetchCreaterForCCMail=nameof(OraStoredProcedureNames.FetchCreaterForCCMail);

    }
}
