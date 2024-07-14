


using ChatApp.Data.Modals;
using ChatApp.Data.Repository.Messages;
using ChatApp.Data.Repository.RoomChats;
using ChatApp.Data.Repository.UserRoomChats;
using ChatApp.Data.Repository.Users;

namespace ChatApp.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        IUserRepository UserRepository { get; }
        IUserRoomChatRepository UserRoomChatRepository { get; }
        IRoomChatRepository RoomChatRepository { get; }
        IMessageRespository MessageRespository { get; }
        Task SaveChanges();
        bool HasChanges();
        void BeginTransaction();
        Task CommitTransactionAsync();
    }
}
