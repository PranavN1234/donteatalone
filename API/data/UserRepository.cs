using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API.data;

public class UserRepository : IUserRepository
{   
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public UserRepository(DataContext context, IMapper mapper)
    {
        _context = context;
        _mapper = mapper;
    }

    public async Task<MemberDTO> GetMemberAsync(string name)
    {
        return await _context.Users.Where(x=>x.UserName == name).ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).SingleOrDefaultAsync();
    }

    public async Task<PagedList<MemberDTO>> GetMembersAsync(UserParams userParams)
    {   
        var query = _context.Users.AsQueryable();

        query = query.Where(u=>u.UserName!=userParams.CurrentUsername);
        query = query.Where(u=>u.Gender == userParams.Gender);

        var minDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MaxAge-1));
        var maxDob = DateOnly.FromDateTime(DateTime.Today.AddYears(-userParams.MinAge));

        query = query.Where(u=>u.DateofBirth>=minDob && u.DateofBirth<=maxDob);

        query = userParams.OrderBy switch{
            "created"=>query.OrderByDescending(u=>u.Created),
            _=>query.OrderByDescending(u=>u.LastActive)
        };
        
        return await PagedList<MemberDTO>.CreateAsync(query.AsNoTracking().ProjectTo<MemberDTO>(_mapper.ConfigurationProvider), userParams.PageNumber, userParams.PageSize);
    }

    public async Task<Appuser> GetUserByIdAsync(int id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<Appuser> GetUserByUsernameAsync(string name)
    {
        return await _context.Users.Include(p=>p.Photos).SingleOrDefaultAsync(x=>x.UserName == name);
    }

    public async Task<IEnumerable<Appuser>> GetUsersAsync()
    {
        return await _context.Users.Include(p=>p.Photos).ToListAsync();
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync()>0;
    }

    public void Update(Appuser user)
    {
        _context.Entry(user).State = EntityState.Modified;
    }
}
