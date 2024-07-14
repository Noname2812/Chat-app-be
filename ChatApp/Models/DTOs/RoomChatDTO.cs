namespace ChatApp.Models.DTOs
{
    public class RoomChatDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatAt { get; set; }
        public bool IsPrivate { get; set; }
        public List<MessageDTO>? Messages { get; set; }
    }
}
