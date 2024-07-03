namespace ChatApp.Models.DTOs
{
    public class FriendDTO
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public DateTime LastOnline { get; set;}
        public bool IsOnline { get; set;}
    }
}
