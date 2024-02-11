using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Threading.Tasks;
using System.Threading;

namespace Application.Common.Interfaces
{
    public interface IBrokerDbContext
    {
        DbSet<ModuleActionRolePermission> ModuleActionRolePermissions { get; set; }
        DbSet<UserActivityLog> UserActivityLogs { get; set; }
        DbSet<UserProfile> UserProfiles { get; set; }
        DbSet<Broker> Brokers { get; set; }
        DbSet<PropertyManagement> PropertyManagements { get; set; }
        DbSet<PropertyActivityLog> PropertyActivityLogs { get; set; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken);
        EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;
        ChangeTracker ChangeTracker { get; }
    }
}
