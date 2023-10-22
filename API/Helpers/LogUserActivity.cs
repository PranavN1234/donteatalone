using Microsoft.AspNetCore.Mvc.Filters;

using System.Security.Claims;
using API.data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;


namespace API;

public class LogUserActivity : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        var resultContext = await next();

        if(!resultContext.HttpContext.User.Identity.IsAuthenticated) return;

        var userId = resultContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var repo = resultContext.HttpContext.RequestServices.GetRequiredService<IUserRepository>();
        
        var user = await repo.GetUserByIdAsync(int.Parse(userId));

        user.LastActive = DateTime.UtcNow;

        await repo.SaveAllAsync();
    }
}
