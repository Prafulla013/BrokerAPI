using Application.Common.Interfaces;
using Common.Enumerations;
using Domain.Entities;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Application.ActivityLogs.Queries
{
    internal class ListActivityLogsHandler : IRequestHandler<ListActivityLogQuery, List<ListActivityLogResponse>>
    {
        private readonly IBrokerDbContext _dbContext;
        public ListActivityLogsHandler(IBrokerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<List<ListActivityLogResponse>> Handle(ListActivityLogQuery request, CancellationToken cancellationToken)
        {
            var response = _dbContext.PropertyActivityLogs.Where(w => w.BrokerId == request.BrokerId)
                                                               .Select(s => new ListActivityLogResponse
                                                               {
                                                                   Id = s.Id,
                                                                   Parameters = s.Parameters,
                                                                   Comment = s.Comment,
                                                                   LastUpdateDateTime = s.LastUpdateDateTime
                                                               }).AsEnumerable()
                                                               .Union(_dbContext.UserActivityLogs.Where(w => w.ModuleAction == ModuleAction.Login &&
                                                                                w.ModuleGroup == ModuleGroup.Account &&
                                                                                w.StatusCode == HttpStatusCode.OK &&
                                                                                w.UserProfile.BrokerId == request.BrokerId)
                                                                .Include(i => i.UserProfile)
                                                                .Select(s => new ListActivityLogResponse
                                                                {
                                                                    Id = s.Id,
                                                                    Parameters = new List<ParamObject>
                                                                    {
                                                                        new ParamObject { Type = "string", Value = $"{s.UserProfile.FirstName} {s.UserProfile.LastName}" },
                                                                        new ParamObject { Type = "date", Value = $"{s.LastUpdateDateTime}" },
                                                                        new ParamObject { Type = "time", Value = $"{s.LastUpdateDateTime}" },

                                                                    },
                                                                    Comment = "{0} logged in {1} at {2}.",
                                                                    LastUpdateDateTime = s.LastUpdateDateTime
                                                                }))
                                                                .OrderByDescending(o => o.LastUpdateDateTime)
                                                                .ToList();

            return response;
        }
    }

    public class ListActivityLogQuery : IRequest<List<ListActivityLogResponse>>
    {
        public long BrokerId { get; set; }
    }

    public class ListActivityLogResponse
    {
        public long Id { get; set; }
        public List<ParamObject> Parameters { get; set; }
        public string Comment { get; set; }
        [JsonIgnore]
        public DateTimeOffset LastUpdateDateTime { get; set; }
    }
}
