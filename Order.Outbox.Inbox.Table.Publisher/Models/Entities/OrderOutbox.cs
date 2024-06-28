using System;

namespace Order.Outbox.Inbox.Table.Publisher.Models.Entities
{
    public class OrderOutbox
    {
        public Guid IdempotentToken { get; set; }
        public DateTime OccuredOn { get; set; }
        public bool IsProcessed { get; set; }
        public string Type { get; set; }
        public string Payload { get; set; }
    }
}
