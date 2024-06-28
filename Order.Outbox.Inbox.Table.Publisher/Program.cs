using MassTransit;
using Microsoft.EntityFrameworkCore;
using Order.Outbox.Inbox.Table.Publisher.Models.Context;
using Order.Outbox.Inbox.Table.Publisher.Service.Jobs;
using Quartz;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddDbContext<OutboxInboxDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("MSSQLServer")));

builder.Services.AddMassTransit(configurator =>
{
    configurator.UsingRabbitMq((context, _configure) =>
    {
        _configure.Host(builder.Configuration["RabbitMQ"]);
    });
});

builder.Services.AddQuartz(configurator =>
{
    JobKey jobKeyOutbox = new("OrderOutboxPublishJob");
    JobKey jobKeyInbox = new("OrderInboxPublishJob");
    configurator.AddJob<OrderOutboxPublishJob>(options => options.WithIdentity(jobKeyOutbox));
    configurator.AddJob<OrderInboxPublishJob>(options => options.WithIdentity(jobKeyInbox));

    TriggerKey triggerKeyOutbox = new("OrderOutboxPublishTrigger");
    configurator.AddTrigger(options => options.ForJob(jobKeyOutbox)
    .WithIdentity(triggerKeyOutbox)
    .StartAt(DateTime.UtcNow)
    .WithSimpleSchedule(builder => builder
        .WithIntervalInSeconds(5)
        .RepeatForever()));

    TriggerKey triggerKeyInbox = new("OrderInboxPublishTrigger");
    configurator.AddTrigger(options => options.ForJob(jobKeyInbox)
    .WithIdentity(triggerKeyInbox)
    .StartAt(DateTime.UtcNow)
    .WithSimpleSchedule(builder => builder
        .WithIntervalInSeconds(7)
        .RepeatForever()));
});

builder.Services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

var host = builder.Build();
host.Run();
