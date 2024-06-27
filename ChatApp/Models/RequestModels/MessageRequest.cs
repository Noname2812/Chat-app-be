namespace ChatApp.Models.Request
{
    public class MessageRequest
    {
        public int? RoomId { get; set; }
        public string? Message { get; set; }
        public int from {  get; set; }
        public int? to { get; set; }
        public bool isPrivate { get; set; }
        public string? name { get; set; }
        public string[]? Images { get; set; }

    }
}
