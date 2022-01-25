using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;

namespace Blogs.Hubs
{
    public class NotificationHub : Hub
    {
        public async Task SendNotification(string userId, int status, string title)
        {
            await Clients.User(userId).SendAsync("ReceiveMessage", status, title);
        }
    }
}