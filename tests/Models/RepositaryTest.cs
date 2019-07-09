using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Core;
using datagrid_mvc5.Models;
using NUnit.Framework;

namespace datagrid_mvc5Tests1.Models
{
    [TestFixtureAttribute]
    public class RepositarysTest
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

      //  [Test]
        public void OrdersRepositaryAddTest()
        {
          var order = F.Get<IOrder>();
          var rep = F.Get<IOrdersRepositary>();

          rep.Add(order);
          var  isChanged=rep.HasChanges();
          Assert.IsTrue(isChanged); 

        }


        [Test]
        public void OrdersRepositaryFindTest()
        {
            int id = 10250;
          var rep = F.Get<IOrdersRepositary>();
       var orders=  rep.Orders.ToList();

   
                    var findedObj = rep.Find(id);
         
          Assert.AreEqual(id, findedObj.Id); 

        }

    }
}
