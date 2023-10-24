namespace API;

public class UserParams: PaginationParams
{

    public String CurrentUsername {get; set;}

    public String Gender { get; set; }

    public int MinAge { get; set; } = 18;

    public int MaxAge {get; set;} = 100;

    public String OrderBy { get; set; } ="lastActive";
}
