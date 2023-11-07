using System.Security.Claims;
using API.Interfaces;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;

namespace API;
[Authorize]
public class MessageHub: Hub
{   
    private readonly IMessageRepository _messageRepository;
    private readonly IUserRepository _userRepository;

    private readonly IMapper _mapper;
    public MessageHub(IMessageRepository messageRepository, IUserRepository userRepository, IMapper mapper)
    {
        _messageRepository = messageRepository;
        _userRepository = userRepository;
        _mapper = mapper;
    }

    public override async Task OnConnectedAsync()
    {
        var httpContext = Context.GetHttpContext();
        var otherUser = httpContext.Request.Query["user"];
        var groupName = GetGroupName(Context.User.FindFirst(ClaimTypes.Name)?.Value, otherUser);
        await Groups.AddToGroupAsync(Context.ConnectionId, groupName);

        var messages = await _messageRepository.GetMessageThread(Context.User.FindFirst(ClaimTypes.Name)?.Value, otherUser);

        await Clients.Group(groupName).SendAsync("ReceiveMessageThread", messages);


    }

    public override Task OnDisconnectedAsync(Exception exception)
    {
        return base.OnDisconnectedAsync(exception);
    }

    public async Task SendMessage(CreateMessageDTO createMessageDTO){
        var username = Context.User.FindFirst(ClaimTypes.Name)?.Value;
        
        if(username == createMessageDTO.ReceipientUsername.ToLower()){
            throw new HubException("You cannot send messages to yourself");
        }

        
        var sender = await _userRepository.GetUserByUsernameAsync(username);
        var receipient = await _userRepository.GetUserByUsernameAsync(createMessageDTO.ReceipientUsername);

        if(receipient==null){
            throw new HubException("Not found");
        }

        var message = new Messages{
            Sender=sender,
            Receipient = receipient,
            SenderUsername = sender.UserName,
            ReceipientUsername = receipient.UserName,
            Content = createMessageDTO.Content,

        };

        _messageRepository.AddMessage(message);

        if(await _messageRepository.SaveAllAsync()){
            var group = GetGroupName(sender.UserName, receipient.UserName);

            await Clients.Group(group).SendAsync("NewMessage", _mapper.Map<MessageDTO>(message));
        }

    }
    private string GetGroupName(string caller, string other){
        var stringCompare = string.CompareOrdinal(caller, other)<0;

        return stringCompare?$"{caller}-{other}":$"{other}-{caller}";
    }
}
