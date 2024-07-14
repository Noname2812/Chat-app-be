
using ChatApp.Data.Modals;
using ChatApp.Data.UnitOfWork;
using ChatApp.Models;
using ChatApp.Services;
using ChatApp.Services.ChatServices;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Hubs
{
    public class ChatHub : Hub
    {
        private readonly ChatAppDBContext _dbContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IHubService _hubService;

        public ChatHub(IUnitOfWork unitOfWork, ICacheService cacheService,  IHubService hubService, ChatAppDBContext chatAppDBContext) : base()
        {
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _hubService = hubService;
            _dbContext = chatAppDBContext;
        }
        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            var usersConn = await _cacheService.GetDataByEndpoint<UserConnection>("list-users-online");
            if (usersConn != null)
            {
                var current = usersConn.Where(user => user.ConnectionId == Context.ConnectionId).FirstOrDefault();
                if (current != null)
                {
                    await _unitOfWork.UserRepository.UpdateStatusOnline(current.UserId, false);
                    // remove cache data
                    await _cacheService.RemoveDataByKey($"list-users-online:{current.UserId}");
                    // return list users online
                    await _hubService.NotifyFriendsOnline(current.UserId);
                }
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
            await _unitOfWork.UserRepository.UpdateStatusOnline(user.UserId, true);
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
