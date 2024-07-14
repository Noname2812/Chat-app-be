namespace ChatApp.Models.DTOs
{
    public class FriendShipDTO
    {
        public int UserId { get; set; }
        public int FriendId { get; set; }
        public string? Status { get; set;}
    }
}
