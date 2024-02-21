using NATS.Client;
using System.Text;

namespace NatsDemo.Api
{
    public class NatsPublisherService : IHostedService
    {
        private readonly IConnection _natsConnection;

        public NatsPublisherService(IConnection natsConnection)
        {
            _natsConnection = natsConnection;
        }
        /// <summary>
        /// This is a NATS service that allows you to see the message being published in the console app. 
        /// NOTE: This is not the consumer of the message.
        /// </summary>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        //public Task StartAsync(CancellationToken cancellationToken)
        //{
        //    var subscription = _natsConnection.SubscribeAsync("auction.created");
        //    subscription.MessageHandler += (sender, args) =>
        //    {
        //        var message = Encoding.UTF8.GetString(args.Message.Data);
        //        Console.WriteLine($"Publish message: {message}");
        //    };
        //    subscription.Start();
        //    Console.WriteLine("Publish 'auction.created'. Waiting to publish messages...");

        //    return Task.CompletedTask;
        //}
        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Subscription for "auction.created" messages
            var createdSubscription = _natsConnection.SubscribeAsync("auction.created");
            createdSubscription.MessageHandler += (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Message.Data);
                Console.WriteLine($"Publish message: {message}");
            };
            createdSubscription.Start();

            // Subscription for "auction.response" messages
            var responseSubscription = _natsConnection.SubscribeAsync("auction.response");
            responseSubscription.MessageHandler += (sender, args) =>
            {
                var message = Encoding.UTF8.GetString(args.Message.Data);
                Console.WriteLine($"Received auction response: {message}");
            };
            responseSubscription.Start();

            Console.WriteLine("Subscribed to 'auction.created' and 'auction.response'. Waiting for messages...");

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _natsConnection.Drain();
            _natsConnection.Close();
            return Task.CompletedTask;
        }
    }
}
