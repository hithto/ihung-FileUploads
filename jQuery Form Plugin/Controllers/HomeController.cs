using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace jQuery_Form_Plugin.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Message = "歡迎使用 ASP.NET MVC!";

            return View();
        }

        public ActionResult About()
        {
            return View();
        }

        public ActionResult Upload()
        {
            return View();
        }

        [HttpPost]
        public JsonResult Upload(HttpPostedFileBase file)
        {
            Dictionary<string, object> jo = new Dictionary<string, object>();

            if (file == null)
            {
                jo.Add("success", false);
                jo.Add("message", "file upload error.");
            }
            else
            {
                if (file.ContentLength > 0 && file.ContentLength < (1 * 1024 * 1024))
                {
                    var fileName = Path.GetFileName(file.FileName);
                    var path = Path.Combine(Server.MapPath("~/FileUploads"), fileName);
                    file.SaveAs(path);

                    jo.Add("success", true);
                    jo.Add("message", file.FileName);
                    jo.Add("ContentLenght", file.ContentLength);
                }
                else
                {
                    if (file.ContentLength <= 0)
                    {
                        jo.Add("success", false);
                        jo.Add("message", "請上傳正確的檔案.");
                    }
                    else if (file.ContentLength > (1 * 1024 * 1024))
                    {
                        jo.Add("success", false);
                        jo.Add("message", "上傳檔案大小不可超過 1MB.");
                    }
                }
            }
            return Json(jo);
        }
    }
}
