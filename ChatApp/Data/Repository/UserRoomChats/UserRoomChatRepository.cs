using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data.Repository.UserRoomChats
{
    public class UserRoomChatRepository : ChatAppRepository<UserRoomChat>, IUserRoomChatRepository 
    {
        public UserRoomChatRepository(ChatAppDBContext db) : base(db)
        {
           
        }
    }
}
