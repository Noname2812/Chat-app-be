
using ChatApp.Models.DTOs;

namespace ChatApp.Models.ResponeModels
{
    public class SearchUserResult
    {
        public int Id { get; set; }
        public string? Avatar { get; set; }
        public FriendShipDTO? FriendShip { get; set; }
        public string? Name { get; set; }
    }
}
