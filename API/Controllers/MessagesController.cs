using System.Security.Claims;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API;

[ServiceFilter(typeof(LogUserActivity))]
[ApiController]
[Route("api/[controller]")]

[Authorize]
public class MessagesController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    private readonly IMessageRepository _messageRepository;
    public MessagesController(IUserRepository userRepository, IMapper mapper, IMessageRepository messageRepository)
    {
        _mapper = mapper;
        _userRepository = userRepository;
        _messageRepository = messageRepository;
    }

    [HttpPost]
    public async Task<ActionResult<MessageDTO>> CreateMessage(CreateMessageDTO createMessageDTO){
        var username = User.FindFirst(ClaimTypes.Name)?.Value;
        
        if(username == createMessageDTO.ReceipientUsername.ToLower()){
            return BadRequest("You cannot send messages to yourself");
        }

        
        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var receipient = await _userRepository.GetUserByUsernameAsync(createMessageDTO.ReceipientUsername);

        if(receipient==null){
            return NotFound();
        }

        var message = new Messages{
            Sender=sender,
            Receipient = receipient,
            SenderUsername = sender.UserName,
            ReceipientUsername = receipient.UserName,
            Content = createMessageDTO.Content,

        };

        _messageRepository.AddMessage(message);

        if(await _messageRepository.SaveAllAsync()) return Ok(_mapper.Map<MessageDTO>(message));

        return BadRequest("Failed to send Message");

    }

    [HttpGet]

    public async Task<ActionResult<PagedList<MessageDTO>>> GetMessagesForUser([FromQuery]MessageParams messageParams){

        messageParams.Username = User.FindFirst(ClaimTypes.Name)?.Value;

        var messages = await _messageRepository.GetMessagesForUser(messageParams);

        Response.AddPaginationHeader(new PaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages));

        return messages;
    }

    [HttpGet("threads/{username}")]

    public async Task<ActionResult<IEnumerable<MessageDTO>>> GetMessageThread(string username){
        var currentUsername = User.FindFirst(ClaimTypes.Name)?.Value;

        return Ok(await _messageRepository.GetMessageThread(currentUsername, username));
    }

    [HttpDelete("{id}")]

    public async Task<ActionResult> DeleteResult(int id){
        var username = User.FindFirst(ClaimTypes.Name)?.Value;

        var message = await _messageRepository.GetMessage(id);

        if(message.SenderUsername!=username && message.ReceipientUsername!=username){
            return Unauthorized();
        }

        if(message.SenderUsername == username){
            message.SenderDeleted = true;
        }

        if(message.ReceipientUsername == username){
            message.ReceipientDeleted = true;
        }

        if(message.SenderDeleted && message.ReceipientDeleted){
            _messageRepository.DeleteMessage(message);
        }

        if(await _messageRepository.SaveAllAsync()){
            return Ok();
        }

        return BadRequest("Problem deleting message");
    }

}
