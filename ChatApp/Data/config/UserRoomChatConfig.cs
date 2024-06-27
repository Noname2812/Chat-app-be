using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Data.config
{
    public class UserRoomChatConfig : IEntityTypeConfiguration<UserRoomChat>
    {
        public void Configure(EntityTypeBuilder<UserRoomChat> builder)
        {
            builder.ToTable("UserRoomChats");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.HasIndex(n => new { n.UserId, n.RoomChatId }, "UK_UserRoomChatMapping").IsUnique();
            builder.Property(n => n.UserId).IsRequired();
            builder.Property(n => n.RoomChatId).IsRequired();
            builder.HasOne(n => n.User).WithMany(k => k.UserRoomChat).HasForeignKey(x => x.UserId).HasConstraintName("FK_UserRoomChatMapping_User");
            builder.HasOne(n => n.RoomChat).WithMany(k => k.UserRoomChat).HasForeignKey(x => x.RoomChatId).HasConstraintName("FK_UserRoomChatMapping_RoomChat");
        }
    }
}
