using ChatApp.Data.Modals;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data.Repository.RoomChats
{
    public class RoomChatRepository : ChatAppRepository<RoomChat>, IRoomChatRepository
    {
        private readonly ChatAppDBContext _dbContext;
        public RoomChatRepository(ChatAppDBContext db) : base(db)
        {
            _dbContext = db;
        }

        public async Task<RoomChat> CreateRoomChat(RoomChat roomChat, List<Guid> listUserId)
        {
            var newRoom = Create(roomChat);
            foreach (var userId in listUserId)
            {
                await _dbContext.UserRoomChat.AddAsync(new UserRoomChat { RoomChatId = newRoom.Id, UserId = userId });
            }
            return newRoom;
        }

        public async Task<List<RoomChat>?> GetAllRoomChatByIdUser(Guid? id)
        {
            var roomChats = await _dbContext.UserRoomChat.Where(r => r.UserId == id)
                .Join(_dbContext.RoomChats, user_roomchat => user_roomchat.RoomChatId, room => room.Id, (userroomchat, room) => room)
                .OrderByDescending(r => r.ModifiedDate)
                .Include(r => r.Messages.OrderByDescending(x => x.CreateAt)
                .Skip(0).Take(10)).ToListAsync();
            return roomChats;
        }
        public async Task<RoomChat?> GetRoomChatById(Guid roomId, int offset = 0, int limit = 10)
        {
            var room = await _dbContext.RoomChats.Where(r => r.Id == roomId)
               .Include(r => r.Messages.OrderByDescending(x => x.CreateAt)
                .Skip(offset).Take(limit)).FirstOrDefaultAsync();
            return room;
        }
        public async Task<RoomChat?> GetRoomChatPrivateBetweenTwoUser(Guid? userId1, Guid? userId2)
        {
            if (userId1 == null || userId2 == null) { return null; }
            var userRoomResult = await (from urc1 in _dbContext.UserRoomChat
                                        join urc2 in _dbContext.UserRoomChat
                                        on urc1.RoomChatId equals urc2.RoomChatId
                                        where urc1.UserId == userId1 && urc2.UserId == userId2
                                        select urc1).FirstOrDefaultAsync();
            if (userRoomResult is not null)
            {
                return await GetItemByQuery(x => x.Id == userRoomResult.RoomChatId);
            }
            return null;
        }

    }
}
