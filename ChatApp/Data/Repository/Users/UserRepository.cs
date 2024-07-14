using ChatApp.Data.Modals;
using ChatApp.Models.DTOs;
using ChatApp.Models.ResponeModels;
using Microsoft.EntityFrameworkCore;
namespace ChatApp.Data.Repository.Users
{
    public class UserRepository : ChatAppRepository<User>, IUserRepository
    {
        private readonly ChatAppDBContext _dbContext;
        public UserRepository(ChatAppDBContext db) : base(db)
        {
            _dbContext = db;
        }
        public async Task<ICollection<User>?> GetFriendsById(Guid userId)
        {
            var user = await _dbContext.Users
            .Include(u => u.Friends.Where(x => x.Status == "Accepted"))
                .ThenInclude(f => f.Friend)
            .Include(u => u.FriendsOf.Where(x => x.Status == "Accepted"))
                .ThenInclude(f => f.User)
            .FirstOrDefaultAsync(u => u.Id == userId);
            if (user != null)
            {
                var friends = user.Friends.Select(f => f.Friend)
              .Union(user.FriendsOf.Select(f => f.User))
              .ToList();
                return friends;
            }
            return null;
        }

        public async Task<ICollection<User>?> GetReceivedFriendRequests(Guid id)
        {
            var user = await _dbContext.Users
           .Include(u => u.FriendsOf)
               .ThenInclude(f => f.User)
           .FirstOrDefaultAsync(u => u.Id == id);
            if (user != null)
            {
                var receivedRequests = user.FriendsOf
                   .Where(f => f.Status == "Pending")
                   .Select(f => f.User)
                   .ToList();
                return receivedRequests;
            }
            return [];
        }

        public async Task<List<SearchUserResult>> SearchUserByQuery(string query, Guid id, int offset = 0, int limit = 10)
        {

            var users = await _dbContext.Users.Where(x => (x.Name.ToLower().IndexOf(query.ToLower()) > -1
            || x.Email.ToLower().IndexOf(query.ToLower()) > -1) && x.Id != id).Take(limit).Skip(offset).ToListAsync();
            return users.Select(x => new SearchUserResult { Avatar = x.Avatar, Id = x.Id, Name = x.Name, FriendShip = getStatusFriend(id, x.Id) }).ToList();
        }

        public async Task<Friendship?> SendFriendRequest(Guid from, Guid to)
        {
            var request = await _dbContext.Friends.Where(x => x.UserId == from && x.FriendId == to).FirstOrDefaultAsync();
            if (request == null)
            {
                var newRequest = new Friendship { UserId = from, FriendId = to, Status = "Pending", CreatedAt = DateTime.Now };
                _dbContext.Friends.Add(newRequest);
                await _dbContext.SaveChangesAsync();
                return newRequest;
            }
            return null;
        }

        public async Task UpdateFriendRequest(Guid from, Guid to, string status)
        {
            var request = await _dbContext.Friends.Where(x => (x.UserId == from && x.FriendId == to) || (x.UserId == to && x.FriendId == from)).FirstOrDefaultAsync();
            if (request != null)
            {
                request.Status = status;
            }
        }

        public async Task UpdateStatusOnline(Guid userId, bool status)
        {
            var user = await GetItemByQuery(x => x.Id == userId);
            if (user != null)
            {
                user.IsOnline = status;
                if (!status)
                {
                    user.LastOnline = DateTime.Now;
                }
            }

        }
        private FriendShipDTO? getStatusFriend(Guid id, Guid friendId)
        {
            var requestAddFriend = _dbContext.Friends.Where(x => (x.UserId == id && x.FriendId == friendId) || (x.UserId == friendId && x.FriendId == id)).FirstOrDefault();
            if (requestAddFriend != null)
            {
                return new FriendShipDTO { FriendId = requestAddFriend.FriendId, Status = requestAddFriend.Status, UserId = requestAddFriend.UserId };
            }
            return null;
        }
    }
}
