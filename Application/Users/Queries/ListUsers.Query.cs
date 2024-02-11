using Application.Common.Interfaces;
using Common.Enumerations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Users.Queries
{
    public class ListUsersHandler : IRequestHandler<ListUsersQuery, List<ListUsersResponse>>
    {
        private readonly IBrokerDbContext _dbContext;
        public ListUsersHandler(IBrokerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ListUsersResponse>> Handle(ListUsersQuery request, CancellationToken cancellationToken)
        {

            var response = await _dbContext.UserProfiles.Include(i => i.User)
                                                        .ThenInclude(i=> i.UserRoles)
                                                        .ThenInclude(i=> i.Role)
                                                        .Where(w => w.BrokerId == request.BrokerId &&
                                                                    w.HasSystemAccess == true &&
                                                                    w.User != null)
                                                        // check user null as there might be null recordsd eventhough has system of profile is true
                                                        .AsNoTracking()
                                                        .Select(s => new ListUsersResponse
                                                        {
                                                            Id = s.Id,
                                                            FirstName = s.FirstName,
                                                            LastName = s.LastName,
                                                            Username = s.User.UserName,
                                                            Email = s.Email,
                                                            IsEmailConfirmed = s.User.EmailConfirmed,
                                                            PhoneNumber = s.PhoneNumber,
                                                            LastAccessDate = s.User.LastAccessedAt
                                                        })
                                                        .OrderBy(o => o.FirstName).ThenBy(o => o.LastName)
                                                        .ToListAsync(cancellationToken);

            return response;
        }
    }

    public class ListUsersQuery : IRequest<List<ListUsersResponse>>
    {
        public long? BrokerId { get; set; }
    }

    public class ListUsersResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public bool IsEmailConfirmed { get; set; }
        public UserType[] UserTypes { get; set; }
        public string PhoneNumber { get; set; }
        public DateTimeOffset LastAccessDate { get; set; }
    }
}
