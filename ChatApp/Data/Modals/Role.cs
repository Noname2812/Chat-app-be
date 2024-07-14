namespace ChatApp.Data.Modals
{
    public class Role
    {
        public Guid id { get; set; }
        public string roleName { get; set; }
        public string? description { get; set; }
        public bool isActive { get; set; }
        public bool isDeleted { get; set; }
        public DateTime createAt { get; set; }
        public DateTime modifiedDate { get; set; }
        public virtual ICollection<RolePrivilege>? privileges { get; set; }
        public virtual ICollection<UserRoleMapping>? UserRoleMappings { get; set; }

    }
}
