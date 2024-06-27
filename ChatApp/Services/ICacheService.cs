namespace ChatApp.Services
{
    public interface ICacheService
    {
        Task<string> GetDataByKey(string key);
        Task<List<T>?> GetDataByEndpoint<T>(string endpoint);
        Task SetData(string key, object data, TimeSpan time);
        Task RemoveDataByKey(string key);
        Task RemoveCacheByPartern(string key, string partern);
    }
}
