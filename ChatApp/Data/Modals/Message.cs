using System.ComponentModel.DataAnnotations;

namespace ChatApp.Data.Modals
{
    public class Message
    {
        public Guid Id { get; set; }
        [MinLength(1)]
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }
        public Guid UserId { get; set; }
        public User? User { get; set; }
        public Guid RoomChatId { get; set; }
        public RoomChat? RoomChat { get; set; }
    }
}
