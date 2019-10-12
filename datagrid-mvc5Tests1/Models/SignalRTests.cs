using System;
using NUnit;

using Microsoft.AspNet.SignalR.Hubs;
using Moq;
using System.Dynamic;
using NUnit.Framework;

using datagrid_mvc5.Controllers;

namespace TestLibrary
{
    [TestFixture]
    public class Tests
    {
        [Test]
        public void HubsAreMockableViaDynamic()
        {
            bool sendCalled = false;
            var hub = new ChatHub();
            var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
            hub.Clients = mockClients.Object;
            dynamic all = new ExpandoObject();

            all.broadcastMessage = new Action<string, string>((name, message) => {
                sendCalled = true;
            });

            mockClients.Setup(m => m.All).Returns((ExpandoObject)all);
            hub.GetMessages();
            Assert.True(sendCalled);
        }

        [Test]
        public void HubsClientIsMockable()
        {
            var hub = new ChatHub();
            hub.Context = new HubCallerContext(null,"connId");
            var mockClients = new Mock<IHubCallerConnectionContext<dynamic>>();
            var clients = new mockClient();

            hub.Clients = mockClients.Object;
         //   clients.Setup(m => m.send(It.IsAny<string>())).Verifiable();
            mockClients.Setup(m => m.Client(It.IsAny<string>())).Returns(clients);
            hub.Send("random");

            clients.VerifyAll();
        }

        public class mockClient{

            public void  Client(string connId)
            {

            }

            ChatMessage _chatMessage;
            public void broadcastMessage(ChatMessage chatMessage)
            {
                _chatMessage = chatMessage;
            }
            internal void VerifyAll()
            {
                if (_chatMessage == null)
                {
                    throw new NotImplementedException();
                }
            }
        }
    }
}