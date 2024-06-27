using ChatApp.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WebAPI.Data.config
{
    internal class UserTypeConfig : IEntityTypeConfiguration<UserType>
    {
        public void Configure(EntityTypeBuilder<UserType> builder)
        {
            builder.ToTable("UserTypes");
            builder.HasKey(x => x.id);
            builder.Property(x => x.id).UseIdentityColumn();
            builder.Property(n => n.name).IsRequired().HasMaxLength(250);
            builder.Property(n => n.description).HasMaxLength(200);
            builder.Property(n => n.createAt);
            builder.Property(n => n.modifiedDate);
        }
    }
}