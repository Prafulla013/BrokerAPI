namespace Common.Configurations
{
    public class CorsConfiguration
    {
        public const string SECTION_NAME = "CORS";
        public string[] ExposedHeaders { get; set; }
        public string[] Origins { get; set; }
    }
}
