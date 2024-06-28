using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Outbox.Inbox.Table.Publisher.Models.Context;
using Shared.Events;
using System.Text.Json;

namespace Stock.Service.Consumers
{
    public class OrderCreatedEventConsumer(OutboxInboxDbContext orderDbContext) : IConsumer<OrderCreatedEvent>
    {
        public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
        {
            var result = await orderDbContext.OrderInboxes.AnyAsync(i => i.IdempotentToken == context.Message.IdempotentToken);
            if (!result)
            {

                await orderDbContext.OrderInboxes.AddAsync(new()
                {
                    IsProcessed = false,
                    Payload = JsonSerializer.Serialize(context.Message),
                    IdempotentToken = context.Message.IdempotentToken
                });

                await orderDbContext.SaveChangesAsync();
            }
    
        }
    }
}
