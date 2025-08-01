namespace DNAS.Domain.Common
{
    public static class CommonMsg
    {
        public const string MailnotExists = "Your E-mail ID does not exist.";
        public const string PasswordLinkExpire = "Your password link has invalidated / expired.";
        public const string fillValidMail = "Please fill valid mail id";
        public const string PasswordChanged = "Your password has been changed. Please log in to the portal";
        public const string PreDefinePasswordNotMatch = "The Predefined Password Not Matched. Please try again.";
        public const string RecoveryPasswordTryBeforeTime = "Please try again later as your password has been recently changed.";
        public const string LdapSuccessNotInDnas = "We couldn't find your user profile in the system. Please contact the DNAS Support team to create your user account";
        public const string InvalidLdapCredential = "Invalid Credentials. Please use your AD password and try again";
    }
}
