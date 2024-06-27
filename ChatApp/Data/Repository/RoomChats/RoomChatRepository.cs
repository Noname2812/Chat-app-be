using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ChatApp.Data.Repository.RoomChats
{
    public class RoomChatRepository : ChatAppRepository<RoomChat>, IRoomChatRepository
    {
        private readonly ChatAppDBContext _dbContext;
        public RoomChatRepository(ChatAppDBContext db) : base(db)
        {
            _dbContext = db;
        }

        public async Task<RoomChat?> CreateRoomChat(RoomChat roomChat, List<int> listUserId)
        {
            try
            {
                _dbContext.Database.BeginTransaction();
                var newRoom = await Create(roomChat);
                foreach (var userId in listUserId)
                {
                    var rs = await _dbContext.UserRoomChat.AddAsync(new UserRoomChat { RoomChatId = newRoom.Id, UserId = userId });
                    if (rs is null)
                    {
                        _dbContext.Database.RollbackTransaction();
                    }
                }
                await _dbContext.SaveChangesAsync();
                _dbContext.Database.CommitTransaction();
                return newRoom;
            }
            catch (Exception ex)
            {
                _dbContext.Database.RollbackTransaction();
                throw new Exception("An error occurred while creating the RoomChat.", ex);
            }
        }

        public async Task<List<RoomChat>?> GetAllRoomChatByIdUser(string? id)
        {
            var roomChats = await _dbContext.UserRoomChat.Where(r => r.UserId == int.Parse(id))
                .Join(_dbContext.RoomChats, user_roomchat => user_roomchat.RoomChatId, room => room.Id, (userroomchat, room) => room).OrderByDescending(r => r.ModifiedDate).Include(r => r.Messages.OrderByDescending(x => x.CreateAt).Skip(0).Take(10)).ToListAsync();
            return roomChats;
        }
        public async Task<RoomChat?> GetRoomChatById(int roomId, int offset = 0, int limit = 10)
        {
            var room = await _dbContext.RoomChats.Where(r => r.Id == roomId).Include(x => x.Messages.Take(limit).Skip(offset)).FirstOrDefaultAsync();
            return room;
        }
        public async Task<RoomChat?> GetRoomChatPrivateBetweenTwoUser(int? userId1, int? userId2)
        {
            if (userId1 == null || userId2 == null) { return null; }
            var roomUser_1 = await _dbContext.RoomChats.Where(r => r.IsPrivate).Include(x => x.UserRoomChat).Include(x => x.Messages.Take(10).Skip(0)).ToListAsync();
            var roomUser_2 = await _dbContext.RoomChats.Where(r => r.IsPrivate).Include(x => x.UserRoomChat
            .Where(r => r.UserId == userId2)).Include(x => x.Messages.Take(10).Skip(0)).ToListAsync();
            var r1 = await _dbContext.UserRoomChat.Where(x => x.UserId == userId1).ToListAsync();
            var r2 = await _dbContext.UserRoomChat.Where(x => x.UserId == userId2).ToListAsync();
            if (r1.Count > 0 && r2.Count > 0)
            {
                var roomResult = roomUser_1.Intersect(roomUser_2).FirstOrDefault();
                return roomResult;
            }
            return null;
        }

    }
}
