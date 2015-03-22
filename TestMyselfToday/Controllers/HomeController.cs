﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using TestMyselfToday.Models;

namespace TestMyselfToday.Controllers
{
    public class HomeController : Controller
    {
        TMTEntities db = new TMTEntities();

        public ActionResult Index(string search)
        {
            List<Test> lstTest = null;

            switch (search)
            {
                case "new":
                    {
                        lstTest = db.Tests.Where(x => x.IsActive).OrderByDescending(o => o.DateAndTime).ToList();
                        break;
                    }
                case "best":
                    {
                        lstTest = db.Tests.Where(x => x.IsActive).OrderByDescending(o => o.UsageCount).ToList();
                        break;
                    }
                default:
                    {
                        lstTest = db.Tests.Where(x => x.IsActive).OrderBy(o => o.SortOrder).ToList();
                        break;
                    }
            }

            return View(lstTest);
        }

        public ActionResult ForAdmin()
        {
            return View();
        }

        public ActionResult Test(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Test test = db.Tests.Find(id);

            if (test == null)
            {
                return HttpNotFound();
            }
            return View(test);
        }

        [HttpPost]
        public ActionResult Test(FormCollection form)
        {
            decimal tempScore = 0;
            string tempId = string.Empty;
            decimal totalScore = 0;

            TestResult testResult = null;
            long testResultId = 0;

            if (!String.IsNullOrEmpty(form["QuestionCount"]))
            {
                int questionCount = Convert.ToInt32(form["QuestionCount"]);

                for (int i = 1; i <= questionCount; i++)
                {
                    tempId = "o-rb-s-" + i;
                    if (!String.IsNullOrEmpty(form[tempId]))
                    {
                        tempScore = 0;
                        Decimal.TryParse(form[tempId], out tempScore);

                        totalScore += tempScore;
                    }
                }

                if (!String.IsNullOrEmpty(form["TestId"]))
                {
                    long testId = Convert.ToInt64(form["TestId"]);

                     var test = db.Tests.Find(testId);

                    if (test != null)
                    {
                        if(test.UsageCount != null)
                        {
                            test.UsageCount = test.UsageCount + 1;
                        }
                        else
                        {
                            test.UsageCount = 1;
                        }

                        db.SaveChanges();

                        testResult = test.TestResults.FirstOrDefault(x => x.RangeStart <= totalScore && x.RangeEnd >= totalScore);
                    }
                }                

                if (testResult != null)
                {
                    testResultId = testResult.Id;                    
                }
                else
                {
                    return RedirectToAction("Alert", "Home", new { id = 1 });
                }

            }
            return RedirectToAction("TestResult", "Home", new { id = testResultId });
        }


        public ActionResult TestResult(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestResult testResult = db.TestResults.Find(id);

            if (testResult == null)
            {
                return HttpNotFound();
            }
            return View(testResult);
        }


        public ActionResult Alert(long id)
        {
            switch (id)
            {
                case 1:
                    {
                        ViewBag.Message = "No Record found against your request";
                        break;
                    }
            }

            return View();
        }
    }
}