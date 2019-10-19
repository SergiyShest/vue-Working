using System;
using System.Threading.Tasks;
using Microsoft.AspNet.SignalR;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(datagrid_mvc5.StartupOwin))]

namespace datagrid_mvc5
{
    public class StartupOwin
    {
        public void Configuration(IAppBuilder app)
        {
            var hubConfiguration = new HubConfiguration();
             hubConfiguration.EnableDetailedErrors = true;
            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = null;
//GlobalHost.Configuration.KeepAlive
            app.MapSignalR(hubConfiguration);
            //app.MapSignalR();
        }
    }
}
//using Microsoft.Owin;
//using Owin;
//[assembly: OwinStartup(typeof(SignalRChat.Startup))]
//namespace SignalRChat
//{
//    public class Startup
//    {
//        public void Configuration(IAppBuilder app)
//        {
//            // Any connection or hub wire up and configuration should go here
//            app.MapSignalR();
//        }
//    }
//}