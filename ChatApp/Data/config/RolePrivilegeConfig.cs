using ChatApp.Data.Modals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebAPI.Data.config
{
    internal class RolePrivilegeConfig : IEntityTypeConfiguration<RolePrivilege>
    {
        public void Configure(EntityTypeBuilder<RolePrivilege> builder)
        {
            builder.ToTable("RolePrivileges");
            builder.HasKey(x => x.id);
            builder.Property(x => x.id).UseIdentityColumn();
            builder.Property(n => n.rolePrivilegeName).HasMaxLength(250);
            builder.Property(n => n.description).HasMaxLength(200);
            builder.Property(n => n.isActive);
            builder.Property(n => n.createAt);
            builder.Property(n => n.modifiedDate);
            builder.HasOne(n => n.role).WithMany(k => k.privileges).HasForeignKey(x => x.roleId).HasConstraintName("FK_RolePrivilege_Role");

        }
    }
}