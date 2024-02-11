using Application.Common.Interfaces;
using Common.Enumerations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Employees.Queries
{
    public class ListEmployeesHandler : IRequestHandler<ListEmployeesQuery, List<ListEmployeesResponse>>
    {
        private readonly IBrokerDbContext _dbContext;
        public ListEmployeesHandler(IBrokerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ListEmployeesResponse>> Handle(ListEmployeesQuery request, CancellationToken cancellationToken)
        {
            var dbUsers = await _dbContext.UserProfiles.Where(w => w.BrokerId == request.BrokerId && w.Type == UserType.Employee)
                                                       .ToListAsync(cancellationToken);


            var response = dbUsers.Select(s => new ListEmployeesResponse
            {
                Id = s.Id,
                FirstName = s.FirstName,
                LastName = s.LastName,
                PhoneNumber = s.PhoneNumber,
                Street = s.Street,
                City = s.City,
                State = s.State,
                ZipCode = s.ZipCode,
                Email = s.Email
            }).ToList();

            return response;
        }
    }

    public class ListEmployeesQuery : IRequest<List<ListEmployeesResponse>>
    {
        [JsonIgnore]
        public long BrokerId { get; set; }
    }

    public class ListEmployeesResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        [JsonIgnore]
        public string Street { get; set; }
        [JsonIgnore]
        public string State { get; set; }
        [JsonIgnore]
        public string City { get; set; }
        [JsonIgnore]
        public string ZipCode { get; set; }
        public string HomeAddress
        {
            get
            {
                var address = new List<string> { Street };
                if (!string.IsNullOrEmpty(City))
                {
                    address.Add(City);
                }
                if (!string.IsNullOrEmpty(State))
                {
                    address.Add(State);
                }
                if (!string.IsNullOrEmpty(ZipCode))
                {
                    address.Add(ZipCode);
                }

                return string.Join(", ", address);
            }
        }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public bool HasSystemAccess { get; set; }
    }
}
