using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;

namespace ChatApp.Data.config
{
    public class FriendshipConfig : IEntityTypeConfiguration<Friendship>
    {
        public void Configure(EntityTypeBuilder<Friendship> builder)
        {
            builder.HasKey(f => new { f.UserId, f.FriendId });
            builder.Property(x => x.Status).HasDefaultValue("Pending");
            builder.Property(x => x.CreatedAt);
            builder
              .HasOne(f => f.User)
            .WithMany(u => u.Friends)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);
            builder.HasOne(f => f.Friend)
            .WithMany(u => u.FriendsOf)
            .HasForeignKey(f => f.FriendId)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}