using System.ComponentModel.DataAnnotations;

namespace ChatApp.Data
{
    public class Message
    {
        public int Id { get; set; }
        [MinLength(1)]
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }
        public int UserId { get; set; }
        public User? User { get; set; }
        public int RoomChatId { get; set; }
        public RoomChat? RoomChat { get; set; }
    }
}
