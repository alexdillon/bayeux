# Bayeux

A .NET implementation of the 
[Bayeux client protocol](https://docs.cometd.org/current/reference/#_bayeux) 
targeting `netstandard2.0`.

Forked from Bayeux by patriksvensson.

New Features:
* Full async/await support when calling Bayeux API operations
* Support for Subscription Extensions
* Support for .NET Standard 2.0

## Example

```csharp
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bayeux;
using Bayeux.Sandbox;

namespace Example
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

                var extensions = new Dictionary<string, object>();
                //extensions.Add("access_token", "abc123");
                // Additional extensions can be added as needed

                // Subscribe to channel.
                await client.Subscribe("/test",
                    message => Console.WriteLine("Message received: {0}", message.Channel),
                    extensions);
            }
        }
    }
}
```
