using ChatApp.Data.Modals;

namespace ChatApp.Data.Repository.RoomChats
{
    public interface IRoomChatRepository : IChatAppRepository<RoomChat>
    {
        // add some method different IChatAppRepository
        Task<List<RoomChat>?> GetAllRoomChatByIdUser(Guid? id);
        Task<RoomChat?> GetRoomChatById(Guid roomId, int offset = 0, int limit = 10);
        Task<RoomChat?> GetRoomChatPrivateBetweenTwoUser(Guid? userId1, Guid? userId2);
        Task<RoomChat> CreateRoomChat(RoomChat roomChat, List<Guid> listUserId);
    }
}
