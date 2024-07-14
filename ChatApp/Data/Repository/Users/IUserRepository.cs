using ChatApp.Data.Modals;
using ChatApp.Models.ResponeModels;

namespace ChatApp.Data.Repository.Users
{
    public interface IUserRepository : IChatAppRepository<User>
    {
        Task<ICollection<User>?> GetFriendsById(Guid userId);
        Task<ICollection<User>?> GetReceivedFriendRequests(Guid id);
        Task<Friendship?> SendFriendRequest(Guid from, Guid to);
        Task UpdateFriendRequest(Guid from, Guid to, string status);
        Task UpdateStatusOnline(Guid userId, bool status);
        Task<List<SearchUserResult>> SearchUserByQuery(string query,Guid id, int offset = 0, int limit = 10);
    }
}
