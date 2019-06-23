using Autofac;
using datagrid_mvc5.Models;
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
            var assm = GetAssemblyByName("datagrid-mvc5");
            Builder.RegisterAssemblyTypes(assm)
            .Where(x => x.Name.EndsWith("Order"))
            .AsImplementedInterfaces();
        }

        public static Assembly GetAssemblyByName( string assemblyName)
        {
            AppDomain domain = AppDomain.CurrentDomain;
            return domain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
        }
    }
}