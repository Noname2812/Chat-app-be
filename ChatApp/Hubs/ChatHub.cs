
using ChatApp.Data;
using ChatApp.Data.Repository.RoomChats;
using ChatApp.Data.Repository.Users;
using ChatApp.Models;
using ChatApp.Models.Request;
using ChatApp.Services;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;





namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUserRepository _userRepository;
        private readonly IRoomChatRepository _roomChatRepository;
        private readonly ChatAppDBContext _dbContext;
        private readonly ICacheService _cacheService;

        public ChatHub( IUserRepository userRepository, IRoomChatRepository roomChatRepository, ChatAppDBContext dbContext, ICacheService cacheService) : base()
        {
            _roomChatRepository = roomChatRepository;
            _userRepository = userRepository;
            _dbContext = dbContext;
            _cacheService = cacheService;
        }
        public override async Task OnConnectedAsync()
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, "ChatApp");
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var usersConn = await _cacheService.GetDataByEndpoint<UserConnection>("list-users-online");
            var current = usersConn.Where(user => user.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if (current != null)
            {
                var user = await _userRepository.GetItemByQuery(x => x.Id == current.UserId);
                if (user != null)
                {
                    user.LastOnline = DateTime.Now;
                    await _userRepository.Update(user);
                }
                // remove cache data
                await _cacheService.RemoveDataByKey($"list-users-online:{current.UserId}");
            }
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, "ChatApp");
            // return list users online
            var cachedUserOnline = await _cacheService.GetDataByEndpoint<UserConnection>("list-users-online");
            await Clients.Group("ChatApp").SendAsync("GetListUserOnline", cachedUserOnline);
        }
        public async Task AddUserConnection(UserConnection user)
        {
            UserConnection userConn = new UserConnection
            {
                ConnectionId = Context.ConnectionId,
                Name = user.Name,
                UserId = user.UserId,
            };
            // add user in cache
            await _cacheService.SetData($"list-users-online:{userConn.UserId}", userConn, TimeSpan.FromSeconds(6000));
            var roomChats = await _dbContext.UserRoomChat.Where(r => r.UserId == user.UserId)
                .Join(_dbContext.RoomChats, user_roomchat => user_roomchat.RoomChatId, room => room.Id, (userroomchat, room) => room).Where(r => r.IsPrivate == false).ToListAsync();
            foreach (var room in roomChats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, room.Name);
            }

            // get users online in cache
            var cachedUserOnline = await _cacheService.GetDataByEndpoint<UserConnection>("list-users-online");
            await Clients.Group("ChatApp").SendAsync("GetListUserOnline", cachedUserOnline);
            await Clients.Others.SendAsync("NotifyUserOnline", user.Name);
        }
        public async Task SendMessage(MessageRequest req)
        {
            if (req.isPrivate)
            {
                var to = await _dbContext.UserRoomChat.Where(x => x.RoomChatId == req.RoomId && x.UserId != req.from).FirstOrDefaultAsync();
                if (to != null)
                {
                    var usersConn = await _cacheService.GetDataByEndpoint<UserConnection>("list-users-online");
                    var toUser = usersConn.Where(user => user.UserId == to.UserId).FirstOrDefault();
                    if (toUser.ConnectionId != null)
                    {
                        await Clients.Client(toUser.ConnectionId).SendAsync("ReceiveMessagePrivate", to.RoomChatId.ToString());
                    }
                }
            }
            else
            {
                var room = await _roomChatRepository.GetItemByQuery(x => x.Id == req.RoomId);
                if (room != null)
                {
                    await Clients.Group(room.Name).SendAsync("ReceiveMessageFromGroup", req.RoomId.ToString());
                }
            }
        }
    }
}
