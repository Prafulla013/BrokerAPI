namespace Application.Common.Interfaces
{
    public interface IMemoryCacheService
    {
        TResult Get<TResult>(string key);
        void Set<TObject>(string key, TObject value);
    }
}
