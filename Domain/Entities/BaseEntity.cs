using System;

namespace Domain.Entities
{
    public class BaseEntity
    {
        public bool IsActive { get; set; }
        public DateTimeOffset LastUpdateDateTime { get; set; } = DateTimeOffset.UtcNow;
        public string LastUpdatedBy { get; set; } = "SA";
    }
}
