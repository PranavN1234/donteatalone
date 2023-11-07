using Microsoft.EntityFrameworkCore;
using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
namespace API.data;

public class DataContext : IdentityDbContext<Appuser, AppRole, int, IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>, IdentityRoleClaim<int>, IdentityUserToken<int>>
{
    public DataContext(DbContextOptions options) : base(options)
    {
    }



    public DbSet<UserLike> Likes {get; set;}

    public DbSet<Messages> Messages{get; set;}

    public DbSet<Group> Groups{get; set;}

    public DbSet<Connection> Connections{get; set;}





    protected override void OnModelCreating(ModelBuilder builder){
        base.OnModelCreating(builder);

        builder.Entity<Appuser>().HasMany(ur=>ur.UserRoles).WithOne(u=>u.User).HasForeignKey(ur=>ur.UserId).IsRequired();

        builder.Entity<AppRole>().HasMany(ur=>ur.UserRoles).WithOne(u=>u.Role).HasForeignKey(ur=>ur.RoleId).IsRequired();

        builder.Entity<UserLike>().HasKey(k=>new{k.SourceId, k.TargetId});

        builder.Entity<UserLike>().HasOne(s=>s.SourceUser).WithMany(l=>l.LikedUsers).HasForeignKey(s=>s.SourceId).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<UserLike>().HasOne(s=>s.TargetUser).WithMany(l=>l.LikedByUsers).HasForeignKey(s=>s.TargetId ).OnDelete(DeleteBehavior.Cascade);

        builder.Entity<Messages>().HasOne(u=>u.Receipient).WithMany(m=>m.MessagesReceived).OnDelete(DeleteBehavior.Restrict);

        builder.Entity<Messages>().HasOne(u=>u.Sender).WithMany(m=>m.MessagesSent).OnDelete(DeleteBehavior.Restrict);
    }
}
