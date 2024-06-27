using ChatApp.Data;
using ChatApp.Models.MessageModels;

namespace ChatApp.Models
{
    public class RoomChatDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatAt { get; set; }
        public List<MessageDTO> Messages { get; set; }

    }
}
