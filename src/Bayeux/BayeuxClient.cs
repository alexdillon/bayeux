using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Bayeux.Internal;

namespace Bayeux
{
    public sealed class BayeuxClient : IDisposable
    {
        private readonly Connection _connection;
        private readonly MessageRouter _router;

        public BayeuxClient(Uri endpoint)
            : this(new BayeuxClientSettings(endpoint))
        {
        }

        public BayeuxClient(BayeuxClientSettings settings)
        {
            var queue = new MessageQueue();

            _connection = new Connection(settings.Endpoint, queue, settings.Extensions, settings.Logger);
            _router = new MessageRouter(queue);
        }

        public bool IsHeartbeatConnected => this._connection.IsHeartbeatRunning;

        void IDisposable.Dispose()
        {
            Disconnect();
        }

        public async Task Connect()
        {
            await _connection.Connect();
            _router.Start();
            _router.ClearAllSubscriptions();
        }

        public void Disconnect()
        {
            _connection.Disconnect();
            _router.Stop();
        }

        public async Task Subscribe(string channel, Action<IBayeuxMessage> callback, Dictionary<string, object> extensions)
        {
            await _connection.Subscribe(channel, extensions);
            _router.Subscribe(channel, callback);
        }
    }
}
