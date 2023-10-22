﻿namespace API;

public class UserParams
{
    private const int MaxPageSize = 50;

    public int PageNumber {get; set; } = 1;

    private int _pageSize = 10;

    public int PageSize
    {
        get=> _pageSize;
        set => _pageSize = (value>MaxPageSize)?MaxPageSize:value;
    }
    
    public String CurrentUsername {get; set;}

    public String Gender { get; set; }

    public int MinAge { get; set; } = 18;

    public int MaxAge {get; set;} = 100;

    public String OrderBy { get; set; } ="lastActive";
}
