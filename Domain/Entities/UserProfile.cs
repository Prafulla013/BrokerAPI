using Common.Enumerations;
using System;
using System.Collections.Generic;

namespace Domain.Entities
{
    public class UserProfile : BaseEntity
    {
        public string Id { get; set; }
        public long? BrokerId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string ZipCode { get; set; }
        public UserType Type { get; set; }
        public bool HasSystemAccess { get; set; }
        public DateTimeOffset CreatedAt { get; set; } = DateTimeOffset.UtcNow;

        public virtual Broker Broker { get; set; }
        public virtual User User { get; set; }
    }
}
