namespace Common.Configurations
{
    public class ApplicationConfiguration
    {
        public const string SECTION_NAME = "Application";
        public string Protocol { get; set; }
        public string ClientUrl { get; set; }
        public string MasterEmail { get; set; }
        public bool IsTestMode { get; set; }
    }
}
