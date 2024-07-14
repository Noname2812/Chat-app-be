namespace ChatApp.Data.Modals
{
    public class Friendship
    {
        public int UserId { get; set; }
        public User User { get; set; }
        public int FriendId { get; set; }
        public User Friend { get; set; }
        public string? Status { get; set; } //Pending, Accepted, Rejected
        public DateTime? CreatedAt { get; set; }
    }
}
