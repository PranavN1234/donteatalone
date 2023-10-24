using API.Entities;

namespace API;

public class UserLike
{
    public Appuser SourceUser { get; set; }

    public int SourceId { get; set; }  

    public Appuser TargetUser { get; set; }

    public int TargetId { get; set; }
}
