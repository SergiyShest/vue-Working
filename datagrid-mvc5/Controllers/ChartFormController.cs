using datagrid_mvc5.Models;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;

namespace datagrid_mvc5.Controllers
{

    public class ChartFormController : Controller
    {


        public ActionResult context()
        {

            return View("context");
        }


        public ActionResult Index()
        {
            var seessionID = base.ControllerContext.HttpContext.Session.SessionID;
            var httpContext = ControllerContext.RequestContext.HttpContext;
            var mSid = httpContext.Session.SessionID;
            sessionStor.sessionId = seessionID;
            return View("ChatForm");
        }
        [HttpPost]
        public ActionResult uploadFile()
        {

            foreach (string file in base.Request.Files)
            {
          var fileS=      Request.Form["file"];
var chatId =      Request.Form["chatId"];
                using (MemoryStream ms = new MemoryStream())
                {
                    Request.Files[file].InputStream.CopyTo(ms);
                   byte[] array = ms.GetBuffer();
                   Debug.WriteLine(file);
                    if (Request.Files[file].FileName != "")
                    {
                        string path = AppDomain.CurrentDomain.BaseDirectory + "/App_Data/";
                        string filename = Path.GetFileName(Request.Files[file].FileName);
                        Request.Files[file].SaveAs(Path.Combine(path, filename));
                    }
                }
            }
            return RedirectToAction("upload");
        }

        public ActionResult FileApi()
        {
            var seessionID = base.ControllerContext.HttpContext.Session.SessionID;
            var httpContext = ControllerContext.RequestContext.HttpContext;
            var mSid = httpContext.Session.SessionID;
            sessionStor.sessionId = seessionID;
            return View("fileApiExample");
        }


    }

    public static class sessionStor
    {
        public static String sessionId { get; set; }
    }
}