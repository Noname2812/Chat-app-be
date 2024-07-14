namespace ChatApp.Data.Modals
{
    public class UserRoomChat
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int RoomChatId { get; set; }
        public User? User { get; set; }
        public RoomChat? RoomChat { get; set; }
    }
}
