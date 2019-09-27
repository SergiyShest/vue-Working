using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace datagrid_mvc5.Controllers
{
    public class ChatHub : Hub
    { 
        static int counter=0;
       static List<ChatMessage> Messages=new List<ChatMessage>();

        public void Send(string messageStr)
        {
            JsonSerializerSettings microsoftDateFormatSettings = new JsonSerializerSettings
            {
                DateFormatHandling = DateFormatHandling.MicrosoftDateFormat
            };

            var sessId = sessionStor.sessionId;
            var httpContext = Context.Request.GetHttpContext();
            //    var mSid=  httpContext.Session.SessionID;
            string name = Context.User.Identity.Name;

            // Call the broadcastMessage method to update clients.
            var mess = new ChatMessage() {Id = ++counter, SenderName = "xxx", Message = messageStr, CrDate = DateTime.Now};
            Clients.All.broadcastMessage(mess);
            Messages.Add(mess);

            foreach (var message in Messages)
            {
                message.Status++;
                Clients.All.changeMessageStatus(message.Id,message.Status);
            }
        }

        public List<ChatMessage> GetMessages()
        {
            return new List<ChatMessage>()
            {
                new ChatMessage(){Id = ++counter,SenderName = "xxx",Message = "ssss"},
                new ChatMessage(){Id = ++counter,SenderName = "xxdd",Message = "ssscccs"},
            };
        }

    }

    public class ChatMessage
    {
        public int Id { get; set; }

public string SenderName { get; set; }

public string Message { get; set; }
[JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime CrDate { get; set; }


public int Status{ get; set;}

    }
    class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "dd.MM.yyyy HH:mm:ss";
        }
    }
}