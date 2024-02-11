using Common.Enumerations;
using Common.Interfaces;

namespace Domain.Entities
{
    public class PropertyManagement : BaseEntity, ICreatedEvent, IUpdatedEvent, IDeletedEvent
    {
        public long Id { get; set; }
        public PropertyType PropertyType { get; set; }
        public string Location { get;set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public long BrokerId { get; set; }
        public virtual Broker Broker { get; set; }
    }
}
