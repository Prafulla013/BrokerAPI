namespace Common.Configurations
{
    public class TwilioConfiguration
    {
        public const string SECTION_NAME = "Twilio";
        public string AccountId { get; set; }
        public string Token { get; set; }
        public string PhoneNumber { get; set; }
        public string ReceiverPhoneNumber { get; set; }
        public bool IsTestMode { get; set; }
    }
}
