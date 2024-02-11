using System.Collections.Generic;

namespace Application.Common.Models
{
    public class EmailModel
    {
        public string From { get; set; }
        public string To { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public List<string> Ccs { get; set; }
    }
}
