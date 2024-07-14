namespace ChatApp.Models
{
    public class UserConnection
    {
        public Guid UserId { get; set; }
        public string Name { get; set; }
        public string? Avatar { get; set; }
        public string ConnectionId { get; set; }
    }
}
