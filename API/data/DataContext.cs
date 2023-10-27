using Microsoft.EntityFrameworkCore;
using API.Entities;
namespace API.data;

public class DataContext : DbContext
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<Appuser> Users { get; set; }

    public DbSet<UserLike> Likes {get; set;}

    public DbSet<Messages> Messages{get; set;}



    protected override void OnModelCreating(ModelBuilder builder){
        base.OnModelCreating(builder);

        builder.Entity<UserLike>().HasKey(k=>new{k.SourceId, k.TargetId});

        builder.Entity<UserLike>().HasOne(s=>s.SourceUser).WithMany(l=>l.LikedUsers).HasForeignKey(s=>s.SourceId).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>().HasOne(s=>s.TargetUser).WithMany(l=>l.LikedByUsers).HasForeignKey(s=>s.TargetId ).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Messages>().HasOne(u=>u.Receipient).WithMany(m=>m.MessagesReceived).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Messages>().HasOne(u=>u.Sender).WithMany(m=>m.MessagesSent).OnDelete(DeleteBehavior.Restrict);
    }
}
