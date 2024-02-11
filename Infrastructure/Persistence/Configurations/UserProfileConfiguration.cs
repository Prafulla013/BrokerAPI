using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public class UserProfileConfiguration : BaseConfiguration<UserProfile>
    {
        public override void Configure(EntityTypeBuilder<UserProfile> builder)
        {
            base.Configure(builder);

            builder.HasKey(h => h.Id);
            builder.Property(p => p.Id)
                .HasMaxLength(50)
                .IsRequired();

            builder.HasOne(h => h.Broker)
                .WithMany()
                .HasForeignKey(h => h.BrokerId)
                .OnDelete(DeleteBehavior.Cascade)
                .IsRequired(false);

            builder.Property(p => p.FirstName)
                .HasMaxLength(300)
                .IsRequired();

            builder.Property(p => p.LastName)
                .HasMaxLength(300)
                .IsRequired();

            builder.HasIndex(h => h.Email)
                .IsUnique();
            builder.Property(p => p.Email)
                .HasMaxLength(100)
                .IsRequired(false);

            builder.Property(p => p.PhoneNumber)
                .HasMaxLength(10)
                .IsRequired(false);

            builder.Property(p => p.Street)
                .HasMaxLength(300)
                .IsRequired(false);

            builder.Property(p => p.City)
               .HasMaxLength(100)
               .IsRequired(false);

            builder.Property(p => p.State)
               .HasMaxLength(100)
               .IsRequired(false);

            builder.Property(p => p.ZipCode)
               .HasMaxLength(10)
               .IsRequired(false);

            builder.Property(p => p.CreatedAt)
                .IsRequired();
        }
    }
}
