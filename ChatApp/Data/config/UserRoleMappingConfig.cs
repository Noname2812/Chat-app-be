using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebAPI.Data.config
{
    internal class UserRoleMappingConfig : IEntityTypeConfiguration<UserRoleMapping>
    {
        public void Configure(EntityTypeBuilder<UserRoleMapping> builder)
        {
            builder.ToTable("UserRoleMappings");
            builder.HasKey(x => x.id);
            builder.Property(x => x.id).UseIdentityColumn();
            builder.HasIndex(n => new { n.userId, n.roleId }, "UK_UserRoleMapping").IsUnique();
            builder.Property(n => n.userId).IsRequired();
            builder.Property(n => n.roleId).IsRequired();
            builder.HasOne(n => n.role).WithMany(k => k.UserRoleMappings).HasForeignKey(x => x.roleId).HasConstraintName("FK_UserRoleMapping_Role");
            builder.HasOne(n => n.user).WithMany(k => k.UserRoleMappings).HasForeignKey(x => x.userId).HasConstraintName("FK_UserRoleMapping_User");
        }
    }
}