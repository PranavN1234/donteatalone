﻿using API.DTOs;
using API.data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using API.Interfaces;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController: ControllerBase
{
    private readonly DataContext _context;

    private readonly ITokenService _tokenService;
    public AccountController(DataContext context, ITokenService tokenService){
        _tokenService = tokenService;
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<UserDTO>> Register(RegisterDTO registerDTO){

        if(await UserExists(registerDTO.Username)){
            return BadRequest("Username is taken");
        }
        using var hmac = new HMACSHA512();

        var user = new Appuser{
            UserName = registerDTO.Username.ToLower(),
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(registerDTO.Password)),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return new UserDTO{
            Username = user.UserName,
            Token = _tokenService.CreateToken(user)
        };
        
    }
    [HttpPost("login")]
    public async Task<ActionResult<UserDTO>> Login(LoginDTO loginDTO){
        var user = await _context.Users.SingleOrDefaultAsync(user=> user.UserName == loginDTO.Username);

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
            Token = _tokenService.CreateToken(user)
        };



    }
    private async Task<bool> UserExists(string username){
        return await _context.Users.AnyAsync(user=>user.UserName == username.ToLower());
    }
}
