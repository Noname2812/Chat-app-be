using ChatApp.Data.Modals;
using ChatApp.Models.ResponeModels;

namespace ChatApp.Data.Repository.Users
{
    public interface IUserRepository : IChatAppRepository<User>
    {
        Task<ICollection<User>?> GetFriendsById(int userId);
        Task<ICollection<User>?> GetReceivedFriendRequests(int id);
        Task<Friendship> SendFriendRequest(int from, int to);
        Task UpdateFriendRequest(int from, int to, string status);
        Task UpdateStatusOnline(int userId, bool status);
        Task<List<SearchUserResult>> SearchUserByQuery(string query,int id, int offset = 0, int limit = 10);
    }
}
