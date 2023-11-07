using API.data;
using AutoMapper;
using AutoMapper.QueryableExtensions;
using Microsoft.EntityFrameworkCore;

namespace API;

public class MessageRepository : IMessageRepository
{   
    private readonly DataContext _context;
    private readonly IMapper _mapper;
    public MessageRepository(DataContext context, IMapper mapper)
    {
         _context = context;
         _mapper = mapper;
    }

    public void addGroup(Group group)
    {
        _context.Groups.Add(group);
    }

    public void AddMessage(Messages message)
    {
       _context.Messages.Add(message);
    }

    public void DeleteMessage(Messages message)
    {
        _context.Messages.Remove(message);
    }

    public async Task<Connection> GetConnection(string connectionId)
    {
        return await _context.Connections.FindAsync(connectionId);
    }

    public async Task<Messages> GetMessage(int id)
    {
        return await _context.Messages.FindAsync(id);
    }

    public async Task<Group> GetMessageGroup(string groupname)
    {
        return await _context.Groups.Include(x => x.Connections)
            .FirstOrDefaultAsync(x=>x.Name == groupname);
    }

    public async Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams)
    {
        var query = _context.Messages.OrderByDescending(x=>x.MessageSent).AsQueryable();

        query = messageParams.Container switch{
            "Inbox"=>query.Where(u=>u.ReceipientUsername == messageParams.Username && u.ReceipientDeleted == false),
            "Outbox"=>query.Where(u=>u.SenderUsername == messageParams.Username && u.SenderDeleted == false),
            _=> query.Where(u=>u.ReceipientUsername == messageParams.Username && u.DateRead == null && u.ReceipientDeleted == false)
        };

        var messages = query.ProjectTo<MessageDTO>(_mapper.ConfigurationProvider);

        return await PagedList<MessageDTO>.CreateAsync(messages, messageParams.PageNumber, messageParams.PageSize);
    }

    public async Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string receipientUsername)
    {
        var messages = await _context.Messages.Include(u=>u.Sender).ThenInclude(p=>p.Photos)
                        .Include(u=>u.Receipient).ThenInclude(p=>p.Photos).Where(m=>m.ReceipientUsername == currentUsername && 
                        m.ReceipientDeleted==false && m.SenderUsername == receipientUsername || m.ReceipientUsername == receipientUsername && m.SenderDeleted == false && m.SenderUsername == currentUsername).OrderBy(m=>m.MessageSent).ToListAsync();

        var unreadMessages = messages.Where(m=>m.DateRead == null && m.ReceipientUsername == currentUsername).ToList();

        if(unreadMessages.Any()){
            foreach(var message in unreadMessages){
                message.DateRead = DateTime.UtcNow;
            }

            await _context.SaveChangesAsync();
        }

        return _mapper.Map<IEnumerable<MessageDTO>>(messages);
    }

    public void removeConnection(Connection connection)
    {
        _context.Connections.Remove(connection);
    }

    public async Task<bool> SaveAllAsync()
    {
        return await _context.SaveChangesAsync()>0;
    }
}
