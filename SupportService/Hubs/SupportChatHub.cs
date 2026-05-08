using Microsoft.AspNetCore.SignalR;

namespace SupportService.Hubs
{
    // SignalR Hub sınıfı, istemciler WebSocket ile bağlanacak
    public class SupportChatHub : Hub
    {
        public async Task JoinTicketChat(string ticketId)
        {
            await Groups.AddToGroupAsync(Context.ConnectionId, ticketId);
            
            await Clients.Group(ticketId).SendAsync("ReceiveSystemMessage", "Sisteme bağlanıldı, canlı destek aktif.");
        }

        public async Task SendMessage(string ticketId, string userId, string message)
        {
            await Clients.Group(ticketId).SendAsync("ReceiveMessage", userId, message);
        }

        public async Task LeaveTicketChat(string ticketId)
        {
            await Groups.RemoveFromGroupAsync(Context.ConnectionId, ticketId);
        }
    }
}