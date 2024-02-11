using Application.Common.Interfaces;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Services
{
    public class ContextService : IContextService
    {
        private readonly string _connectionString;
        public ContextService(string connectionString)
        {
            _connectionString = connectionString;
        }

        public IBrokerDbContext CreateContext()
        {
            var _dbContextBuilder = new DbContextOptionsBuilder<BrokerDbContext>().UseSqlServer(_connectionString);
            var dbContext = new BrokerDbContext(_dbContextBuilder.Options);
            return dbContext;
        }
    }
}
