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

    public async Task<IEnumerable<MemberDTO>> GetMembersAsync()
    {
        return await _context.Users.ProjectTo<MemberDTO>(_mapper.ConfigurationProvider).ToListAsync();
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
