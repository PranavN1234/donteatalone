namespace API;

public interface IMessageRepository
{
    void AddMessage(Messages message);

    void DeleteMessage(Messages message);

    Task<Messages> GetMessage(int id);

    Task<PagedList<MessageDTO>> GetMessagesForUser(MessageParams messageParams);

    Task<IEnumerable<MessageDTO>> GetMessageThread(string currentUsername, string receipientUsername);

    Task<bool> SaveAllAsync();
}
