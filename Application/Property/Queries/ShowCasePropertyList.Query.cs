using Application.Common.Interfaces;
using Common.Enumerations;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ProperyManagement.Queries
{
    public class ShowCasePropertyManagementHandler : IRequestHandler<ShowCasePropertyManagementQuery, List<ShowCasePropertyManagementResponse>>
    {
        private readonly IBrokerDbContext _dbContext;
        public ShowCasePropertyManagementHandler(IBrokerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ShowCasePropertyManagementResponse>> Handle(ShowCasePropertyManagementQuery request, CancellationToken cancellationToken)
        {

            var response = await _dbContext.PropertyManagements.Include(i => i.Broker)
                                                        .AsNoTracking()
                                                        .Select(s => new ShowCasePropertyManagementResponse
                                                        {
                                                            Id = s.Id,
                                                            Location = s.Location,
                                                            Price = s.Price,
                                                            Description = s.Description,
                                                            BrokerId = s.BrokerId,
                                                            BrokerImage = s.Broker.BrokerImage
                                                        })
                                                        .Where(w=> w.Price >= request.lowPrice && w.Price <= request.highPrice)
                                                        .ToListAsync(cancellationToken);

            return response;
        }
    }

    public class ShowCasePropertyManagementQuery : IRequest<List<ShowCasePropertyManagementResponse>>
    {
        public decimal lowPrice { get; set; }
        public decimal highPrice { get; set; }
    }

    public class ShowCasePropertyManagementResponse
    {
        public long Id { get; set; }
        public string Location { get; set; }
        public decimal Price { get; set; }
        public string Description { get; set; }
        public long BrokerId { get; set; }
        public string BrokerImage { get; set; }
    }
}
