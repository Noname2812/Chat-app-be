using ChatApp.Data.Modals;
using ChatApp.Data.Repository.Messages;
using ChatApp.Data.Repository.RoomChats;
using ChatApp.Data.Repository.UserRoomChats;
using ChatApp.Data.Repository.Users;

namespace ChatApp.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ChatAppDBContext _dbContext;

        public IUserRepository UserRepository { get; private set; }

        public IUserRoomChatRepository UserRoomChatRepository { get; private set; }

        public IRoomChatRepository RoomChatRepository { get; private set; }

        public IMessageRespository MessageRespository { get; private set; }
        public UnitOfWork(ChatAppDBContext chatAppDBContext)
        {
            _dbContext = chatAppDBContext;
            UserRepository = new UserRepository(_dbContext);
            UserRoomChatRepository = new UserRoomChatRepository(_dbContext);
            RoomChatRepository = new RoomChatRepository(_dbContext);
            MessageRespository = new MessageRepository(_dbContext);
        }

        public void Dispose()
        {
            _dbContext.Dispose();
        }

        public bool HasChanges()
        {
            return _dbContext.ChangeTracker.HasChanges();
        }

        public async Task SaveChanges()
        {
            if (HasChanges())
            {
                await _dbContext.SaveChangesAsync();
            }
        }

        public void BeginTransaction()
        {
            _dbContext.Database.BeginTransaction();
        }

        public async Task CommitTransactionAsync()
        {
            try
            {
                await _dbContext.SaveChangesAsync();
                _dbContext.Database.CommitTransaction();
            }
            catch (Exception ex)
            {
                _dbContext.Database.RollbackTransaction();
                throw new Exception("An error occurred while save changes.", ex);
            }
        }
    }
}
