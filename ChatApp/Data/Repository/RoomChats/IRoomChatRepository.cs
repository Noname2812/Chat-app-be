using ChatApp.Data.Modals;
using ChatApp.Models.MessageModels;

namespace ChatApp.Data.Repository.RoomChats
{
    public interface IRoomChatRepository : IChatAppRepository<RoomChat>
    {
        // add some method different IChatAppRepository
        Task<List<RoomChat>?> GetAllRoomChatByIdUser(string? id);
        Task<RoomChat?> GetRoomChatById(int roomId, int offset = 0, int limit = 10);
        Task<RoomChat?> GetRoomChatPrivateBetweenTwoUser(int? userId1, int? userId2);
        Task<RoomChat?> CreateRoomChat(RoomChat roomChat, List<int> listUserId);
    }
}
