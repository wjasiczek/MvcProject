using MvcProject.Models;
using System.Text;
using System.Web.Mvc;
using Microsoft.AspNet.Identity;
using System.Linq;
using System.Data.Entity;
using PagedList;
using System;

namespace MvcProject.Controllers
{
    public class CrcController : Controller
    {
        private ApplicationDbContext appDb = new ApplicationDbContext();

        // GET: Crc
        [HttpGet]
        public ActionResult Index()
        {
            return View(new CrcSubmitViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult SubmitForm(CrcSubmitViewModel model)
        {
            if (ModelState.IsValid)
            {
                ModelState.Remove("crcModel.remainder");
                ModelState.Remove("crcModel.signal");
                model.crcModel.remainder = ComputeFrame(model.crcModel.binaryValue, model.crcModel.generator);
                model.crcModel.signal = model.crcModel.binaryValue + model.crcModel.remainder;
                model.crcModel.DateAdded = DateTime.Now;

                if (User.Identity.IsAuthenticated)
                {
                    var uniqueRecord = (from record in appDb.CrcModels
                                        where record.binaryValue == model.crcModel.binaryValue &&
                                       record.generator == model.crcModel.generator
                                        select record).ToList();

                    if (uniqueRecord.Count == 0)
                    {
                        var currentUser = appDb.Users.Find(User.Identity.GetUserId());
                        model.crcModel.user = currentUser;
                        appDb.CrcModels.Add(model.crcModel);
                        appDb.SaveChanges();
                        model.result = "Data loaded!";
                    }
                    else
                    {
                        model.result = "Data already in database";
                    }
                }
                else
                {
                    model.result = "Data loaded!";
                }
            }            

            return View("Index", model);
        }


        public ActionResult GetCorrectness(string signal, string remainder, int? id)
        {
            var isCorrect = CheckCorrectness(remainder);
            var message = isCorrect ? "Correct signal!" : "Wrong signal!";

            if (User.Identity.IsAuthenticated)
            {
                var query = (from record in appDb.CrcModels
                             where record.remainder == remainder &&
                             record.signal == signal
                             select record).First();

                query.correctness = message;

                appDb.Entry(query).State = EntityState.Modified;

                appDb.SaveChanges();
            }

            return Json(new { message = message, id = id }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult CrcList(string filter, int pageNumber = 1)
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            var currentUser = User.Identity.GetUserId();
            var crcModels = from models in appDb.CrcModels
                            where models.user.Id == currentUser
                            orderby models.DateAdded
                             descending
                            select models;

            if (filter != null)
            {
                if (filter == "All")
                {
                    return PartialView("_crcFilterList", crcModels.ToPagedList(pageNumber, 5));
                }

                switch (filter)
                {
                    case "Correct": { filter = "Correct signal!"; break; }
                    case "Wrong": { filter = "Wrong signal!"; break; }
                }

                var filteredModels = (from models in crcModels
                                      where models.correctness == filter
                                      orderby models.DateAdded
                                        descending
                                      select models).ToPagedList(pageNumber, 5);

                return PartialView("_crcFilterList", filteredModels);
            }

            return View(crcModels.ToPagedList(pageNumber, 5));
        }

        public bool CheckCorrectness(string remainder)
        {
            var isCorrect = true;

            for (var i = 0; i < remainder.Length; i++)
            {
                if (remainder[i] != '0')
                {
                    isCorrect = false;
                    break;
                }
            }

            return isCorrect;
        }

        public ActionResult WhatIsIt()
        {
            return View();
        }

        private string ComputeFrame(string value, string generator)
        {
            for (var i = 0; i < generator.Length - 1; i++)
            {
                value += "0";
            }

            var iterationBuffer = 0;

            for (var i = 0; i < value.Length - generator.Length + 1; i++)
            {
                iterationBuffer = i;
                if (value[i] == '1')
                {
                    for (var j = 0; j < generator.Length; j++)
                    {
                        var strBuilder = new StringBuilder(value);
                        strBuilder[i + j] = value[i + j] == generator[j] ? '0' : '1';
                        value = strBuilder.ToString();
                    }
                }
            }

            value = value.Substring(iterationBuffer);

            return value;
        }

        protected override void Dispose(bool disposing)
        {
            if(disposing && appDb != null)
            {
                appDb.Dispose();
                appDb = null;
            }

            base.Dispose(disposing);
        }
    }
}