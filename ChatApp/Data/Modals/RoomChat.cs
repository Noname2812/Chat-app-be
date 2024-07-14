namespace ChatApp.Data.Modals
{
    public class RoomChat
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime CreatAt { get; set; }
        public DateTime ModifiedDate { get; set; }
        public bool IsPrivate { get; set; }
        public virtual List<Message>? Messages { get; set; }
        public virtual ICollection<UserRoomChat>? UserRoomChat { get; set; }
    }
}
