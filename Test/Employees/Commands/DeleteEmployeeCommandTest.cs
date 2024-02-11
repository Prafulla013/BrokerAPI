using Application.Employees.Commands;
using Common.Exceptions;
using Infrastructure.Services;
using System.Threading;
using System.Threading.Tasks;
using Test.Setup;
using Xunit;

namespace Test.Employees.Commands
{
    public class DeleteEmployeeCommandTest : BaseTest
    {
        public DeleteEmployeeCommandTest() : base(nameof(DeleteEmployeeCommandTest) + _dbCount++) { }

        [Theory]
        [InlineData(SetupBroker.EMP_ID, SetupBroker.ID2)]
        public async Task Delete_ShouldDeleteEmployee(string employeeId, long brokerId)
        {
            // Arrange
            var command = new DeleteEmployeeCommand
            {
                Id = employeeId,
                BrokerId = brokerId
            };
            var cancellationToken = new CancellationToken();
            // TODO: Revisit
            var contextService = new ContextService("");
            var memoryCache = new MemoryCacheService(null);
            var handler = new DeleteEmployeeHandler(_dbContext);

            // Act
            await handler.Handle(command, cancellationToken);

            // Assert
            var dbEmployee = await _dbContext.UserProfiles.FindAsync(employeeId);
            Assert.Null(dbEmployee);
        }

        [Theory]
        [InlineData(SetupBroker.EMP_ID, SetupBroker.ID1)]
        [InlineData("", SetupBroker.ID1)]
        [InlineData(SetupBroker.EMP_ID, 0)]
        public async Task Delete_Invalid_ShouldThrowNotFoundxception(string employeeId, long brokerId)
        {
            // Arrange
            var command = new DeleteEmployeeCommand
            {
                Id = employeeId,
                BrokerId = brokerId
            };
            var cancellationToken = new CancellationToken();
            // TODO: Revisit
            var contextService = new ContextService("");
            var memoryCache = new MemoryCacheService(null);
            var handler = new DeleteEmployeeHandler(_dbContext);

            // Act
            var result = await Assert.ThrowsAnyAsync<NotFoundException>(async () => await handler.Handle(command, cancellationToken));

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Invalid employee id.", result.Message);
        }
    }
}
