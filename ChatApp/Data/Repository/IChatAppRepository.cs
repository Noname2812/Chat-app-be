using System.Linq.Expressions;

namespace ChatApp.Data.Repository
{
    public interface IChatAppRepository<T> where T : class
    {
        Task<List<T>?> GetAll();
        Task<T?> GetItemByQuery(Expression<Func<T, bool>> filter, bool isNotTracking = false);
        Task<List<T>?> GetListItemByQuery(Expression<Func<T, bool>> filter, bool isNotTracking = false, int offset = 0, int limit = 10);
        Task<T> Update(T record);
        Task<T> Delete(T record);
        Task<T> Create(T record);
        Task<List<T>?> ExecuteSql(string query);
        Task AddListData(List<T> list);
    }
}
