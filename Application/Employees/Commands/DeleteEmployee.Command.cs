using Application.Common.Interfaces;
using Common.Enumerations;
using Common.Exceptions;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Employees.Commands
{
    public class DeleteEmployeeHandler : IRequestHandler<DeleteEmployeeCommand, DeleteEmployeeResponse>
    {
        private readonly IBrokerDbContext _dbContext;
        public DeleteEmployeeHandler(IBrokerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<DeleteEmployeeResponse> Handle(DeleteEmployeeCommand request, CancellationToken cancellationToken)
        {
            var employees = new UserType[] { UserType.Employee };
            var dbProfile = await _dbContext.UserProfiles.FirstOrDefaultAsync(fd => fd.Id == request.Id &&
                                                                                    fd.BrokerId == request.BrokerId &&
                                                                                    fd.Type == UserType.Employee, cancellationToken);
            if (dbProfile == null)
            {
                throw new NotFoundException("Invalid employee id.");
            }

        
            _dbContext.UserProfiles.Remove(dbProfile);

            await _dbContext.SaveChangesAsync(cancellationToken);

            return new DeleteEmployeeResponse();
        }
    }

    public class DeleteEmployeeCommand : IRequest<DeleteEmployeeResponse>
    {
        public string Id { get; set; }
        [JsonIgnore]
        public long BrokerId { get; set; }
    }

    public class DeleteEmployeeResponse { }
}
