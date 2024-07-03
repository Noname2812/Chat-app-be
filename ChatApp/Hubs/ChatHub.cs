
using AutoMapper;
using ChatApp.Data;
using ChatApp.Data.Repository.RoomChats;
using ChatApp.Data.Repository.Users;
using ChatApp.Models;
using ChatApp.Models.DTOs;
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

        public ChatHub(IUserRepository userRepository, IRoomChatRepository roomChatRepository, ChatAppDBContext dbContext, ICacheService cacheService) : base()
        {
            _roomChatRepository = roomChatRepository;
            _userRepository = userRepository;
            _dbContext = dbContext;
            _cacheService = cacheService;
        }
        public override async Task OnConnectedAsync()
        {
            //await Groups.AddToGroupAsync(Context.ConnectionId, "ChatApp");
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
                // return list users online
                await NotifyFriendsOnline(current.UserId);
            }
        }
        public async Task AddUserConnection(UserConnection user)
        {
            UserConnection userConn = new UserConnection
            {
                ConnectionId = Context.ConnectionId,
                Name = user.Name,
                UserId = user.UserId,
                Avatar = user.Avatar ?? "",
            };
            // add user in cache
            await _cacheService.SetData($"list-users-online:{userConn.UserId}", userConn, TimeSpan.FromHours(24));
            // join group chat
            var roomChats = await _dbContext.UserRoomChat.Where(r => r.UserId == user.UserId)
                .Join(_dbContext.RoomChats, user_roomchat => user_roomchat.RoomChatId, room => room.Id, (userroomchat, room) => room)
                .Where(r => r.IsPrivate == false).ToListAsync();
            foreach (var room in roomChats)
            {
                await Groups.AddToGroupAsync(Context.ConnectionId, room.Name);
            }
            // notify list friends online
            await NotifyFriendsOnline(user.UserId);
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
                    if (toUser != null && toUser.ConnectionId != null)
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
        private async Task NotifyFriendsOnline(int id)
        {
            
            var cachedUserOnline = await _cacheService.GetDataByEndpoint<UserConnection>("list-users-online");
            var friends = await _userRepository.GetFriendsById(id);
            var onlineFriends = cachedUserOnline
                .Where(userConn => friends.Any(user => user.Id == userConn.UserId))
                .ToList();
            //  check notify for caller
            var caller = cachedUserOnline.Where(u => u.UserId == id).FirstOrDefault();
            if (caller != null)
            {
                onlineFriends.Add(caller);
            }
            // notify all friend
            foreach (var user in onlineFriends)
            {
                await Clients.Client(user.ConnectionId).SendAsync("ListFriendsOnline", onlineFriends);
            }
        }
    }
}
