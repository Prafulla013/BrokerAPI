using Application.Common.Interfaces;
using Common.Enumerations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Employees.Queries
{
    public class ListAllEmployeesSelectOptionsHandler : IRequestHandler<ListAllEmployeesSelectOptionsQuery, List<ListAllEmployeesSelectOptionsResponse>>
    {
        private readonly IBrokerDbContext _dbContext;
        public ListAllEmployeesSelectOptionsHandler(IBrokerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ListAllEmployeesSelectOptionsResponse>> Handle(ListAllEmployeesSelectOptionsQuery request, CancellationToken cancellationToken)
        {
            var response = await _dbContext.UserProfiles.Where(w => w.Type == UserType.Employee &&
                                                                    w.IsActive == true &&
                                                                    w.BrokerId == request.BrokerId)
                                                        .Select(s => new ListAllEmployeesSelectOptionsResponse
                                                        {
                                                            Id = s.Id,
                                                            Name = s.FirstName + " " + s.LastName
                                                        })
                                                        .OrderBy(o => o.Name)
                                                        .ToListAsync(cancellationToken);

            return response;
        }
    }

    public class ListAllEmployeesSelectOptionsQuery : IRequest<List<ListAllEmployeesSelectOptionsResponse>>
    {
        public long BrokerId { get; set; }
    }

    public class ListAllEmployeesSelectOptionsResponse
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
