using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Caching.Distributed;
using PMS.Services.Utility;
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
            _cache=cache;
        }

        public override async Task OnConnectedAsync()
        {
            Debug.WriteLine($"ConnectionId: {Context.ConnectionId} - UserIdentifier: {Context.UserIdentifier}");

            await _cache.SetRecordAsync<int>($"ConnectionId_{Context.ConnectionId}", 1);

            return;
        }
    }
}
