using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UserConfiguration : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.HasOne(h => h.Profile)
                .WithOne(w => w.User)
                .HasForeignKey<User>(h => h.Id)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(h => h.Email)
                .IsUnique();
            builder.Property(p => p.Email)
                .HasMaxLength(100)
                .IsRequired();

            builder.HasIndex(h => new { h.UserName })
                .IsUnique();
            builder.Property(p => p.UserName)
                .HasMaxLength(100)
                .IsRequired();

            builder.Property(p => p.IsActive)
                .IsRequired();

            builder.Property(p => p.LastUpdateDateTime)
                 .IsRequired();

            builder.Property(p => p.LastUpdatedBy)
                .HasMaxLength(300)
                .IsRequired();
        }
    }
}
