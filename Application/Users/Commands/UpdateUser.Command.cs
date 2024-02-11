using Application.Common.Interfaces;
using Common.Exceptions;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users.Commands
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResponse>
    {
        private readonly IBrokerDbContext _dbContext;
        private readonly IIdentityService _identityService;
        public UpdateUserHandler(IBrokerDbContext dbContext, IIdentityService identityService)
        {
            _dbContext = dbContext;
            _identityService = identityService;
        }

        public async Task<UpdateUserResponse> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            var dbUserProfile = await _dbContext.UserProfiles.Include(i => i.User)
                                                             .ThenInclude(i=> i.UserRoles)
                                                             .FirstOrDefaultAsync(fd => fd.Id == request.Id &&
                                                                                        fd.BrokerId == fd.BrokerId &&
                                                                                        fd.HasSystemAccess == true, cancellationToken);
            if (dbUserProfile == null)
            {
                throw new NotFoundException("Invalid user id.");
            }

            var dbBroker = await _dbContext.Brokers.FindAsync(request.BrokerId);
            if (dbBroker != null)
            {
                #region Role
                if (request.Role != null)
                {
                    var dbRolesToRemove = dbUserProfile.User.UserRoles.Where(w => !request.Role.Contains(w.Role.Name));
                    foreach (var dbRoleToRemove in dbRolesToRemove)
                    {
                        dbUserProfile.User.UserRoles.Remove(dbRoleToRemove);
                    }
                    var rolesToAdd = request.Role.Where(w => !dbUserProfile.User.UserRoles.Select(s => s.Role?.Name).Contains(w));
                    foreach (var roleToAdd in rolesToAdd)
                    {
                        var dbRole = await _identityService.GetRoleByNameAsync(roleToAdd);
                        dbUserProfile.User.UserRoles.Add(new UserRole
                        {
                            UserId = dbUserProfile.User.Id,
                            RoleId = dbRole.Id
                        });
                    }
                }
                else if (dbUserProfile.User.UserRoles.Count > 0)
                {
                    dbUserProfile.User.UserRoles.Clear();
                }
                #endregion
            }

            dbUserProfile.FirstName = request.FirstName;
            dbUserProfile.LastName = request.LastName;
            dbUserProfile.Email = request.Email;
            dbUserProfile.PhoneNumber = request.PhoneNumber;
            dbUserProfile.LastUpdateDateTime = DateTimeOffset.UtcNow;
            dbUserProfile.LastUpdatedBy = request.CurrentUser;

            dbUserProfile.User.PhoneNumber = request.PhoneNumber;
            dbUserProfile.User.Email = request.Email;
            dbUserProfile.User.LastUpdateDateTime = DateTimeOffset.UtcNow;
            dbUserProfile.User.LastUpdatedBy = request.CurrentUser;

            _dbContext.UserProfiles.Update(dbUserProfile);
            await _dbContext.SaveChangesAsync(cancellationToken);

            return new UpdateUserResponse();
        }
    }

    public class UpdateUserCommand : IRequest<UpdateUserResponse>
    {
        [JsonIgnore]
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        [JsonIgnore]
        public long? BrokerId { get; set; }
        [JsonIgnore]
        public string CurrentUser { get; set; }
        public string[] Role { get; set; }
    }

    public class UpdateUserResponse { }
}
