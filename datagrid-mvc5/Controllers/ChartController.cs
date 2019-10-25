using Microsoft.AspNet.SignalR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System.Threading.Tasks;

namespace datagrid_mvc5.Controllers
{
    public class ConnectionMapping<T>
    {
        private readonly Dictionary<T, HashSet<string>> _connections =
            new Dictionary<T, HashSet<string>>();

        public int Count
        {
            get
            {
                return _connections.Count;
            }
        }

        public void Add(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    connections = new HashSet<string>();
                    _connections.Add(key, connections);
                }

                lock (connections)
                {
                    connections.Add(connectionId);
                }
            }
        }

        public IEnumerable<string> GetConnections(T key)
        {
            HashSet<string> connections;
            if (_connections.TryGetValue(key, out connections))
            {
                return connections;
            }

            return Enumerable.Empty<string>();
        }

        public void Remove(T key, string connectionId)
        {
            lock (_connections)
            {
                HashSet<string> connections;
                if (!_connections.TryGetValue(key, out connections))
                {
                    return;
                }

                lock (connections)
                {
                    connections.Remove(connectionId);

                    if (connections.Count == 0)
                    {
                        _connections.Remove(key);
                    }
                }
            }
        }
    }

    public interface IClientContract
    {
        void broadcastMessage(string mess);
        void changeMessageStatus(int Id, int Status);
    }
    public interface IChatRepository
    {
        void Add(string name, string message);
        // Other methods not shown.
    }

    public class ChatRepository:IChatRepository
    {
       public void Add(string name, string message) { }
        // Other methods not shown.
    }

    public class ChatHub : Hub
    {
public  ChatHub(IChatRepository ch)
        {

        }
        private readonly static ConnectionMapping<string> _connections = new ConnectionMapping<string>();

        static int counter = 0;
        static List<ChatMessage> Messages = new List<ChatMessage>();

        public int Send(string messageStr)
        { //throw new Exception("dfas");

            var sessId = Context.ConnectionId;
            //var httpContext = Context.Request.GetHttpContext();
            //var mSid=  httpContext.Session.SessionID;
            //string name = Context.User.Identity.Name;
            // Call the broadcastMessage method to update clients.
            var mess = new ChatMessage()
            {
                Id = ++counter,
                SenderName = sessId.Substring(0, 5),
                Message = messageStr,
                CrDate = DateTime.Now
            };
            


            Clients.Client(Context.ConnectionId).broadcastMessage(mess);

            Messages.Add(mess);

            foreach (var message in Messages)
            {
                //    message.Status++;
                //    Clients.All.changeMessageStatus(message.Id,message.Status);
            }
             return mess.Id;
        }

        public List<ChatMessage> GetMessages()
        {
            var mess = new List<ChatMessage>()
            {
                new ChatMessage(){Id = ++counter,SenderName = "жну",Message = "ssss",CrDate = DateTime.Now.AddDays(-1),Files=new {"fff"} },
                new ChatMessage(){Id = ++counter,SenderName = "xxdd",Message = "ssscccs",CrDate = DateTime.Now},

            };
            Messages.AddRange(mess);
            return mess;
        }

        public void SetMessageReaded(int messageId)
        {
            var message = Messages.Find(m => m.Id == messageId);
            message.Status = 3;
            Clients.All.changeMessageStatus(message.Id, message.Status);
        }

       public void  FilesUploaded(int messageId)
        {
        Clients.All.filesUploaded(messageId,new[] { "message","Status","sgdfgsdf"});
        }

        public override Task OnConnected()
        {
            // My code OnConnected
            var name = Clients.Caller.GetName().Result;
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            return base.OnReconnected();
        }

    }



    public class ChatMessage
    {
        public int Id { get; set; }

        public string SenderName { get; set; }

        public string Message { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime CrDate { get; set; }

      IEnumerable<string>  Files{ get; set; }
        public int Status { get; set; }

    }
    class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "dd.MMM HH:mm:ss";
        }
    }
}