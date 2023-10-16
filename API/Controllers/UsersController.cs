
using System.Security.Claims;
using API.data;
using API.DTOs;
using API.Entities;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace API.Controllers;

[ApiController]
[Route("api/[controller]")]

[Authorize]
public class UsersController: ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    private readonly IPhotoService _photoService;
    public UsersController(IUserRepository userRepository, IMapper mapper, IPhotoService photoService)
    {   
        _mapper = mapper;
        _userRepository = userRepository;
        _photoService = photoService;
    }
    
    
    [HttpGet]
    public async Task<ActionResult<IEnumerable<MemberDTO>>> GetUsers(){
        
        var users = await _userRepository.GetMembersAsync();


        return Ok(users);
    }

    [HttpGet("{username}")]
    public async Task<ActionResult<MemberDTO>> GetUser(string username){
        
        return await _userRepository.GetMemberAsync(username);
    }

    [HttpPut]

    public async Task<ActionResult> UpdateUser(MemberUpdateDTO memberUpdateDTO){
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

        var user = await _userRepository.GetUserByUsernameAsync(username);

        if(user == null) return NotFound();

        _mapper.Map(memberUpdateDTO, user);

        if(await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Failed to update user");


    }

    [HttpPost("add-photo")]
    public async Task<ActionResult<PhotoDTO>> AddPhoto(IFormFile file){
         var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

         var user = await _userRepository.GetUserByUsernameAsync(username);

         if(user == null) return NotFound();

         var result = await _photoService.AddPhotoAsync(file);

         if(result.Error!=null){
            return BadRequest(result.Error.Message);
         }

         var photo = new Photo{
            Url = result.SecureUrl.AbsoluteUri, 
            publicId = result.PublicId
         };

         if(user.Photos.Count == 0) photo.isMain = true;

         user.Photos.Add(photo);

         if(await _userRepository.SaveAllAsync()){
            return CreatedAtAction(nameof(GetUser), new {username = user.UserName}, _mapper.Map<PhotoDTO>(photo));
         }

         return BadRequest("Problem adding photo");
    }

    [HttpPut("set-main-photo/{photoId}")]
    public async Task<ActionResult> SetMainPhoto(int photoId){

        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userRepository.GetUserByUsernameAsync(username);

        if(user==null) return NotFound();

        var photo = user.Photos.FirstOrDefault(x=>x.Id==photoId);

        if(photo==null) return NotFound();

        if(photo.isMain){
            return BadRequest("Photo already main");
        }

        var currentMain = user.Photos.FirstOrDefault(x=>x.isMain);

        if(currentMain!=null){
            currentMain.isMain = false;
        }

        photo.isMain = true;

        if(await _userRepository.SaveAllAsync()) return NoContent();

        return BadRequest("Error setting main photo");
    }

    [HttpDelete("delete-photo/{photoId}")]

    public async Task<ActionResult> DeletePhoto(int photoId){
        var username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        var user = await _userRepository.GetUserByUsernameAsync(username);

        var photo = user.Photos.FirstOrDefault(x=>x.Id == photoId);

        if(photo==null) return NotFound();

        if(photo.isMain) return BadRequest("This is main photo, change main before deleting");

        if(photo.publicId!=null){
            var result = await _photoService.DeletePhotoAsync(photo.publicId);
            if(result.Error!=null){
                return BadRequest(result.Error.Message);
            }
        }

        user.Photos.Remove(photo);

        if(await _userRepository.SaveAllAsync()) return Ok();

        return BadRequest("Problem deleting the image");
    }


}
