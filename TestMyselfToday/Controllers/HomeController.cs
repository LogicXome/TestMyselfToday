using System;
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

        public ActionResult Index()
        {
            var tests = db.Tests.OrderBy(o => o.SortOrder);
            return View(tests.ToList());
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

                    testResult = db.TestResults.FirstOrDefault(x => x.TestId == testId && x.RangeStart <= totalScore && x.RangeEnd >= totalScore);
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