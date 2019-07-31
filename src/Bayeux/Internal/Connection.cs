using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bayeux.Diagnostics;

namespace Bayeux.Internal
{
    internal sealed class Connection
    {
        private readonly Broker _broker;
        private readonly ConnectionHeartbeat _heartbeat;

        public Connection(Uri endpoint, MessageQueue queue, IEnumerable<BayeuxProtocolExtension> extensions, IBayeuxLogger logger)
        {
            _broker = new Broker(new LongPollingTransport(endpoint, logger), extensions);
            _heartbeat = new ConnectionHeartbeat(_broker, queue);
        }

        public bool IsHeartbeatRunning => this._heartbeat.IsRunning;

        public async Task Connect()
        {
            if (!_heartbeat.IsRunning)
            {
                // Send handshake to server.
                var handshake = await _broker.SendHandshake();
                if (handshake?.Response == null || !handshake.Response.Successful)
                {
                    var message = $"Could not connect to server. {handshake?.Response?.Error}".Trim();
                    throw new InvalidOperationException(message);
                }

                // Update the client ID.
                _broker.SetClientId(handshake.Response.ClientId);

                // Create a new cancellation token source.
                var context = new ConnectionHeartbeatContext(handshake.Response.ClientId);
                _heartbeat.Start(context);
            }
        }

        public void Disconnect()
        {
            if (_heartbeat.IsRunning)
            {
                _heartbeat.Stop();
                _broker.SetClientId(null);
            }
        }

        public async Task Subscribe(string channel, Dictionary<string, object> extensions)
        {
            if (!_heartbeat.IsRunning)
            {
                throw new InvalidOperationException("Not connected to server.");
            }
            await _broker.SendSubscribe(channel, extensions);
        }
    }
}
