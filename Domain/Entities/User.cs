using Common.Enumerations;
using Common.Interfaces;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities
{
    public class User : IdentityUser, ICreatedEvent, IActivatedEvent
    {
        public DateTimeOffset LastAccessedAt { get; set; } = DateTimeOffset.UtcNow;
        public bool IsActive { get; set; }
        public DateTimeOffset LastUpdateDateTime { get; set; } = DateTimeOffset.UtcNow;
        public string LastUpdatedBy { get; set; } = "SA";

        public virtual UserProfile Profile { get; set; }
        public virtual ICollection<UserRole> UserRoles { get; set; }

        [NotMapped]
        public string ClientUrl { get; set; }
        [NotMapped]
        public ActivityLog Activity { get; set; }
    }
}
