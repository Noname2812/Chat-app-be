using ChatApp.Data;
using System.ComponentModel.DataAnnotations;

namespace ChatApp.Models.DTOs
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string? Content { get; set; }
        public string? ImageUrl { get; set; }
        public DateTime CreateAt { get; set; }
        public int UserId { get; set; }
        public int RoomChatId { get; set; }
    }
}
