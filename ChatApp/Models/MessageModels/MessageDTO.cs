

namespace ChatApp.Models.MessageModels
{
    public class MessageDTO
    {
        public int Id { get; set; }
        public string Content { get; set; }
        public string Image { get; set; }
        public DateTime CreateAt { get; set; }
        public dynamic User { get; set; }
        public dynamic Room {  get; set; }
    }
}
