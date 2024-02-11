using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
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
