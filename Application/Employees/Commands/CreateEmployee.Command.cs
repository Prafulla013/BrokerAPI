using Application.Common.Interfaces;
using Common.Enumerations;
using Domain.Entities;
using MediatR;
using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Employees.Commands
{
    public class CreateEmployeeHandler : IRequestHandler<CreateEmployeeCommand, CreateEmployeeResponse>
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IStringHelper _stringHelper;
        private readonly IAzureFileService _azureFileService;
        public CreateEmployeeHandler(IBrokerDbContext dbContext,
                                     IIdentityService identityService,
                                     IStringHelper stringHelper,
                                     IAzureFileService azureFileService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
            _stringHelper = stringHelper;
            _azureFileService = azureFileService;
        }

        public async Task<CreateEmployeeResponse> Handle(CreateEmployeeCommand request, CancellationToken cancellationToken)
        {
            // Both employee and broker users are persisted in same table. All employees of brokers will be assigned with Employee role.
            var dbProfile = new UserProfile
            {
                Id = Guid.NewGuid().ToString(),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                State = request.State,
                Street = request.Street,
                City = request.City,
                ZipCode = request.ZipCode,
                BrokerId = request.BrokerId,
                HasSystemAccess = request.HasSystemAccess,
                Type = request.UserType,
                IsActive = true,
                LastUpdatedBy = request.CurrentUser
            };


            if (request.HasSystemAccess)
            {
                var dbRole = await _identityService.GetRoleByNameAsync("Broker Employee");
                var dbUser = new User
                {
                    Id = dbProfile.Id,
                    UserName = request.Username,
                    Email = request.Email,
                    PhoneNumber = request.PhoneNumber,
                    IsActive = false,
                    Profile = dbProfile,
                    ClientUrl = _stringHelper.ToClientUrl(request.Subdomain),
                    Activity = ActivityLog.Created,
                    LastUpdatedBy = request.CurrentUser
                };

                await _identityService.CreateAsync(dbUser, dbRole.Id);
            }
            else
            {
                _dbContext.UserProfiles.Add(dbProfile);
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            var response = new CreateEmployeeResponse
            {
                Id = dbProfile.Id
            };

            return response;
        }
    }

    public class CreateEmployeeCommand : IRequest<CreateEmployeeResponse>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string Street { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public string ZipCode { get; set; }
        public bool HasSystemAccess { get; set; }
        public UserType UserType { get; set; } = UserType.Employee;
        [JsonIgnore]
        public long BrokerId { get; set; }
        [JsonIgnore]
        public string CurrentUser { get; set; }
        [JsonIgnore]
        public string Subdomain { get; set; }
    }

    public class CreateEmployeeResponse
    {
        public string Id { get; set; }
    }
}
