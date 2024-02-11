using Application.Common.Interfaces;
using Common.Enumerations;
using Domain.Entities;
using MediatR;
using System;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Property.Commands
{
    public class CreateProperyManagementHandler : IRequestHandler<CreateProperyManagementCommand, CreateProperyManagementResponse>
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IStringHelper _stringHelper;
        private readonly IAzureFileService _azureFileService;
        public CreateProperyManagementHandler(IBrokerDbContext dbContext,
                                     IIdentityService identityService,
                                     IStringHelper stringHelper,
                                     IAzureFileService azureFileService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
            _stringHelper = stringHelper;
            _azureFileService = azureFileService;
        }

        public async Task<CreateProperyManagementResponse> Handle(CreateProperyManagementCommand request, CancellationToken cancellationToken)
        {
            var dbProperty = new PropertyManagement
            {
                PropertyType = request.PropertyType,
                Location = request.Location,
                Price = request.Price,
                Description = request.Description,
                IsActive = true,
                LastUpdatedBy = request.CurrentUser
            };
            _dbContext.PropertyManagements.Add(dbProperty);
            await _dbContext.SaveChangesAsync(cancellationToken);
            return new CreateProperyManagementResponse { };
        }
    }

    public class CreateProperyManagementCommand : IRequest<CreateProperyManagementResponse>
    {
        public PropertyType PropertyType { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        [JsonIgnore]
        public long BrokerId { get; set; }
        [JsonIgnore]
        public string CurrentUser { get; set; }
    }

    public class CreateProperyManagementResponse
    {
    }
}
