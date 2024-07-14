namespace ChatApp.Data.Modals
{
    public class RolePrivilege
    {
        public int id { get; set; }
        public string rolePrivilegeName { get; set; }
        public string? description { get; set; }
        public int roleId { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public DateTime createAt { get; set; }
        public DateTime modifiedDate { get; set; }
        public Role? role { get; set; }

    }
}
