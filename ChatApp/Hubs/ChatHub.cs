
using AutoMapper;
using ChatApp.Data;
using ChatApp.Data.Repository.Users;
using ChatApp.Models;
using ChatApp.Models.DTOs;
using ChatApp.Services;
using ChatApp.Services.ChatServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly IUserRepository _userRepository;
        private readonly ChatAppDBContext _dbContext;
        private readonly ICacheService _cacheService;
        private readonly IHubService _hubService;

        public ChatHub(IUserRepository userRepository, ChatAppDBContext dbContext, ICacheService cacheService,  IHubService hubService) : base()
        {
            _userRepository = userRepository;
            _dbContext = dbContext;
            _cacheService = cacheService;
            _hubService = hubService;
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var usersConn = await _cacheService.GetDataByEndpoint<UserConnection>("list-users-online");
            var current = usersConn.Where(user => user.ConnectionId == Context.ConnectionId).FirstOrDefault();
            if (current != null)
            {
                await _userRepository.UpdateStatusOnline(current.UserId, false);
                // remove cache data
                await _cacheService.RemoveDataByKey($"list-users-online:{current.UserId}");
                // return list users online
                await _hubService.NotifyFriendsOnline(current.UserId);
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
            // update database
            await _userRepository.UpdateStatusOnline(user.UserId, true);
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
            await _hubService.NotifyFriendsOnline(user.UserId);
        }
    }
}
