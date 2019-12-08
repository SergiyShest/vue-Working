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
using System.Security.Cryptography;
using System.Text;

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

    public class ChatRepository : IChatRepository
    {
        public void Add(string name, string message) { }
        // Other methods not shown.
    }

    public class ChatHub : Hub
    {
        public ChatHub(IChatRepository ch)
        {

        }

        public ChatHub()
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
                CrDate = DateTime.Now,
                Files = new List<string>() ,
                DeliveryList = new[] {
                        new Delivery() {Status=0,User="VASA" },
                        new Delivery() {Status=0,User="PETA" },
                        new Delivery() {Status=0,User="GLOK" }
                    }
            };

          Clients.Client(Context.ConnectionId).broadcastMessage(mess);

            Messages.Add(mess);

            return mess.Id;
        }

        public List<ChatMessage> GetMessages()
        {
            var mess = new List<ChatMessage>()
            {
                new ChatMessage(){Id = ++counter,SenderName = "PUK",Message = "ssss",CrDate = DateTime.Now.AddDays(-1),
                    Files =new[] {"fff","bla bla bla.txt","mx my .com" } ,
                    DeliveryList= new[] {
                        new Delivery() {Status=1,User="VASA" },
                        new Delivery() {Status=1,User="PETA" },
                        new Delivery() {Status=1,User="GLOK" },
                    } },
                new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = "ssscccs",CrDate = DateTime.Now,
                    Files =new[] {"fff.ee" },
                    DeliveryList= new[] {
                        new Delivery() {Status=1,User="VASA" } ,
                        new Delivery() {Status=0,User="PETA" }
                    }
                }//,
                //new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = @"sstestcs\r\ntest\r\nэээ test ",CrDate = DateTime.Now,
                //    Files =new List <string> (0),
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" } ,
                //        new Delivery() {Status=0,User="PETA" }
                //    }
                //},
                //new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = "еуые test sss",CrDate = DateTime.Now,
                //    Files =new[] {"fff.ee" },
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" } ,
                //        new Delivery() {Status=0,User="PETA" }
                //    }
                //},
                //new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = "Несе Галя воду",CrDate = DateTime.Now,
                //    Files =new List <string> (0),
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" } ,
                //        new Delivery() {Status=0,User="PETA" }
                //    }
                //},
                //new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = "Как ныне сбирается Вещий Олег Щиты прибивать на ворота",CrDate = DateTime.Now,
                //    Files =new List <string> (0),
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" } ,
                //        new Delivery() {Status=0,User="PETA" }
                //    }
                //},
                //new ChatMessage(){Id = ++counter,SenderName = "PUK",Message = "ssss",CrDate = DateTime.Now.AddDays(-1),
                //    Files =new[] {"fff","bla bla bla.txt","mx my .com" } ,
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" },
                //        new Delivery() {Status=1,User="PETA" },
                //        new Delivery() {Status=1,User="GLOK" },
                //    }},
                //new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = "ssscccs",CrDate = DateTime.Now,
                //    Files =new[] {"fff.ee" },
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" } ,
                //        new Delivery() {Status=0,User="PETA" }
                //    }
                //},
                //new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = @"sstestcs\r\ntest\r\nэээ test ",CrDate = DateTime.Now,
                //    Files =new List <string> (0),
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" } ,
                //        new Delivery() {Status=0,User="PETA" }
                //    }
                //},
                //new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = "testеуые test sss test",CrDate = DateTime.Now,
                //    Files =new[] {"fff.ee" },
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" } ,
                //        new Delivery() {Status=0,User="PETA" }
                //    }
                //},
                //new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = "Несе Галя воду",CrDate = DateTime.Now,
                //    Files =new List <string> (0),
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" } ,
                //        new Delivery() {Status=0,User="PETA" }
                //    }
                //},
                //new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = "test.Олег.Как ныне сбирается Вещий Олег Щиты прибивать на ворота",CrDate = DateTime.Now,
                //    Files =new List <string> (0),
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" } ,
                //        new Delivery() {Status=0,User="PETA" }
                //    }
                //},               new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = "Несе Галя воду",CrDate = DateTime.Now,
                //    Files =new List <string> (0),
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" } ,
                //        new Delivery() {Status=0,User="PETA" }
                //    }
                //},               new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = "Несе Галя воду",CrDate = DateTime.Now,
                //    Files =new List <string> (0),
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" } ,
                //        new Delivery() {Status=0,User="PETA" }
                //    }
                //},               new ChatMessage(){Id = ++counter,SenderName = "CLOK",Message = "testНесе Галя воду",CrDate = DateTime.Now,
                //    Files =new List <string> (0),
                //    DeliveryList= new[] {
                //        new Delivery() {Status=1,User="VASA" } ,
                //        new Delivery() {Status=0,User="PETA" }
                //    }
                //}
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

        public void FilesUploaded(int messageId)
        {
            Clients.All.filesUploaded(messageId, new[] { "message", "Status", "sgdfgsdf" });
        }

        public override Task OnConnected()
        {
            string name = Context.User.Identity.Name;
            _connections.Add(name, Context.ConnectionId);
            var named = Clients.Caller.GetName().Result;
            return base.OnConnected();
        }
        public override Task OnReconnected()
        {
            string name = Context.User.Identity.Name;
            var named = Clients.Caller.GetName().Result;
            if (!_connections.GetConnections(name).Contains(Context.ConnectionId))
            {
                _connections.Add(name, Context.ConnectionId);
            }
            return base.OnReconnected();
        }

        public override Task OnDisconnected(bool stopCalled)
        {
            string name = Context.User.Identity.Name;

            _connections.Remove(name, Context.ConnectionId);
            return base.OnDisconnected(stopCalled);
        }


        public string pass()
        {

            HttpClient client = new HttpClient((new HttpClientHandler() { UseDefaultCredentials = true }));
             var c = base.Context.Request.Url.Authority;
            var x = client.GetAsync("http://" + c + "/home/GetPassword").Result;
            var xx = x.Content.ReadAsStringAsync().Result;

            var byteArray = Convert.FromBase64String(xx);
            byte[] unencodedBytes = ProtectedData.Unprotect(byteArray
                , null
                , DataProtectionScope.CurrentUser);
            string utfString = Encoding.UTF8.GetString(unencodedBytes, 0, unencodedBytes.Length);
            return utfString;
        }

    }



    public class ChatMessage
    {
        public int Id { get; set; }

        public string SenderName { get; set; }

        public string Message { get; set; }
        [JsonConverter(typeof(CustomDateTimeConverter))]
        public DateTime CrDate { get; set; }

        public IEnumerable<Delivery> DeliveryList { get; set; }

        public IEnumerable<string> Files { get; set; }

        public int Status { get; set; }

    }

    public class Delivery
    {
        public int Status { get; set; }
        public string User { get; set; }
    }

    class CustomDateTimeConverter : IsoDateTimeConverter
    {
        public CustomDateTimeConverter()
        {
            base.DateTimeFormat = "dd.MMM HH:mm:ss";
        }
    }
}