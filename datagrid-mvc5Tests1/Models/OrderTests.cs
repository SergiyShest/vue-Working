using datagrid_mvc5.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using NUnit.Framework;
using System;
using System.Data.Entity.Validation;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace datagrid_mvc5.Models.Tests
{
    [TestFixture]
    public class OrderTests
    {

        [TestInitialize]
        public void init()
        {

            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            AppDomain.CurrentDomain.SetData("DataDirectory", path);

        }



        [Test]
        public void OrderTest()
        {

            Northwind nrv = new Northwind();
            var orders = nrv.Orders.ToList();
            orders[0].ShipCity = "Old noterdam de Pary in Moskow";
            var errors = nrv.GetValidationErrors();

            foreach (var valRes in errors)
            {
                foreach (var valError in valRes.ValidationErrors)
                {
                    Debug.WriteLine(valError.PropertyName + " " + valError.ErrorMessage);
                }
            }
            //catch (DbEntityValidationException ex)
            {
                foreach (DbEntityValidationResult validationError in errors)
                {
                    Debug.WriteLine("Object: " + validationError.Entry.Entity.ToString());
                    Debug.WriteLine("");
                    foreach (DbValidationError err in validationError.ValidationErrors)
                    {
                        Debug.WriteLine(err.ErrorMessage + "");
                    }
                }
            }
        }

       // [TestCase("mos")]
        [TestCase("Moskow")]
       // [TestCase("Old noterdam de Pary in CustomCheckAttribute")]
        public void ControllerTest(string city)
        {
            Northwind nrv = new Northwind();
            var order = nrv.Orders.FirstOrDefault();
            var srt = JsonConvert.SerializeObject(order);
            srt = Regex.Replace(srt, @"""ShipCity"":""(\w+)""", @"""ShipCity"":""" + city + @"""");

            OrdersController controller = new OrdersController();

            var res = controller.Validate(order.OrderID, srt);

        }


        [TestCase("","")]
        [TestCase("France","null")]
        [TestCase("Brazil", "RJ")]
        [TestCase("", "RJ")]
       // [TestCase("Old noterdam de Pary in CustomCheckAttribute")]
        public void CityListTest(string country,string region)
        {
            OrdersController controller = new OrdersController();
            var res = controller.AvaiableCityList(region, country);

        }

    }
}