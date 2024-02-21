using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using NATS.Client;
using NATS.Client.JetStream;
using NatsDemo.Core.Data;
using NatsDemo.Infrastructure.Implementation;
using NatsDemo.Infrastructure.Interface;

namespace NatsDemo.Api
{
    public static class ServiceExtension
    {
        public static void ConfigureConnectionString(this IServiceCollection services, IConfiguration Configuration)
        {

            services.AddDbContext<NatsDemoDbContext>(options =>
            {
                options.UseMySql(connectionString: Configuration.GetConnectionString("ApplicationConnectionString"),
                    serverVersion: ServerVersion.AutoDetect(Configuration.GetConnectionString("ApplicationConnectionString")),
                    mySqlOptionsAction: sqlOptions =>
                {
                    sqlOptions.UseNetTopologySuite();
                    sqlOptions.EnableRetryOnFailure(maxRetryCount: 10, maxRetryDelay: TimeSpan.FromSeconds(30), errorNumbersToAdd: null);
                    sqlOptions.UseQuerySplittingBehavior(QuerySplittingBehavior.SplitQuery);
                });
            });

            var options = ConnectionFactory.GetDefaultOptions();
            options.Url = "nats://localhost:4222";
            options.MaxReconnect = Options.ReconnectForever;
            options.ReconnectWait = 2000;

            var natsConnection = new ConnectionFactory().CreateConnection(options);
            services.AddSingleton<IConnection>(natsConnection);
            services.AddScoped<IAuctionService, AuctionService>();

            services.AddHostedService<NatsPublisherService>();
            CreateJetStreamStream(natsConnection);
        }
        public static void CreateJetStreamStream(IConnection natsConnection)
        {
            // Create JetStream management context
            var jsManagement = natsConnection.CreateJetStreamManagementContext();

            // Stream configuration
            var streamConfig = StreamConfiguration.Builder()
                .WithName("AuctionStream") // Name of the stream
                .WithSubjects("auction.created") // Subjects the stream should cover
                .Build();

            // Try to create the stream
            try
            {
                jsManagement.AddStream(streamConfig);
                Console.WriteLine("Stream created successfully.");
            }
            catch (NATSJetStreamException e)
            {
                // Handle cases where the stream might already exist or other errors
                Console.WriteLine($"Failed to create stream: {e.Message}");
            }
        }


    }
}
