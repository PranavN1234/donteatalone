using API.DTOs;
using API.data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using API.Interfaces;
using AutoMapper;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController: ControllerBase
{
    private readonly DataContext _context;

    private readonly ITokenService _tokenService;

    private readonly IMapper _mapper;
    public AccountController(DataContext context, ITokenService tokenService, IMapper mapper){
        _tokenService = tokenService;
        _context = context;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){

        if(await UserExists(registerDTO.Username)){
            return BadRequest("Username is taken");
        }

        var user = _mapper.Map<Appuser>(registerDTO);
        using var hmac = new HMACSHA512();

        
        user.UserName = registerDTO.Username.ToLower();
        user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password));
        user.PasswordSalt = hmac.Key;
        

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDTO{
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.isMain)?.Url,
            Knownas = user.Knownas
        };
        
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){
        var user = await _context.Users.Include(p=>p.Photos).SingleOrDefaultAsync(user=> user.UserName == loginDTO.Username);

        if(user == null){
            return Unauthorized("Invalid Username");
        }

        using var hmac = new HMACSHA512(user.PasswordSalt);

        var computedHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(loginDTO.Password));

        for(int i=0; i<computedHash.Length; ++i){
            if(computedHash[i]!=user.PasswordHash[i]){
                return Unauthorized("Incorrect Password");
            }
        }

        return new UserDTO{
            Username = user.UserName,
            Token = _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.isMain)?.Url,
            Knownas = user.Knownas
        };



    }
    private async Task<bool> UserExists(string username){
        return await _context.Users.AnyAsync(user=>user.UserName == username.ToLower());
        
    }
}
