using Common.Enumerations;
using System.Net;

namespace Application.Common.Models
{
    public class UserActivityModel
    {
        public string UserId { get; set; }
        public string Url { get; set; }
        public string Comment { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public ModuleAction ModuleAction { get; set; }
        public ModuleGroup ModuleGroup { get; set; }
    }
}
