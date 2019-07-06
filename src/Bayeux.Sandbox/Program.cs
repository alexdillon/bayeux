using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bayeux.Diagnostics;

namespace Bayeux.Sandbox
{
    public class Program
    {
        public static void Main(string[] args)
        {
            System.Threading.Tasks.Task.Run(BayeuxDemoAsync);

            // Wait for exit.
            Console.WriteLine("Press ANY key to quit.");
            Console.ReadKey(true);
        }
        public static async Task BayeuxDemoAsync()
        {
            // Create the client settings.
            var endpoint = new Uri("https://localhost:8000/faye");
            var settings = new BayeuxClientSettings(endpoint)
            {
                Logger = new ConsoleLogger()
            };

            // Create the client.
            using (var client = new BayeuxClient(settings))
            {
                // Connect to server.
                await client.Connect();

                // Subscribe to channel.
                await client.Subscribe("/test",
                    message => Console.WriteLine("Message received: {0}", message.Channel));
            }
        }
    }
}