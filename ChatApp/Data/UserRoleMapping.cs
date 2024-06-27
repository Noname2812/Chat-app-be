using ChatApp.Data;

namespace WebAPI.Data
{
    public class UserRoleMapping
    {
        public int id { get; set; }
        public int userId { get; set; }
        public int roleId { get; set; }
        public User user { get; set; }
        public Role role { get; set; }
    }
}
