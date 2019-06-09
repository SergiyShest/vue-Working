using datagrid_mvc5.Models;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Drawing;
using System.Dynamic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using System.Web.WebSockets;

namespace datagrid_mvc5.Controllers
{

    public class OrdersController : Controller
    {

      public new ActionResult SimpleIndex()
      {
            ViewBag.Orders = _db.Orders;
            return View();
        }
        public ActionResult Index()
        {
            var usedCustomers = from o in _db.Orders
                                select new
                                {
                                    Id = o.CustomerID,
                                    Name = o.Customer.ContactName,
                                };
            var usedCust = usedCustomers.Distinct().ToList();
            var sb = new StringBuilder();
            foreach (var cust in usedCust)
            {
                sb.Append("{Id=\"" + cust.Id + "\",Name=\"" + cust.Name + "\"},");
            }

            ViewBag.customers = sb.ToString();
            return View();
        }
        public ActionResult Edit(int id = 0)
        {
            ViewBag.Id = id;
            return View();
        }

        public ActionResult SimpleEdit(int id = 0)
        {
            ViewBag.Id = id;
            return View();
        }


        Northwind _db = new Northwind();

        [HttpGet]
        public ActionResult Get(DataSourceLoadOptions loadOptions)
        {
            loadOptions.PrimaryKey = new[] { "OrderID" };
            var ordersQuery = from o in _db.Orders
                              select new
                              {
                                  o.OrderID,
                                  o.CustomerID,
                                  //                                  CustomerName = o.Customer.ContactName,
                                  o.EmployeeID,
                                  EmployeeName = o.Employee.FirstName + " " + o.Employee.LastName,
                                  o.OrderDate,
                                  o.RequiredDate,
                                  o.ShippedDate,
                                  o.ShipVia,
                                  ShipViaName = o.Shipper.CompanyName,
                                  o.Freight,
                                  o.ShipName,
                                  o.ShipAddress,
                                  o.ShipCity,
                                  o.ShipRegion,
                                  o.ShipPostalCode,
                                  o.ShipCountry
                              };

            var loadResult = DataSourceLoader.Load(ordersQuery, loadOptions);
            return Content(JsonConvert.SerializeObject(loadResult), "application/json");
        }

        [HttpGet]
        public ActionResult GetShipCountry(DataSourceLoadOptions loadOptions)
        {
            var ordersQuery = from o in _db.Orders
                              select new
                              {
                                  o.ShipCountry
                              };
            var json = JsonConvert.SerializeObject(ordersQuery.Distinct().ToList());
            return Content(json, "application/json");
        }

        [HttpGet]
        public ActionResult GetById(int id)
        {
            var order = _db.Orders.Find(id);//Получили объект
            string orderStr = JsonConvert.SerializeObject(order);//Сериализовали его
            return Content(orderStr, "application/json");//отправили 
        }

        [HttpGet]
        public ActionResult Validate(int id, string json)
        {
            var order = _db.Orders.Find(id);
            JsonConvert.PopulateObject(json, order);
            var errorsD = GetErrorsAndChanged();
            return Content(errorsD.ToString(), "application/json");
        }

        [HttpGet]
        public ActionResult Save(int id, string json)
        {
            String res= null;
            List<DbEntityValidationResult> errors=new List<DbEntityValidationResult>();
            var order = _db.Orders.Find(id);
            JsonConvert.PopulateObject(json, order);
          var changed=  _db.ChangeTracker.HasChanges();
            try
            {
                _db.SaveChanges();
                changed = false;
            }
            catch (DbEntityValidationException ex)
            {
               errors.AddRange( ex.EntityValidationErrors);

            }
            res  = GetErrorsAndChanged( errors,changed).ToString();
           // res = string.Format("{{isChanged:{0},errors:{1}",changed, err);
            return Content(res, "application/json");
        }

        private String  GetErrorsAndChanged()
        {
            var changed=  _db.ChangeTracker.HasChanges();
            var errors = _db.GetValidationErrors();
            return GetErrorsAndChanged(errors,changed);
        }

        private static string   GetErrorsAndChanged(IEnumerable<DbEntityValidationResult> errors,bool changed)
        {
            dynamic dynamic = new ExpandoObject();
            dynamic.IsChanged = changed;//Создание свойства IsChanged
            var errProperty = new Dictionary<string, object>();//Создание массива с будущими свойсвтвами ошибки
            dynamic.Errors = new DynObject(errProperty);//Создание объекта у которого свойства задаются в массиве
            foreach (DbEntityValidationResult validationError in errors)//Заполнение массива ошибками
            {
                foreach (DbValidationError err in validationError.ValidationErrors)//Заполнение массива ошибками
                {
                    errProperty.Add(err.PropertyName,err.ErrorMessage);
                }
            }
            var json = JsonConvert.SerializeObject(dynamic); return json;
        }

