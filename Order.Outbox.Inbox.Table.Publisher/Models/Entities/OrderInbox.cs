using System.ComponentModel.DataAnnotations;

namespace Order.Outbox.Inbox.Table.Publisher.Models.Entities
{
    public class OrderInbox
    {
        [Key]
        public Guid IdempotentToken { get; set; }
        public bool IsProcessed { get; set; }
        public string Payload { get; set; }
    }
}
