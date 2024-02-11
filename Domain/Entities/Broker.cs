using System.Collections.Generic;

namespace Domain.Entities
{
    public class Broker : BaseEntity
    {
        public long Id { get; set; }
        public string BrokerName { get; set; }
        public string BrokerImage { get; set; }
        public virtual ICollection<PropertyActivityLog> PropertyActivityLogs { get; set; }

    }
}
