using MassTransit;
using Order.Outbox.Inbox.Table.Publisher.Models.Entities;
using Quartz;
using Shared.Events;
using System.Text.Json;

namespace Order.Outbox.Inbox.Table.Publisher.Service.Jobs
{
    public class OrderOutboxPublishJob(IPublishEndpoint publishEndpoint) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            if (OrderSingletonDatabase.DataReaderState)
            {
                OrderSingletonDatabase.DataReaderBusy();

                List<OrderOutbox> orderOutboxes = (await OrderSingletonDatabase.QueryAsync<OrderOutbox>($@"SELECT * FROM ORDEROUTBOXES WHERE ISPOCESSED = 0 ORDER BY OCCUREDON ASC")).ToList();

                foreach (var orderOutbox in orderOutboxes)
                {
                    if (orderOutbox.Type == nameof(OrderCreatedEvent))
                    {
                        OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderOutbox.Payload);
                        if (orderCreatedEvent != null)
                        {
                            await publishEndpoint.Publish(orderCreatedEvent);
                            OrderSingletonDatabase.ExecuteAsync($"UPDATE ORDEROUTBOXES SET ISPOCESSED = 1 WHERE IdempotentToken = '{orderOutbox.IdempotentToken}'");
                        }
                    }
                }

                OrderSingletonDatabase.DataReaderReady();
                await Console.Out.WriteLineAsync("Order outbox table checked!");
            }
        }
    }
}
