namespace ChatApp.Data.Modals
{
    public class UserRoleMapping
    {
        public Guid id { get; set; }
        public Guid userId { get; set; }
        public Guid roleId { get; set; }
        public User user { get; set; }
        public Role role { get; set; }
    }
}
