using ChatApp.Data.Modals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Data.config
{
    public class MessageConfig : IEntityTypeConfiguration<Message>
    {
        public void Configure(EntityTypeBuilder<Message> builder)
        {
            builder.ToTable("Messages");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(n => n.CreateAt);
            builder.Property(n => n.Content);
            builder.Property(n => n.ImageUrl);
            builder.HasOne(x => x.RoomChat).WithMany(k => k.Messages).HasForeignKey(x => x.RoomChatId).HasConstraintName("FK_Message_RoomChat");
            builder.HasOne(x => x.User).WithMany(k => k.Messages).HasForeignKey(x => x.UserId).HasConstraintName("FK_Message_User");
        }
    }
}
