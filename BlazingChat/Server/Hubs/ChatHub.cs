using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlazingChat.Shared.Models;
using Microsoft.AspNetCore.SignalR;

namespace BlazingChat.Server.Hubs
{
    public class ChatHub : Hub
    {
        public async Task SendMessage(MessagePack messagePack)
        {
            await Clients.All.SendAsync("ReceiveMessage", messagePack);
        }
    }
}
