using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MvcProject.Models;
using System.IO;

namespace MvcProject.Controllers
{
    public class DownloadController : Controller
    {
        [Authorize]
        public ActionResult Index()
        {
            var model = new FilesModel();
            var directoryPath = HttpContext.Server.MapPath("~/App_Data/FilesToDownload");
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            model.filesPaths = Directory.GetFiles(directoryPath);
            var list = new List<string>();

            foreach (var file in model.filesPaths)
            {
                list.Add(Path.GetFileName(file));
            }

            return View("DownloadFiles", list);
        }

        public ActionResult Download(string fileName)
        {
            var directoryPath = HttpContext.Server.MapPath("~/App_Data/FilesToDownload");
            byte[] fileBytes = System.IO.File.ReadAllBytes(directoryPath + "/" + fileName);

            return File(fileBytes, System.Net.Mime.MediaTypeNames.Application.Octet, fileName);
        }
    }
}