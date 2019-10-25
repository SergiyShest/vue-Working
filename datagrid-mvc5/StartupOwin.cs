using System;
using System.Reflection;
using System.Threading.Tasks;
using Autofac;
using Autofac.Integration.SignalR;
using datagrid_mvc5.Controllers;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;
using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(datagrid_mvc5.StartupOwin))]

namespace datagrid_mvc5
{
    public class StartupOwin
    {

        public void Configuration(IAppBuilder app)
        {
            GlobalHost.DependencyResolver.Register(typeof(ChatHub),() => new ChatHub(new ChatRepository()));

            var hubConfiguration = new HubConfiguration();
            hubConfiguration.EnableDetailedErrors = true;
            GlobalHost.Configuration.MaxIncomingWebSocketMessageSize = null;
            //GlobalHost.Configuration.KeepAlive
            app.MapSignalR(hubConfiguration);
            //app.MapSignalR();
        }

        //public void Configuration(IAppBuilder app)
        //{
        //    var builder = new ContainerBuilder();

        //    // STANDARD SIGNALR SETUP:

        //    // Get your HubConfiguration. In OWIN, you'll create one
        //    // rather than using GlobalHost.
        //    var config = new HubConfiguration();

        //    // Register your SignalR hubs.
        //    builder.RegisterHubs(Assembly.GetExecutingAssembly());

        //    // Set the dependency resolver to be Autofac.
        //    var container = builder.Build();
        //    builder.RegisterType<ChatRepository>();
        //  //  builder.RegisterHubs(typeof(StartupOwin).Assembly).EnableClassInterceptors().InterceptedBy(typeof(ChatRepository));
        // //   builder.RegisterControllers(typeof(Startup).Assembly);

        //    config.Resolver = new AutofacDependencyResolver(container);
        //    config.Resolver.Resolve<IChatRepository>();
        //    // OWIN SIGNALR SETUP:

        //    // Register the Autofac middleware FIRST, then the standard SignalR middleware.
        //    // app.UseAutofacMiddleware(container);
        //    app.MapSignalR("/signalr", config);

        //    // To add custom HubPipeline modules, you have to get the HubPipeline
        //    // from the dependency resolver, for example:
        //   // var hubPipeline = config.Resolver.Resolve<IChatRepository>();
        //   // hubPipeline.AddModule(new MyPipelineModule());
        //}
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