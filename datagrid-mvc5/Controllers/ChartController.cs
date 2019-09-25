using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace datagrid_mvc5.Controllers
{
    public class ChatHub : Hub
    {
        public void Send(string name, string message)
        {
            var sessId = sessionStor.sessionId;
            var httpContext = Context.Request.GetHttpContext();
            //    var mSid=  httpContext.Session.SessionID;
            string name1 = Context.User.Identity.Name;

            // Call the broadcastMessage method to update clients.
            Clients.All.broadcastMessage(sessId, message);
        }


        public List<ChatMessage> GetMessages()
        {
            return new List<ChatMessage>()
            {
                new ChatMessage(){Id = 1,SenderName = "xxx",Message = "ssss"},
                new ChatMessage(){Id = 1,SenderName = "xxdd",Message = "ssscccs"},
            };
        }

    }

    public class ChatMessage
    {
        public int Id { get; set; }

public string SenderName { get; set; }

public string Message { get; set; }

    }

}