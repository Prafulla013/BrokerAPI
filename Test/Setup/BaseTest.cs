using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Common.Configurations;
using Common.Interfaces;
using Domain.Entities;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure.Services;
using Microsoft.Extensions.Caching.Memory;

namespace Test.Setup
{
    public abstract class BaseTest
    {
        protected static int _dbCount = 1;

        protected DbContextOptionsBuilder<BrokerDbContext> _dbContextBuilder;
        protected UserManager<User> _userManager;
        protected RoleManager<Role> _roleManager;

        protected IBrokerDbContext _dbContext;
        protected IContextService _contextService;

        protected IUserStore<User> _userStore;
        protected IUserRoleStore<User> _userRoleStore;
        protected IRoleStore<Role> _roleStore;

        protected Mock<IEventDispatcherService> _mockEventDispatcherService;

        protected IOptions<JwtConfiguration> _jwtOptions;
        protected IOptions<ApplicationConfiguration> _appOptions;

        protected IMemoryCacheService _memoryCacheService;

        public BaseTest(string uniqueDbName)
        {
            _jwtOptions = Options.Create(new JwtConfiguration
            {
                Audience = "https://localhost:3001",
                ExpireInMinutes = 60,
                Issuer = "https://localhost:3001",
                Key = "any-random-value-can-be-provided-since-this-is-for-test-no-need-to-replace-it"
            });

            _appOptions = Options.Create(new ApplicationConfiguration
            {
                Protocol = "http://",
                ClientUrl = "localhost:3001",
                MasterEmail = $"test-mail-{uniqueDbName}@testdomain.com"
            });

            _mockEventDispatcherService = new Mock<IEventDispatcherService>();

            _dbContextBuilder = new DbContextOptionsBuilder<BrokerDbContext>()
                                   .UseInMemoryDatabase(uniqueDbName);

            _roleStore = new RoleStore<Role>(new BrokerDbContext(_dbContextBuilder.Options, _mockEventDispatcherService.Object, _appOptions));
            // Must declare UserRole (IdentityUserRole<string>) in UserStore
            _userRoleStore = new UserStore<User,
                                           Role,
                                           BrokerDbContext,
                                           string,
                                           IdentityUserClaim<string>,
                                           UserRole,
                                           IdentityUserLogin<string>,
                                           IdentityUserToken<string>,
                                           IdentityRoleClaim<string>>
                                           (new BrokerDbContext(_dbContextBuilder.Options, _mockEventDispatcherService.Object, _appOptions));

            _dbContext = new BrokerDbContext(_dbContextBuilder.Options, _mockEventDispatcherService.Object, _appOptions);

            _userManager = SetupUserManager(_userRoleStore);
            _roleManager = SetupRoleManager(_roleStore);

            SeedDb();

            _contextService = new ContextService(uniqueDbName);
            var memoryCache = new Mock<IMemoryCache>();
            _memoryCacheService = new MemoryCacheService(memoryCache.Object);
        }

        public void SeedDb()
        {
            using var dbContext = new BrokerDbContext(_dbContextBuilder.Options, _mockEventDispatcherService.Object, _appOptions);
            dbContext.Database.EnsureDeleted();
            dbContext.Database.EnsureCreated();

            //SetupRole.SeedRole(_roleManager).Wait();
            SetupUser.SeedUser(_userManager).Wait();

            dbContext.SaveChanges();
        }

        private UserManager<TUser> SetupUserManager<TUser>(IUserRoleStore<TUser> store = null) where TUser : class
        {
            store ??= new Mock<IUserRoleStore<TUser>>().Object;

            var identityOptions = new IdentityOptions();
            identityOptions.Lockout.AllowedForNewUsers = false;
            identityOptions.Password.RequireDigit = true;
            identityOptions.Password.RequireLowercase = true;
            identityOptions.Password.RequireNonAlphanumeric = true;
            identityOptions.Password.RequireUppercase = true;
            identityOptions.Password.RequiredLength = 8;
            identityOptions.Password.RequiredUniqueChars = 1;
            identityOptions.Tokens.EmailConfirmationTokenProvider = "DataProtectorTokenProvider";
            identityOptions.Tokens.PasswordResetTokenProvider = "DataProtectorTokenProvider";

            var options = new Mock<IOptions<IdentityOptions>>();
            options.Setup(s => s.Value).Returns(identityOptions);

            var mockValidator = new Mock<IUserValidator<TUser>>();
            var userValidators = new List<IUserValidator<TUser>>
            {
                mockValidator.Object
            };

            var passwordValidators = new List<PasswordValidator<TUser>>
            {
                new PasswordValidator<TUser>()
            };

            var mockLogger = new Mock<ILogger<UserManager<TUser>>>();

            var userManager = new UserManager<TUser>(store,
                                                     options.Object,
                                                     new PasswordHasher<TUser>(),
                                                     userValidators,
                                                     passwordValidators,
                                                     new UpperInvariantLookupNormalizer(),
                                                     new IdentityErrorDescriber(),
                                                     null,
                                                     mockLogger.Object);

            // This section is not used yet, may be useful in future enhancement.
            var mockDataProtectionLogger = new Mock<ILogger<DataProtectorTokenProvider<TUser>>>();
            var dataProtectionProvider = DataProtectionProvider.Create(nameof(BaseTest));
            IUserTwoFactorTokenProvider<TUser> twoFactorTokenProvider = new DataProtectorTokenProvider<TUser>(dataProtectionProvider, null, mockDataProtectionLogger.Object);
            userManager.RegisterTokenProvider("DataProtectorTokenProvider", twoFactorTokenProvider);

            mockValidator.Setup(s => s.ValidateAsync(userManager, It.IsAny<TUser>())).Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            return userManager;
        }

        private RoleManager<TRole> SetupRoleManager<TRole>(IRoleStore<TRole> store = null) where TRole : class
        {
            store ??= new Mock<IRoleStore<TRole>>().Object;

            var mockValidator = new Mock<IRoleValidator<TRole>>();
            var roleValidators = new List<IRoleValidator<TRole>>
            {
                mockValidator.Object
            };

            var mockLogger = new Mock<ILogger<RoleManager<TRole>>>();

            var roleManager = new RoleManager<TRole>(store,
                                                     roleValidators,
                                                     new UpperInvariantLookupNormalizer(),
                                                     new IdentityErrorDescriber(),
                                                     mockLogger.Object);

            mockValidator.Setup(s => s.ValidateAsync(roleManager, It.IsAny<TRole>())).Returns(Task.FromResult(IdentityResult.Success)).Verifiable();

            return roleManager;
        }
    }
}
