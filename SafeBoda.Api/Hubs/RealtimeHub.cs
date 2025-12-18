using Microsoft.AspNetCore.SignalR;

namespace SafeBoda.Api.Hubs
{
    public class RealtimeHub : Hub
    {
        // Optional: add methods for client-to-server calls
        public async Task SendMessage(string user, string message)
        {
            await Clients.All.SendAsync("ReceiveMessage", user, message);
        }
    }
}