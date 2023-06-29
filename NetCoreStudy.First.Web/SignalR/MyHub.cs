using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;

namespace NetCoreStudy.First.Web.SignalR
{
    public class MyHub:Hub
    {
        [Authorize]
        public Task SendPublicMsg(string msg)
        {
          string connId=  this.Context.ConnectionId;
            string msgToSend = $"{connId}{DateTime.Now}:{msg}";
            var user = this.Clients.User(connId);

            return user.SendAsync("PublicMsgReceived", msgToSend);
           //  this.Clients.All.SendAsync("PublicMsgReceived", msgToSend);
        }

        public override async Task OnDisconnectedAsync(Exception? exception)
        {
            string cid = GetConnectionId();

            await base.OnDisconnectedAsync(exception);
        }

        private string GetConnectionId()
        {
           return this.GetConnectionId();
        }
    }
}
