using Application.Common.Interfaces;
using Common.Configurations;
using Common.Enumerations;
using Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Employees.Commands
{
    public class UpdateEmployeeHandler : IRequestHandler<UpdateEmployeeCommand, UpdateEmployeeResponse>
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        private readonly IStringHelper _stringHelper;
        private readonly IAzureFileService _azureFileService;
        public UpdateEmployeeHandler(IBrokerDbContext dbContext,
                                     IIdentityService identityService,
                                     IStringHelper stringHelper,
                                     IAzureFileService azureFileService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
            _stringHelper = stringHelper;
            _azureFileService = azureFileService;
        }

        public async Task<UpdateEmployeeResponse> Handle(UpdateEmployeeCommand request, CancellationToken cancellationToken)
        {
            var dbProfile = await _dbContext.UserProfiles.Include(i => i.User)
                                                         .FirstOrDefaultAsync(fd => fd.Id == request.Id &&
                                                                                    fd.BrokerId == request.BrokerId, cancellationToken);
            if (dbProfile == null)
            {
                throw new NotFoundException("Invalid employee id.");
            }

           

            dbProfile.FirstName = request.FirstName;
            dbProfile.LastName = request.LastName;
            dbProfile.Email = request.Email;
            dbProfile.PhoneNumber = request.PhoneNumber;
            dbProfile.Street = request.Street;
            dbProfile.State = request.State;
            dbProfile.ZipCode = request.ZipCode;
            dbProfile.City = request.City;
            dbProfile.BrokerId = request.BrokerId;
            dbProfile.Type = request.UserType;
            dbProfile.HasSystemAccess = request.HasSystemAccess;
            dbProfile.IsActive = true;
            dbProfile.LastUpdateDateTime = DateTimeOffset.UtcNow;
            dbProfile.LastUpdatedBy = request.CurrentUser;

            if (request.HasSystemAccess)
            {
                if (dbProfile.User == null)
                {
                    await _dbContext.SaveChangesAsync(cancellationToken);

                    var dbRole = await _identityService.GetRoleByNameAsync("Employee");
                    // Create user account
                    dbProfile.User = new User
                    {
                        Id = dbProfile.Id,
                        UserName = request.Username,
                        Email = request.Email,
                        PhoneNumber = request.PhoneNumber,
                        IsActive = false,
                        ClientUrl = _stringHelper.ToClientUrl(request.Subdomain),
                        Activity = ActivityLog.Created,
                        LastUpdateDateTime = DateTimeOffset.UtcNow,
                        LastUpdatedBy = request.CurrentUser
                    };
                    await _identityService.CreateAsync(dbProfile.User, dbRole.Id);
                }
                else
                {
                    // Update user account
                    dbProfile.User.UserName = request.Username;
                    dbProfile.User.Email = request.Email;
                    dbProfile.User.PhoneNumber = request.PhoneNumber;
                    dbProfile.User.IsActive = true;
                    dbProfile.LastUpdateDateTime = DateTimeOffset.UtcNow;
                    dbProfile.User.LastUpdatedBy = request.CurrentUser;

                    await _dbContext.SaveChangesAsync(cancellationToken);
                }
            }
            else
            {
                await _dbContext.SaveChangesAsync(cancellationToken);
            }

            return new UpdateEmployeeResponse();
        }
    }

    public class UpdateEmployeeCommand : IRequest<UpdateEmployeeResponse>
    {
        [JsonIgnore]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public string City { get; set; }
        public bool HasSystemAccess { get; set; }
        public UserType UserType { get; set; }
        [JsonIgnore]
        public long BrokerId { get; set; }
        [JsonIgnore]
        public string CurrentUser { get; set; }
        [JsonIgnore]
        public string Subdomain { get; set; }
    }

    public class UpdateEmployeeResponse { }
}
