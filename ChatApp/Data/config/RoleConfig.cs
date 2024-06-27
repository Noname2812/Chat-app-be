using ChatApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebAPI.Data.config
{
    internal class RoleConfig : IEntityTypeConfiguration<Role>
    {
        public void Configure(EntityTypeBuilder<Role> builder)
        {
            builder.ToTable("Roles");
            builder.HasKey(x => x.id);
            builder.Property(x => x.id).UseIdentityColumn();
            builder.Property(n => n.roleName).HasMaxLength(250);
            builder.Property(n => n.description).HasMaxLength(200);
            builder.Property(n => n.isActive);
            builder.Property(n => n.createAt);
            builder.Property(n => n.modifiedDate);
        }
    }

}