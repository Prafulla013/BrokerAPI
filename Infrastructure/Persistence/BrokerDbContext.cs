using Application.Common.Interfaces;
using Common.Configurations;
using Common.Interfaces;
using Domain.Entities;
using Domain.Events;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.Extensions.Options;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence
{
    public class BrokerDbContext : IdentityDbContext<User,
                                                     Role,
                                                     string,
                                                     IdentityUserClaim<string>,
                                                     UserRole,
                                                     IdentityUserLogin<string>,
                                                     IdentityRoleClaim<string>,
                                                     IdentityUserToken<string>>,
                                    IBrokerDbContext
    {
        private readonly IEventDispatcherService _eventDispatcherService;
        private readonly ApplicationConfiguration _applicationConfiguration;
        public BrokerDbContext(DbContextOptions<BrokerDbContext> options,
                               IEventDispatcherService eventDispatcherService,
                               IOptions<ApplicationConfiguration> appOptions) : base(options)
        {
            _eventDispatcherService = eventDispatcherService;
            _applicationConfiguration = appOptions.Value;
        }

        public BrokerDbContext(DbContextOptions<BrokerDbContext> options) : base(options) { }

        // Arrange all dbsets alphabetically
        public DbSet<ModuleActionRolePermission> ModuleActionRolePermissions { get; set; }
        public DbSet<PropertyActivityLog> PropertyActivityLogs { get; set; }
        public DbSet<UserActivityLog> UserActivityLogs { get; set; }
        public DbSet<UserProfile> UserProfiles { get; set; }
        public DbSet<Broker> Brokers { get; set; }
        public DbSet<PropertyManagement> PropertyManagements { get; set; }

        public override ChangeTracker ChangeTracker => base.ChangeTracker;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            // always run OnModelCreating before running custom configuration to avoid overwrite of custom navigation on identity user
            base.OnModelCreating(builder);
            builder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());

            if (_applicationConfiguration != null)
            {
                // Use this only at the end of the devlopment cycle.
                var dbInitializer = new DbInitializer(builder, _applicationConfiguration);
                // Initial data seed for User and Roles.
                dbInitializer.SeedUsersAndRoles();
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                QueueDomainEvents();
                var result = await base.SaveChangesAsync(cancellationToken);
                return result;
            }
            catch
            {
                if (_eventDispatcherService != null)
                {
                    _eventDispatcherService.ClearQueue();
                }
                throw;
            }
        }

        private void QueueDomainEvents()
        {
            var addedEntities = ChangeTracker.Entries<ICreatedEvent>().Where(w => w.State == EntityState.Added);
            foreach (var addedEntity in addedEntities)
            {
                var entity = new CreatedEvent(addedEntity.Entity);
                _eventDispatcherService.QueueNotification(entity);
            }

            var activatedEntities = ChangeTracker.Entries<IActivatedEvent>().Where(w => w.State == EntityState.Modified);
            foreach (var activatedEntitiy in activatedEntities)
            {
                var entity = new ActivatedEvent(activatedEntitiy.Entity);
                _eventDispatcherService.QueueNotification(entity);
            }

            var updatedEntities = ChangeTracker.Entries<IUpdatedEvent>().Where(w => w.State == EntityState.Modified);
            foreach (var updatedEntitiy in updatedEntities)
            {
                var entity = new UpdatedEvent(updatedEntitiy.Entity);
                _eventDispatcherService.QueueNotification(entity);
            }

            var deletedEntities = ChangeTracker.Entries<IDeletedEvent>().Where(w => w.State == EntityState.Deleted);
            foreach (var deletedEntitiy in deletedEntities)
            {
                var entity = new DeletedEvent(deletedEntitiy.Entity);
                _eventDispatcherService.QueueNotification(entity);
            }
        }
    }
}
