namespace Common.Configurations
{
    public class EmailConfiguration
    {
        public const string SECTION_NAME = "Email";
        public string Key { get; set; }
        public string HostEmail { get; set; }

        public const string ACTIVATION_TEMPLATE = "ActivationTemplate.html";
        public const string CONFIRMATION_TEMPLATE = "ConfirmationTemplate.html";
        public const string REQUEST_RESET_PASSWORD_TEMPLATE = "RequestResetPasswordTemplate.html";

    }
}
