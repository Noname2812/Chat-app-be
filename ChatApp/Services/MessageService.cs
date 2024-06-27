using ChatApp.Data;
using ChatApp.Data.Repository.Messages;

namespace ChatApp.Services
{
    public class MessageService : MessageRepository
    {
        public MessageService(ChatAppDBContext db) : base(db)
        {
        }

    }
}
