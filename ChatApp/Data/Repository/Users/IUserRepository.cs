namespace ChatApp.Data.Repository.Users
{
    public interface IUserRepository : IChatAppRepository<User>
    {
        Task<ICollection<User>?> GetFriendsById(int userId);
        Task<ICollection<User>?> GetReceivedFriendRequests(int id);
        Task SendFriendRequest(int from, int to);
        Task UpdateFriendRequest(int from, int to, string status);
        Task UpdateStatusOnline(int userId, bool status);
    }
}
