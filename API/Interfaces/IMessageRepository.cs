namespace API;

public interface IMessageRepository
{
    void AddMessage(Messages message);

    void DeleteMessage(Messages message);

    Task<Messages> GetMessage(int id);

    Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams);

    Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string receipientUsername);

    Task<bool> SaveAllAsync();

    void addGroup(Group group);

    void removeConnection(Connection connection);

    Task<Connection> GetConnection(string connectionId);

    Task<Group> GetMessageGroup(string groupname);
}
