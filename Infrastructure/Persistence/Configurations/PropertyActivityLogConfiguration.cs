using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class PropertyActivityLogConfiguration : BaseConfiguration<PropertyActivityLog>
    {
        public override void Configure(EntityTypeBuilder<PropertyActivityLog> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.Id);
            builder.Property(p => p.Id)
                .UseIdentityColumn(1, 1)
                .ValueGeneratedOnAdd();

            builder.HasOne(h => h.Broker)
                 .WithMany(w => w.PropertyActivityLogs)
                 .HasForeignKey(h => h.BrokerId)
                 .OnDelete(DeleteBehavior.Cascade);

            builder.Property(p => p.Comment)
                .IsRequired();

            builder.Property(p => p.Params)
                .IsRequired();
        }
    }
}
