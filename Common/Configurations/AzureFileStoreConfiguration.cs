using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.Configurations
{
    public class AzureFileStoreConfiguration
    {
        public const string SECTION_NAME = "AzureFileStore";
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
    }
}
