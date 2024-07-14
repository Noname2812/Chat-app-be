namespace ChatApp.Models.Request
{
    public class MessageRequest
    {
        public Guid? RoomId { get; set; }
        public string? Message { get; set; }
        public Guid from {  get; set; }
        public Guid? to { get; set; }
        public bool isPrivate { get; set; }
        public string? name { get; set; }
        public string[]? Images { get; set; }

    }
}
