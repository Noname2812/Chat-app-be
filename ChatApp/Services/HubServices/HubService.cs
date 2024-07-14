using ChatApp.Hubs;
using ChatApp.Models;
using ChatApp.Models.Request;
using Microsoft.AspNetCore.SignalR;
using AutoMapper;
using ChatApp.Models.DTOs;
using ChatApp.Data.UnitOfWork;

namespace ChatApp.Services.ChatServices
{
    public class HubService : IHubService
    {
        private readonly IHubContext<ChatHub> _hubContext;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICacheService _cacheService;
        private readonly IMapper _mapper;
        public HubService(IHubContext<ChatHub> hubContext, IUnitOfWork unitOfWork, ICacheService cacheService, IMapper mapper)
        {
            _hubContext = hubContext;
            _unitOfWork = unitOfWork;
            _cacheService = cacheService;
            _mapper = mapper;
        }

        public async Task NotifyAddFriendRequest(FriendShipDTO friendShip)
        {
            var usersOnl = await _cacheService.GetDataByEndpoint<UserConnection>("list-users-online");
            if (usersOnl != null)
            {
                var from = usersOnl.Where(x => x.UserId == friendShip.UserId).FirstOrDefault();
                var to = usersOnl.Where(x => x.UserId == friendShip.FriendId).FirstOrDefault();
                if (from != null && friendShip.Status == "Accepted")
                {
                    await _hubContext.Clients.Client(from.ConnectionId).SendAsync("NotifyAcceptedAddFriendRequest", to);
                    await NotifyFriendsOnline(friendShip.FriendId);
                }
                if (to != null && friendShip.Status == "Pending")
                {
                    await _hubContext.Clients.Client(to.ConnectionId).SendAsync("NotifyRevicedAddFriendRequest", from);
                }
            }
        }

        public async Task SendMessage(MessageRequest req)
        {
            var room = await _unitOfWork.RoomChatRepository.GetItemByQuery(x => x.Id == req.RoomId);
            if (room != null)
            {
                if (room.IsPrivate)
                {
                    var to = await _unitOfWork.UserRoomChatRepository.GetItemByQuery(x => x.RoomChatId == req.RoomId && x.UserId != req.from);
                    if (to != null)
                    {
                        var usersOnl = await _cacheService.GetDataByEndpoint<UserConnection>("list-users-online");
                        var toUser = usersOnl.Where(user => user.UserId == to.UserId).FirstOrDefault();
                        if (toUser != null && toUser.ConnectionId != null)
                        {
                            await _hubContext.Clients.Client(toUser.ConnectionId).SendAsync("ReceiveMessagePrivate", to.RoomChatId.ToString());
                        }
                    }
                }
                else
                {
                    await _hubContext.Clients.Group(room.Name).SendAsync("ReceiveMessageFromGroup", req.RoomId.ToString());
                }
            }
        }
        public async Task NotifyFriendsOnline(int id)
        {

            var cachedUserOnline = await _cacheService.GetDataByEndpoint<UserConnection>("list-users-online");
            var friends = await _unitOfWork.UserRepository.GetFriendsById(id);
            // list friends was Online
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
                var temp = await _unitOfWork.UserRepository.GetFriendsById(user.UserId);
                var friendResult = _mapper.Map<List<FriendDTO>>(temp);
                await _hubContext.Clients.Client(user.ConnectionId).SendAsync("ListFriendsOnline", friendResult);
            }

        }
    }
}
