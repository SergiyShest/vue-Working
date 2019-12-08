using datagrid_mvc5.Models;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
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
        public ActionResult Maket()
        {
            return View("Maket");
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
        public void uploadFile()
        {

            foreach (string file in base.Request.Files)
            {
          var fileS=      Request.Form["file"];
          var messageId =      Request.Form["messageId"];
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
           // return RedirectToAction("upload");
        }
        public FileResult Download(string xxxx)
        {
            byte[] fileBytes = System.IO.File.ReadAllBytes(@"c:\Users\titov\source\repos\SergiyShest\vue-Working\datagrid-mvc5\App_Data\TestCafeTests.zip");
            string fileName = "myfile.ext";
            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, xxxx + fileName);
        }
        public ActionResult FileApi()
        {
            var seessionID = base.ControllerContext.HttpContext.Session.SessionID;
            var httpContext = ControllerContext.RequestContext.HttpContext;
            var mSid = httpContext.Session.SessionID;
            sessionStor.sessionId = seessionID;
            return View("fileApiExample");
        }

        [HttpGet]
        // [AllowAnonymous]
        public ActionResult GetPassword()
        {
            var byteArray = Encoding.UTF8.GetBytes("username:password1234");
            byte[] encodedBytes = ProtectedData.Protect(byteArray
                , null
                , DataProtectionScope.CurrentUser);
            string utfString = Convert.ToBase64String(encodedBytes, 0, encodedBytes.Length);
            return Content(utfString, "application/text");
        }

    }

    public static class sessionStor
    {
        public static String sessionId { get; set; }
    }
}