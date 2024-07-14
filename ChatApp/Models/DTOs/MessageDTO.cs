

namespace ChatApp.Models.DTOs
{
    public class MessageDTO
    {
        public Guid Id { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }
        public Guid UserId { get; set; }
        public Guid RoomChatId { get; set; }
    }
}
