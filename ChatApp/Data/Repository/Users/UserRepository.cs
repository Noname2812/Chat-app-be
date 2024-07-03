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
        public async Task<ICollection<User>?> GetFriendsById(int userId)
        {
            var user = await _dbContext.Users
            .Include(u => u.Friends.Where(x => x.Status == "Accepted"))
                .ThenInclude(f => f.Friend)
            .Include(u => u.FriendsOf)
                .ThenInclude(f => f.User)
            .FirstOrDefaultAsync(u => u.Id == userId);
            var friends = user.Friends.Select(f => f.Friend)
                .Union(user.FriendsOf.Select(f => f.User))
                .ToList();
            return friends;
        }

        public async Task<ICollection<User>?> GetReceivedFriendRequests(int id)
        {
            var user = await _dbContext.Users
           .Include(u => u.FriendsOf)
               .ThenInclude(f => f.User)
           .FirstOrDefaultAsync(u => u.Id == id);
            var receivedRequests = user.FriendsOf
               .Where(f => f.Status == "Pending")
               .Select(f => f.User)
               .ToList();
            return receivedRequests;
        }

        public async Task SendFriendRequest(int from, int to)
        {
            var request = await _dbContext.Friends.Where(x => (x.UserId == from && x.FriendId == to) || (x.UserId == to && x.FriendId == from)).FirstOrDefaultAsync();
            if (request == null)
            {
                var newRequest = new Friendship { UserId = from, FriendId = to, Status = "Pending", CreatedAt = DateTime.Now };
                 _dbContext.Friends.Add(newRequest);
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                request.Status = "Accepted";
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task UpdateFriendRequest(int from, int to, string status)
        {
            var request = await _dbContext.Friends.Where(x => x.UserId == from && x.FriendId == to).FirstOrDefaultAsync();
            if (request != null)
            {
                request.Status = status;
                await _dbContext.SaveChangesAsync();
            }
        }
    }
}
