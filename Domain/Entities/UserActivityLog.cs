using Common.Enumerations;
using System.Net;

namespace Domain.Entities
{
    public class UserActivityLog : BaseEntity
    {
        public long Id { get; set; }
        public string UserId { get; set; }
        public string Url { get; set; }
        public string Comment { get; set; }
        public HttpStatusCode StatusCode { get; set; }
        public ModuleGroup ModuleGroup { get; set; }
        public ModuleAction ModuleAction { get; set; }
        public virtual UserProfile UserProfile { get; set; }
    }
}
