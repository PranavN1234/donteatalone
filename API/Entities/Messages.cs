using API.Entities;

namespace API;

public class Messages
{
    public int Id { get; set; }

    public int SenderId { get; set; }

    public string SenderUsername { get; set; }

    public Appuser Sender { get; set; }

    public int ReceipientId { get; set; }

    public string ReceipientUsername { get; set; }

    public Appuser Receipient { get; set; }

    public string Content { get; set; }

    public DateTime? DateRead { get; set; }

    public DateTime MessageSent { get; set; } = DateTime.UtcNow;

    public bool SenderDeleted { get; set; }

    public bool ReceipientDeleted { get; set; } 


}
