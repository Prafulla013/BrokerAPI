using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UserActivityLogConfiguration : BaseConfiguration<UserActivityLog>
    {
        public override void Configure(EntityTypeBuilder<UserActivityLog> builder)
        {
            base.Configure(builder);

            builder.HasKey(x => x.Id);
            builder.Property(p => p.Id)
                .UseIdentityColumn(1, 1)
                .ValueGeneratedOnAdd();

            builder.HasOne(h => h.UserProfile)
                 .WithMany()
                 .HasForeignKey(h => h.UserId)
                 .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
