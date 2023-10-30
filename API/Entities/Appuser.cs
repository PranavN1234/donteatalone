using API.Extensions;
using Microsoft.AspNetCore.Identity;

namespace API.Entities;

public class Appuser: IdentityUser<int>
{

    public DateOnly DateofBirth {get; set;}

    public string Knownas { get; set; }

    public DateTime Created { get; set; } = DateTime.UtcNow;

    public DateTime LastActive { get; set; } = DateTime.UtcNow;

    public string Gender { get; set; }

    public string Introduction { get; set; }

    public string Cuisines { get; set; }

    public string City { get; set; }

    public string Country { get; set; }

    public List<Photo> Photos { get; set; } = new List<Photo>(); 

    public List<UserLike> LikedByUsers {get; set;}

    public List<UserLike> LikedUsers { get; set; }

    public List<Messages> MessagesSent {get; set;}

    public List<Messages> MessagesReceived{get; set;}

    public ICollection<AppUserRole> UserRoles { get; set; }

    




}
