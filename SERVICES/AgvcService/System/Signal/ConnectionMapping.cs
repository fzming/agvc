using System.Collections.Generic;
using System.Linq;
using AgvcService.Users.Models.Messages;

namespace AgvcService.System.Signal
{
    public class ConnectionMapping<T, TConnectionAgent> where TConnectionAgent : ConnectionAgent
    {
         
        private readonly Dictionary<T, HashSet<TConnectionAgent>> _connections =
            new Dictionary<T, HashSet<TConnectionAgent>>();

        public int Count
        {
            get
            {
                lock (_connections)
                {
                    var ret = _connections;
                    return (int) ret?.Count;
                }
            }
        }

        public void Add(T key, TConnectionAgent connection)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(key, out var connections))
                {
                    connections = new HashSet<TConnectionAgent>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.RemoveWhere(p => p.ConnectionId == connection.ConnectionId);
                    connections.Add(connection);
                }
            }
        }

        public IEnumerable<TConnectionAgent> GetConnections(T key)
        {
            lock (_connections)
            {
                return _connections.TryGetValue(key, out var connections) ? connections : Enumerable.Empty<TConnectionAgent>();
            }
        }
        public IEnumerable<TConnectionAgent> GetConnections(T[] keys)
        {
            lock (_connections)
            {
                return _connections.Where(p => keys.Contains(p.Key)).SelectMany(p => p.Value);
            }
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(key, out var connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.RemoveWhere(p => p.ConnectionId == connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
        public void Remove(T key, string[] connectionIds)
        {
            lock (_connections)
            {
                if (!_connections.TryGetValue(key, out var connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.RemoveWhere(p => connectionIds.Contains(p.ConnectionId));

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }

}