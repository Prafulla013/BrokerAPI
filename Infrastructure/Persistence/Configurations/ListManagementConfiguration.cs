using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class ListManagementConfiguration : BaseConfiguration<PropertyManagement>
    {
        public override void Configure(EntityTypeBuilder<PropertyManagement> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.Id);
            builder.Property(p => p.Id)
                .UseIdentityColumn(1, 1)
                .ValueGeneratedOnAdd();

            builder.HasOne(h => h.Broker)
                 .WithMany()
                 .HasForeignKey(h => h.BrokerId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
