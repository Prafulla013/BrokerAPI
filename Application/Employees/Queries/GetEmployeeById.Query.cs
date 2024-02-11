using Application.Common.Interfaces;
using Common.Enumerations;
using Common.Exceptions;
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
    public class GetEmployeeByIdHandler : IRequestHandler<GetEmployeeByIdQuery, GetEmployeeByIdResponse>
    {
        private readonly IBrokerDbContext _dbContext;
        public GetEmployeeByIdHandler(IBrokerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<GetEmployeeByIdResponse> Handle(GetEmployeeByIdQuery request, CancellationToken cancellationToken)
        {
            var dbProfile = await _dbContext.UserProfiles.Include(i => i.User)
                                                         .FirstOrDefaultAsync(fd => fd.Id == request.Id &&
                                                                                    fd.BrokerId == request.BrokerId &&
                                                                                    fd.Type == UserType.Employee,
                                                                                    cancellationToken);
            if (dbProfile == null)
            {
                throw new NotFoundException("Invalid employee id.");
            }

            var response = new GetEmployeeByIdResponse
            {
                Id = dbProfile.Id,
                FirstName = dbProfile.FirstName,
                LastName = dbProfile.LastName,
                Email = dbProfile.Email,
                PhoneNumber = dbProfile.PhoneNumber,
                Username = dbProfile.User?.UserName,
                State = dbProfile.State,
                City = dbProfile.City,
                Street = dbProfile.Street,
                ZipCode = dbProfile.ZipCode,
                HasSystemAccess = dbProfile.HasSystemAccess,
                UserType = dbProfile.Type
            };
            return response;
        }
    }

    public class GetEmployeeByIdQuery : IRequest<GetEmployeeByIdResponse>
    {
        public string Id { get; set; }
        [JsonIgnore]
        public long BrokerId { get; set; }
    }

    public class GetEmployeeByIdResponse
    {
        public string Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Username { get; set; }
        public string City { get; set; }
        public string Street { get; set; }
        public string ZipCode { get; set; }
        public string State { get; set; }
        public bool HasSystemAccess { get; set; }
        public UserType UserType { get; set; }
    }
}
