using datagrid_mvc5.Models;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace datagrid_mvc5.Controllers
{

    public class ChartFormController : Controller
    {



        public ActionResult Index()
        {
            var seessionID = base.ControllerContext.HttpContext.Session.SessionID;
            var httpContext = ControllerContext.RequestContext.HttpContext;
            var mSid = httpContext.Session.SessionID;
            sessionStor.sessionId = seessionID;
            return View("ChatForm");
        }
    }
    public static class sessionStor
    {
        public static String sessionId { get; set; }
    }
}