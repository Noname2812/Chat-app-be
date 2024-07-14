namespace ChatApp.Data.Modals
{
    public class Friendship
    {
        public Guid UserId { get; set; }
        public User User { get; set; }
        public Guid FriendId { get; set; }
        public User Friend { get; set; }
        public string? Status { get; set; } //Pending, Accepted, Rejected
        public DateTime? CreatedAt { get; set; }
    }
}
