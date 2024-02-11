namespace Application.Common.Interfaces
{
    public interface IContextService
    {
        IBrokerDbContext CreateContext();
    }
}
