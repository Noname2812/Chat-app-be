using ChatApp.Models.DTOs;
using ChatApp.Models.Request;

namespace ChatApp.Services.ChatServices
{
    public interface IHubService
    {
        Task SendMessage(MessageRequest req);
        Task NotifyAddFriendRequest(FriendShipDTO friendShip);
        Task NotifyFriendsOnline(int id);
    }
}
