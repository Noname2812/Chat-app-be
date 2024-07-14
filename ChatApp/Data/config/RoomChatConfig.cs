using ChatApp.Data.Modals;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace ChatApp.Data.config
{
    public class RoomChatConfig : IEntityTypeConfiguration<RoomChat>
    {
        public void Configure(EntityTypeBuilder<RoomChat> builder)
        {
            builder.ToTable("RoomChats");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).UseIdentityColumn();
            builder.Property(n => n.Name).IsRequired();
            builder.Property(n => n.IsPrivate).HasDefaultValue(true);
            builder.Property(n => n.CreatAt);
            builder.Property(n => n.ModifiedDate);
        }
    }
}
