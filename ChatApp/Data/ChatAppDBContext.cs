using ChatApp.Data.config;
using Microsoft.EntityFrameworkCore;
using WebAPI.Data;
using WebAPI.Data.config;
namespace ChatApp.Data
{
    public class ChatAppDBContext : DbContext
    {
        public ChatAppDBContext(DbContextOptions<ChatAppDBContext> options) : base(options)
        {   
            
        }
        public DbSet<Message> Messages { get; set; }
        public DbSet<RoomChat> RoomChats { get; set; }
        public DbSet<UserRoomChat> UserRoomChat {  get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<RolePrivilege> RolePrivileges { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserType> UserTypes { get; set; }
        public DbSet<UserRoleMapping> UserRoles { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.ApplyConfiguration(new MessageConfig());
            modelBuilder.ApplyConfiguration(new RoomChatConfig());
            modelBuilder.ApplyConfiguration(new UserRoomChatConfig());
            modelBuilder.ApplyConfiguration(new UserConfig());
            modelBuilder.ApplyConfiguration(new RoleConfig());
            modelBuilder.ApplyConfiguration(new RolePrivilegeConfig());
            modelBuilder.ApplyConfiguration(new RolePrivilegeConfig());
            modelBuilder.ApplyConfiguration(new UserTypeConfig());

        }
    }
}
