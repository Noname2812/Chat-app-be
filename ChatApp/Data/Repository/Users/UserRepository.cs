namespace ChatApp.Data.Repository.Users
{
    public class UserRepository : ChatAppRepository<User>, IUserRepository
    {
        private readonly ChatAppDBContext _dbContext;
        public UserRepository(ChatAppDBContext db) : base(db)
        {
            _dbContext = db;
        }
    }
}
