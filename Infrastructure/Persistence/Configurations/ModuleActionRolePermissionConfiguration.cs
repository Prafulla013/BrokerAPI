using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Infrastructure.Persistence.Configurations
{
    internal class ModuleActionRolePermissionConfiguration : BaseConfiguration<ModuleActionRolePermission>
    {
        public override void Configure(EntityTypeBuilder<ModuleActionRolePermission> builder)
        {
            base.Configure(builder);

            builder.HasKey(h => h.Id);
            builder.Property(p => p.Id)
                .UseIdentityColumn(1, 1)
                .ValueGeneratedOnAdd();

            builder.HasOne(h => h.Role)
                .WithMany()
                .HasForeignKey(h => h.RoleId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasIndex(h => new { h.RoleId, h.UserType, h.ModuleGroup,h.ModuleAction })
                .IsUnique();
        }
    }
}
