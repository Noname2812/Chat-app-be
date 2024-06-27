
using ChatApp.Models;

namespace ChatApp.Services
{
    public class ChatService
    {
        private static List<UserConnection> Users = new List<UserConnection>();

        public void AddUserConnection(UserConnection user)
        {
            lock (Users)
            {
                UserConnection? check = Users.Find(x => x.UserId == user.UserId);
                if (check == null)
                {
                    Users.Add(user);
                }
            }
        }
        public void RemoveUserDisconnect(string connectionId)
        {
            lock (Users)
            {
                var user = Users.FirstOrDefault(x => x.ConnectionId == connectionId);
                if (user != null)
                {
                    Users.Remove(user);
                }
            }
        }
        public int? getUserIdByConnectionId(string connectionId)
        {
            lock (Users)
            {
                return Users.Where(x => x.ConnectionId == connectionId).Select(x => x.UserId).FirstOrDefault();
            }
        }
        public string? getConnectionIdByUserId(int? userId)
        {
            lock (Users)
            {
                return Users.Where(x => x.UserId == userId).Select(x => x.ConnectionId).FirstOrDefault();
            }
        }
        public List<UserConnection> getUsersOnline ()
        {
            lock (Users) {
                return Users.OrderBy(x => x.Name).ToList();
            }
        }
    }
}
