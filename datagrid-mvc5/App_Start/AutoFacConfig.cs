using Autofac;
using datagrid_mvc5.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace datagrid_mvc5.App_Start
{
    public static class AutoFacConfigX
    {

 public static ContainerBuilder Builder { get; set; }
         static  AutoFacConfigX()
        {
            Builder = new ContainerBuilder();
            var assembly = GetAssemblyByName("datagrid-mvc5");

            Builder.RegisterAssemblyTypes(assembly)
            .Where(x => x.Name.EndsWith("Order"))
            .AsImplementedInterfaces();

            Builder.RegisterAssemblyTypes(assembly)
            .Where(x => x.Name=="Northwind")
            .AsImplementedInterfaces();
        }

        public static Assembly GetAssemblyByName( string assemblyName)
        {
            AppDomain domain = AppDomain.CurrentDomain;
            return domain.GetAssemblies().FirstOrDefault(a => a.GetName().Name == assemblyName);
        }
    }
}

