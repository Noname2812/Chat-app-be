namespace ChatApp.Data.Modals
{
    public class UserRoomChat
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid RoomChatId { get; set; }
        public User? User { get; set; }
        public RoomChat? RoomChat { get; set; }
    }
}
