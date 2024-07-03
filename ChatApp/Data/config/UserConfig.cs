using ChatApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebAPI.Data.config
{
    public class UserConfig : IEntityTypeConfiguration<User>
    {
        public void Configure(EntityTypeBuilder<User> builder)
        {
            builder.ToTable("Users");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(n => n.UserName).IsRequired().HasMaxLength(30);
            builder.Property(n => n.Password).IsRequired().HasMaxLength(30);
            builder.Property(n => n.Address).HasMaxLength(100);
            builder.Property(n => n.Email);
            builder.Property(n => n.IsOnline).HasDefaultValue(false);
            builder.HasIndex(x => x.Email).IsUnique();
            builder.Property(n => n.Phone).HasMaxLength(11);
            builder.Property(n => n.userTypeId);
            builder.Property(n => n.createAt);
            builder.Property(n => n.modifiedDate);
            builder.Property(n => n.LastOnline);
            builder.Property(n => n.Avatar);
            builder.HasOne(s => s.UserType).WithMany(m => m.Users).HasForeignKey(s => s.userTypeId).HasConstraintName("FK_User_UserTypes");
        }
    }
}
