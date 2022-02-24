using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using PMS.DTO;
using PMS.Services.Caching;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PMS.WebFramework.ApplicationHubs
{
    [Authorize]
    public class NotificationHub : Hub
    {
        private readonly IDistributedCache _cache;

        public NotificationHub(IDistributedCache cache)
        {
            _cache = cache;
        }

        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine($"ConnectionId: {Context.ConnectionId} - UserIdentifier: {Context.UserIdentifier}");

            string recordId = $"UserId_{Context.UserIdentifier}";
            var record = await _cache.GetRecordAsync<UserHubConnections>(recordId);
            var obj = new UserHubConnections();

            if (record != null)
            {
                await _cache.RemoveAsync(recordId);
            }

            obj = new UserHubConnections
            {
                UserId = int.Parse(Context.UserIdentifier),
                Connections = new List<string>() { Context.ConnectionId }
            };

            await _cache.SetRecordAsync(recordId, obj);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            string recordId = $"UserId_{Context.UserIdentifier}";
            var record = await _cache.GetRecordAsync<UserHubConnections>(recordId);

            if (record != null)
            {
                await _cache.RemoveAsync(recordId);
            }
        }

        public void SendMessage(string message)
        {

        }
    }
}
