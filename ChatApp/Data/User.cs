using WebAPI.Data;


namespace ChatApp.Data
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public string? Address { get; set; }
        public string Password { get; set; }
        public string UserName { get; set; }
        public string? Avatar {  get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpired { get; set; }

        public DateTime? createAt { get; set; }
        public DateTime? modifiedDate { get; set; }
        public DateTime? LastOnline { get; set; }
        public int userTypeId { get; set; }
        public virtual UserType? UserType { get; set; }
        public virtual ICollection<UserRoomChat>? UserRoomChat { get; set; }
        public virtual ICollection<Message>? Messages { get; set; }
        public virtual ICollection<UserRoleMapping>? UserRoleMappings { get; set; }
        public ICollection<Friendship> Friends { get; set; }
        public ICollection<Friendship> FriendsOf { get; set; }
    }
}
