using Application.Common.Interfaces;
using Common.Enumerations;
using Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users.Commands
{
    public class DeleteUserHandler : IRequestHandler<DeleteUserCommand>
    {
        private readonly IBrokerDbContext _dbContext;
        public DeleteUserHandler(IBrokerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var admins = new UserType[] { UserType.RootUser, UserType.Admin };
            var dbProfile = await _dbContext.UserProfiles.Include(i => i.User)
                                                         .FirstOrDefaultAsync(fd => fd.Id == request.Id &&
                                                                              fd.BrokerId == request.BrokerId &&
                                                                              admins.Contains(fd.Type), cancellationToken);
            if (dbProfile == null)
            {
                throw new NotFoundException("Invalid user id.");
            }

            _dbContext.UserProfiles.Remove(dbProfile);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    public class DeleteUserCommand : IRequest
    {
        public string Id { get; set; }
        public long? BrokerId { get; set; }
    }
}