        [HttpGet]
        public ActionResult AvaialbeEmploers()
        {
            var emploer = from o in _db.Employees
                          select new
                          {
                              Id = o.EmployeeID,
                              Name = o.Employee1.FirstName
                          };
            string jsonStr = JsonConvert.SerializeObject(emploer);
            return Content(jsonStr, "application/json");
        }

        [HttpGet]
        public ActionResult AvaialbeCustomers()
        {
            var product = from o in _db.Customers
                          select new
                          {
                              Id = o.CustomerID,
                              Name = o.ContactName,
                          };
            string jsonStr = JsonConvert.SerializeObject(product);
            return Content(jsonStr, "application/json");
        }


        #region Получение списков Linq
        /// <summary>
        /// Список доступных городов c учетом региона и страны
        /// если регион или страна не заданы , то все города 
        /// </summary>
        /// <param name="country"></param>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AvaiableCityList( string country,string region=null)
        {
            var avaiableCity =  _db.Orders.Where(c => ((c.ShipRegion == region) || region == null)&& (c.ShipCountry == country) || country == null).Select(a => a.ShipCity).Distinct();
            
            var jsonStr = JsonConvert.SerializeObject(avaiableCity);
            return Content(jsonStr, "application/json");
        }

        /// <summary>
        /// Список доступных стран c учетом региона
        /// если регион не задан, то все страны
        /// </summary>
        /// <param name="region"></param>
        /// <returns></returns>
        [HttpGet]
        public ActionResult AvaiableCountrys(string region=null)
        {
            var resList = _db.Orders.Where(c => (c.ShipRegion == region)||region==null).Select(c => c.ShipCountry).Distinct();
            var json = JsonConvert.SerializeObject(resList);
            return Content(json, "application/json");
        }

        [HttpGet]
        public ActionResult AvaiableRegions()
        {
            var resList = _db.Orders.Select(c=>c.ShipRegion).Distinct();
            var json = JsonConvert.SerializeObject(resList);
            return Content(json, "application/json");
        }

        #endregion

        #region Получение списка селектом


        [HttpGet]
        public ActionResult AvaiableCityListSql(string region, string country)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string query = "Select distinct ShipCity from orders where {ShipCountry} {ShipRegion}  1=1";
            query = MadeParam("ShipCountry", country, parameters, query);
            query = MadeParam("ShipRegion", region, parameters, query);

            return GetActionResultSql(query, parameters);
        }


        [HttpGet]
        public ActionResult AvaiableCountrySql(string region)
        {
            List<SqlParameter> parameters = new List<SqlParameter>();
            string query = "Select distinct ShipCountry from orders where  {ShipRegion}  1=1";
            query = MadeParam("ShipRegion", region, parameters, query);

            return GetActionResultSql(query, parameters);
        }

        [HttpGet]
        public ActionResult AvaiableRegionsSql()
        {
            string query = "Select distinct ShipRegion from orders";
            return GetActionResultSql(query);
        }

  
       

        /// <summary>
        /// получение коллекции запросом
        /// </summary>
        /// <param name="query"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private ActionResult GetActionResultSql(string query, List<SqlParameter> parameters = null)
        {
            if (parameters == null) parameters = new List<SqlParameter>();
            var resList = _db.Database.SqlQuery<string>(query, parameters.ToArray()).ToList();

            string json = JsonConvert.SerializeObject(resList);
            return Content(json, "application/json");
        }

        private static string MadeParam(string name, string paramValue, List<SqlParameter> parameters, string query)
        {
            var prarString = String.Empty;
            if (!string.IsNullOrEmpty(paramValue))
            {
                prarString = name + " = @" + name + " and";
                if (paramValue == "null")
                {
                    prarString = name + " is null and";
                }
                else
                {
                    parameters.Add(new SqlParameter(name, SqlDbType.Char) { Value = paramValue });
                }
            }

            query = query.Replace("{" + name + "}", prarString);
            return query;
        }
        #endregion
    }

    public  class NamedEntity
    {
        public int Id { get; set; }
        public  string Name { get; set; }
    }

}