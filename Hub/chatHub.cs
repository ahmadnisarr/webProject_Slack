using Microsoft.AspNetCore.SignalR;
using WebProject.Models;

namespace WebProject
{
    public class chatHub:Hub
    {
        public async Task SendMessage(string senderId, string receiverId, string messageText)
        {
            //var senderId = Context.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (senderId == null) throw new HubException("User not authenticated");

            messages newMessage = new messages
            {
                SenderId = senderId,
                ReceiverId = receiverId,
                Content = messageText,
                Timestamp = DateTime.UtcNow
            };

            string connectionString = "Data Source = (localdb)\\MSSQLLocalDB;Initial Catalog = YammerDB; Integrated Security = True;";
            IRepository<messages> repo = new GenericRepository<messages>(connectionString);
            repo.Add(newMessage);

            await Clients.User(receiverId).SendAsync("ReceiveMessage", senderId, receiverId, messageText);


        }
    }
}
