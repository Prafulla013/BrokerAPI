using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class BrokerConfiguration : BaseConfiguration<Broker>
    {
        public override void Configure(EntityTypeBuilder<Broker> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.Id);
            builder.Property(p => p.Id)
                .UseIdentityColumn(1, 1)
                .ValueGeneratedOnAdd();
        }
    }
}
