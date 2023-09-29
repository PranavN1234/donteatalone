using API.data;
using API.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AccountController: ControllerBase
{
    private readonly DataContext _context;
    public AccountController(DataContext context){
        _context = context;
    }

    [HttpPost("register")]
    public async Task<ActionResult<Appuser>> Register(string username, string password){
        using var hmac = new HMACSHA512();

        var user = new Appuser{
            UserName = username,
            PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes(password)),
            PasswordSalt = hmac.Key
        };

        _context.Users.Add(user);
        await _context.SaveChangesAsync();

        return user;
        
    }
}
