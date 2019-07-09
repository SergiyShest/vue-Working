using datagrid_mvc5.Controllers;

using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using Assert = Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace datagrid_mvc5.Models.Tests
{
    [TestFixture]
    public class OrderTests
    {
        #region Init

       // [TestSe]
        public void Init()
        {

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain.CurrentDomain.SetData("DataDirectory", path);
            //Устанавливаю директорию дата так как в ней лежит база данных 
        }

        #endregion

        #region MyRegion
        [TestCase(Description = "Существующий идентификатор")]
        public void OrderControllerGetByIdTest()
        {
            var bdContext = new Northwind();
            var id = bdContext.Orders.First().OrderID;//получил первый существующий идентификатор

            var orderController = new OrdersController();
            var json = orderController.GetById(id) as ContentResult;

            var res = JsonConvert.DeserializeObject(json.Content, typeof(Order)) as Order;
            Assert.AreEqual(id, res.OrderID);//Полученный объект имеет тот же идентификатор
        }

        [Test]
        public void AvaiableRegionsTest()
        {
            OrdersController controller = new OrdersController();
            var res = controller.AvaiableRegions() as ContentResult;
            var ress = JsonConvert.DeserializeObject<string[]>(res.Content).ToList();
            Assert.IsTrue(ress.Count > 0);

        }

        [TestCase(true)]
        [TestCase(false)]
        [TestCase(null)]
        public void AvaiableCountryTest(bool? valRegion)
        {
            Northwind db = new Northwind();
            string region = null;
            if (valRegion == true)
            {

                region = db.Orders.Select(c => c.ShipRegion).First();//беру первый регион
            }
            if (valRegion == false)
            {
                region = "dontExist"; // такого региона нет
            }

            OrdersController controller = new OrdersController();
            var res = controller.AvaiableCountrys(region) as ContentResult;
            var ress = JsonConvert.DeserializeObject<string[]>(res.Content).ToList();
            if (valRegion == true)//Регион корректный
            {
                Assert.IsTrue(ress.Count > 0);
            }
            if (valRegion == false)//Регион не корректный
            {
                Assert.IsTrue(ress.Count == 0);
            }
            if (valRegion == null)//Регион не задан
            {
                Assert.AreEqual(db.Orders.Select(c => c.ShipCountry).Distinct().Count(), ress.Count);
            }
        }

        [TestCase(true, null)]//страна валидная, региона  нет, количество элементов  списка больше нуля
        [TestCase(false, null)]//страна не валидная, регион нулл, количество элементов  списка  нуль
        [TestCase(null, null)] //страна нулл, регион нулл, количество элементов  списка = количеству городов
        [TestCase(null, true)] //страна нулл, регион валидный, количество элементов  списка больше нуля
        public void CityListTest(bool? valCountry, bool? valRegion)
        {
            Northwind db = new Northwind();
            string region = null;
            if (valRegion == true)
            {
                region = db.Orders.Select(c => c.ShipRegion).First();//беру первый регион
            }
            if (valRegion == false)
            {
                region = "dontExist"; // такого региона нет
            }
            string country = null;
            if (valCountry == true && valRegion == true)
            {
                country = db.Orders.Where(c => c.ShipRegion == region).Select(c => c.ShipCountry).First();//беру первую страну регион
            }
            if (valCountry == false)
            {
                country = "dontExist"; // такой страны нет
            }


            OrdersController controller = new OrdersController();
            var res = controller.AvaiableCityList(country, region) as ContentResult;
            var ress = JsonConvert.DeserializeObject<string[]>(res.Content).ToList();
            if (valCountry == true || valRegion == true)//Регион корректный и страна конрректная
            {
                Assert.IsTrue(ress.Count > 0);
            }
            if (valCountry == false || valRegion == false)//страна или регион не корректные
            {
                Assert.IsTrue(ress.Count == 0);
            }
            if (valCountry == null && valRegion == null)//Регион не задан страна не задана
            {
                Assert.AreEqual(db.Orders.Select(c => c.ShipCity).Distinct().Count(), ress.Count);
            }
        }


        #endregion

        

    }
}