using MassTransit;
using Order.Outbox.Inbox.Table.Publisher.Service;
using Order.Outbox.Inbox.Table.Publisher.Models.Entities;
using Quartz;
using Shared.Events;
using System.Text.Json;

namespace Order.Outbox.Inbox.Table.Publisher.Service.Jobs
{
    public class OrderInboxPublishJob(IPublishEndpoint publishEndpoint) : IJob
    {
        public async Task Execute(IJobExecutionContext context)
        {
            if (OrderSingletonDatabase.DataReaderState)
            {
                OrderSingletonDatabase.DataReaderBusy();
               
                List<OrderInbox> orderInboxes = (await OrderSingletonDatabase.QueryAsync<OrderInbox>($@"SELECT * FROM ORDERINBOXES WHERE ISPROCESSED = 0")).ToList();

                foreach (var orderOutbox in orderInboxes)
                {                  
                    OrderCreatedEvent orderCreatedEvent = JsonSerializer.Deserialize<OrderCreatedEvent>(orderOutbox.Payload);
                    if (orderCreatedEvent != null)
                    {
                        await publishEndpoint.Publish(orderCreatedEvent);
                        OrderSingletonDatabase.ExecuteAsync($"UPDATE ORDERINBOXES SET ISPROCESSED = 0 WHERE IdempotentToken = '{orderOutbox.IdempotentToken}'");
                        Console.WriteLine($"Stock operations of the order corresponding to the order id value {orderCreatedEvent.OrderId} have been successfully completed.");
                    }                   
                }

                OrderSingletonDatabase.DataReaderReady();
                await Console.Out.WriteLineAsync("Order outbox table checked!");
            }
        }
    }
}
