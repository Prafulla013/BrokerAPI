using Application.Common.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Employees.Queries
{
    public class ListTopEmployeesHandler : IRequestHandler<ListTopEmployeesQuery, List<ListTopEmployeesResponse>>
    {
        private readonly IBrokerDbContext _dbContext;

        public ListTopEmployeesHandler(IBrokerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ListTopEmployeesResponse>> Handle(ListTopEmployeesQuery request, CancellationToken cancellationToken)
        {
            var response = await _dbContext.UserProfiles.Include(i => i.User)
                                                        .Where(w => w.BrokerId == request.BrokerId && w.HasSystemAccess == true)
                                                        .OrderByDescending(o => o.User.LastAccessedAt)
                                                        .Take(5)
                                                        .Select(s => new ListTopEmployeesResponse
                                                        {
                                                            FirstName = s.FirstName,
                                                            LastName = s.LastName,
                                                            Email = s.Email,
                                                            LastAccessDate = s.User.LastAccessedAt
                                                        })
                                                        .ToListAsync(cancellationToken);

            return response;
        }
    }

    public class ListTopEmployeesQuery : IRequest<List<ListTopEmployeesResponse>>
    {
        public long BrokerId { get; set; }
    }

    public class ListTopEmployeesResponse
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTimeOffset LastAccessDate { get; set; }
    }
}
