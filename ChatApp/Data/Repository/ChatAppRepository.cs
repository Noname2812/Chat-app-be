using ChatApp.Data.Modals;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;

namespace ChatApp.Data.Repository
{
    public class ChatAppRepository<T> : IChatAppRepository<T> where T : class
    {
        private readonly ChatAppDBContext _dbContext;
        private DbSet<T> _dbSet;

        public ChatAppRepository(ChatAppDBContext db)
        {
            _dbContext = db;
            _dbSet = _dbContext.Set<T>();
        }
        public T Create(T record)
        {
            _dbSet.Add(record);
            return record;
        }

        public void Delete(T record)
        {
            _dbSet.Remove(record);
        }

        public async Task<List<T>?> GetAll()
        {
            return await _dbSet.ToListAsync();

        }

        public async Task<T?> GetItemByQuery(Expression<Func<T, bool>> filter, bool isNotTracking = false)
        {
            if (isNotTracking)
            {
                return await _dbSet.AsNoTracking().Where(filter).FirstOrDefaultAsync();

            }
            return await _dbSet.Where(filter).FirstOrDefaultAsync();
        }

        public async Task<List<T>?> GetListItemByQuery(Expression<Func<T, bool>> filter, bool isNotTracking = false, int offset = 0, int limit = 10)
        {
            if (isNotTracking)
            {
                return await _dbSet.AsNoTracking().Where(filter).Skip(offset).Take(limit).ToListAsync();

            }
            return await _dbSet.Where(filter).Skip(offset).Take(limit).ToListAsync();
        }

        public void Update(T record)
        {
            _dbSet.Attach(record);
            _dbContext.Entry(record).State = EntityState.Modified;
        }
        public async Task<List<T>?> ExecuteSql(string query)
        {
            var result = await _dbContext.Database
                                .SqlQuery<T>(FormattableStringFactory.Create(query)).ToListAsync();
            return result;
        }

        public void AddListData(List<T> list)
        {
            try
            {
                _dbSet.AddRange(list);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
    }
}
