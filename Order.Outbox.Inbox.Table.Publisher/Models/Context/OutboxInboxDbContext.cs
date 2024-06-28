using Microsoft.EntityFrameworkCore;
using Order.Outbox.Inbox.Table.Publisher.Models.Entities;

namespace Order.Outbox.Inbox.Table.Publisher.Models.Context
{

    public class OutboxInboxDbContext : DbContext
    {
        public OutboxInboxDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet<OrderOutbox> OrderOutboxes { get; set; }
        public DbSet<OrderInbox> OrderInboxes { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {

        }
    }
}
