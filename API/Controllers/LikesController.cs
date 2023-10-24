using System.Security.Claims;
using API.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]

[Authorize]
public class LikesController: ControllerBase
{   
    private readonly IUserRepository _userRepository;
    private readonly ILikesService _likesService;
    public LikesController(IUserRepository userRepository, ILikesService likeService){
        _likesService = likeService;
        _userRepository = userRepository;
    }

    [HttpPost("{username}")]
    public async Task<ActionResult> AddLike(string username){
        var sourceuserId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        var likedUser = await _userRepository.GetUserByUsernameAsync(username);

        var sourceUser = await _likesService.GetUserWithLikes(sourceuserId);

        if(likedUser == null){
            return NotFound();
        }

        if(sourceUser.UserName == username){
            return BadRequest("You cannot like yourself");
        }

        var userLike = await _likesService.GetUserLike(sourceuserId, likedUser.Id);

        if(userLike!=null){
            return BadRequest("You already liked this user");
        }

        userLike = new UserLike{
            SourceId = sourceuserId,
            TargetId = likedUser.Id
        };

        sourceUser.LikedUsers.Add(userLike);

        if(await _userRepository.SaveAllAsync()){
            return Ok();
        }

        return BadRequest("Failed to like user");
    }

    [HttpGet]

    public async Task<ActionResult<PagedList<LikeDTO>>> GetUserLikes([FromQuery]LikesParams likesParams){

        var userId = int.Parse(User.FindFirst(ClaimTypes.NameIdentifier)?.Value);

        likesParams.UserId = userId;

        var users = await _likesService.GetUserLikes(likesParams);

        Response.AddPaginationHeader(new PaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages));

        return Ok(users);
    }

   
}
