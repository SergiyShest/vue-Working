
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
using Core;
using Core.Interfaces;

namespace datagrid_mvc5Tests1.Models
{
    /// <summary>
    /// 
    /// </summary>
    [TestFixture]
    public class FactoryTests
    {
        #region Init

       [OneTimeSetUp]
        public void Init()
        {

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            //Устанавливаю директорию дата так как в ней лежит база данных 
        }

        #endregion

        #region IOrder

        [Test]
        public void IOrderRegistrationTest()
        {
            var order = F.Get<IOrder>();
            Assert.IsNotNull(order);

        }
        [Test]
        public void IOrderReposRegistrationTest()
        {

            var orderRepositary = F.Get<IOrdersRepositary>();
            Assert.IsNotNull(orderRepositary);
            //  var orders = orderRepositary.Orders.ToList();
            //  Assert.IsTrue(orders.Count>0);

        }

        #endregion

        #region ICustomer

        [Test]
        public void ICustomerRegistrationTest()
        {
            var order = F.Get<ICustomer>();
            Assert.IsNotNull(order);
        }

        [Test]
        public void ICustomerReposRegistrationTest()
        {
            var repositary = F.Get<ICustomerRepositary>();
            Assert.IsNotNull(repositary);
        }

        #endregion

        #region IEmployee

        [Test]
        public void IEmployeeRegistrationTest()
        {
            var order = F.Get<IEmployee>();
            Assert.IsNotNull(order);
        }

        [Test]
        public void IEmployeeReposRegistrationTest()
        {
            var repositary = F.Get<IEmployeeRepositary>();
            Assert.IsNotNull(repositary);
        }

        #endregion

    }
}
