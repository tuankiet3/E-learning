using Microsoft.AspNetCore.Connections.Features;
using Microsoft.AspNetCore.SignalR;

namespace E_learning.Services.RealTimeChat
{
    public class SignalR : Hub
    {
        private readonly Dictionary<string,List<string>> _conncetion = new();

        public async Task registerConnect(string userID)
        { 
            var connectionId = Context.ConnectionId;
            if (!_conncetion.ContainsKey(userID)) { 
                _conncetion.Add(userID, new List<string>());
                _conncetion[userID].Add(connectionId);
            } else
            {
                var list = _conncetion[userID];
                lock (list)
                {
                    if (!list.Contains(connectionId))
                    {
                        list.Add(connectionId);
                    }
                }
            }
        }

        public async Task removeConnect(string userID)
        {
            var connectionId = Context.ConnectionId;
            if (_conncetion.ContainsKey(userID))
            {
                var list = _conncetion[userID];
                lock (list)
                {
                    if (list.Contains(connectionId))
                    {
                        list.Remove(connectionId);
                    }
                }
                if (list.Count == 0)
                {
                    _conncetion.Remove(userID);
                }
            }
        }
    }
}
