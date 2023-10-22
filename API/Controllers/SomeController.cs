using API.data;
using API.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SQLitePCL;

namespace API;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]
public class SomeController : ControllerBase
{
    private readonly DataContext _context;
    public SomeController(DataContext context)
    {
        _context = context;
    }
    [Authorize]
    [HttpGet("auth")]
    public ActionResult<string> GetSecrets()
    {

        return "secret text";
    }

    [HttpGet("not-found")]
    public ActionResult<Appuser> GetNotFound()
    {

        var thing = _context.Users.Find(-1);
        if(thing==null){
            return NotFound();
        }

        return thing;
    }


    [HttpGet("server-error")]
    public ActionResult<string> GetServerError()
    {
        var thing = _context.Users.Find(-1);
        var thingtoReturn = thing.ToString();

        return thingtoReturn;
    }


    [HttpGet("bad-request")]
    public ActionResult<string> GetBadRequest()
    {

        return BadRequest("This was not a good request");
    }




}
