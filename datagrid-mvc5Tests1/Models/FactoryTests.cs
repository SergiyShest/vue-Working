
using Autofac;
using datagrid_mvc5.App_Start;
using datagrid_mvc5.Models;

using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace datagrid_mvc5Tests1.Models
{   [TestFixture]
    public class FactoryTests
    {
        #region Init

       // [TestInitialize]
        public void Init()
        {

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            //Устанавливаю директорию дата так как в ней лежит база данных 
        }

        #endregion
        [Test]
        public void test()
        {
            var builder = AutoFacConfig.Builder;
            var container= builder.Build();
            var order = container.Resolve<IOrder>();

        }

    }
}
