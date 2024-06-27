namespace ChatApp.Data.Repository.Messages
{
    public class MessageRepository : ChatAppRepository<Message>, IMessageRespository
    {
        private readonly ChatAppDBContext _dbContext;

        public MessageRepository(ChatAppDBContext db) : base(db)
        {
            _dbContext = db;
        }
        // some method different IMessageRespository
    }
}
