using API.DTOs;
using API.data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
namespace API.Controllers;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]
public class AccountController: ControllerBase
{
    private readonly UserManager<Appuser> _userManager;

    private readonly ITokenService _tokenService;

    private readonly IMapper _mapper;
    public AccountController(UserManager<Appuser> userManager, ITokenService tokenService, IMapper mapper){
        _tokenService = tokenService;
        _userManager = userManager;
        _mapper = mapper;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){

        if(await UserExists(registerDTO.Username)){
            return BadRequest("Username is taken");
        }

        var user = _mapper.Map<Appuser>(registerDTO);
       
        
        user.UserName = registerDTO.Username.ToLower();

        

        var result = await _userManager.CreateAsync(user, registerDTO.Password);

        if(!result.Succeeded){
            return BadRequest(result.Errors);
        }

        var roleResult = await _userManager.AddToRoleAsync(user, "Member");

        if(!roleResult.Succeeded){
            return BadRequest(result.Errors);
        }
        return new UserDTO{
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.isMain)?.Url,
            Knownas = user.Knownas,
            Gender = user.Gender

        };
        
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){
        var user = await _userManager.Users.Include(p=>p.Photos).SingleOrDefaultAsync(user=> user.UserName == loginDTO.Username);

        if(user == null){
            return Unauthorized("Invalid Username");
        }

        var result = await _userManager.CheckPasswordAsync(user, loginDTO.Password);

        if(!result) return Unauthorized("Invalid Password");


        return new UserDTO{
            Username = user.UserName,
            Token = await _tokenService.CreateToken(user),
            PhotoUrl = user.Photos.FirstOrDefault(x=>x.isMain)?.Url,
            Knownas = user.Knownas,
            Gender = user.Gender
        };



    }
    private async Task<bool> UserExists(string username){
        return await _userManager.Users.AnyAsync(user=>user.UserName == username.ToLower());
        
    }
}
