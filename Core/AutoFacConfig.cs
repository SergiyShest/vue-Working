using Autofac;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace datagrid_mvc5.App_Start
{
    public static class AutoFacConfig
    {

     public static ContainerBuilder Builder { get; set; }
         static  AutoFacConfig()
        {
            Builder = new ContainerBuilder();
            var domainAssembly = GetAssemblyByName("Domain");
           var repAssembly = GetAssemblyByName("UI");

            Builder.RegisterAssemblyTypes(domainAssembly)
            .Where(x => x.Name.EndsWith("Order"))
            .AsImplementedInterfaces();

            Builder.RegisterAssemblyTypes(domainAssembly)
            .Where(x => x.Name.EndsWith("Employee"))
            .AsImplementedInterfaces();

            Builder.RegisterAssemblyTypes(domainAssembly)
            .Where(x => x.Name.EndsWith("Customer"))
            .AsImplementedInterfaces();

            
            Builder.RegisterAssemblyTypes(repAssembly)
            .Where(x => x.Name=="Northwind")
            .AsImplementedInterfaces();
        }

        public static Assembly GetAssemblyByName( string assemblyName)
        {
            AppDomain domain = AppDomain.CurrentDomain;
            var ass= domain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
            if (ass == null)
            {
               ass = domain.Load(assemblyName);
            }

            return ass;
        }
    }
}