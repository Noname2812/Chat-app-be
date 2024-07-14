namespace ChatApp.Models.DTOs
{
    public class FriendShipDTO
    {
        public Guid UserId { get; set; }
        public Guid FriendId { get; set; }
        public string? Status { get; set;}
    }
}
