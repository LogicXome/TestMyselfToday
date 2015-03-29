using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System.Web.UI;
using TestMyselfToday.Models;
using TestMyselfToday.Models.Code;

namespace TestMyselfToday.Controllers
{
    public class HomeController : Controller
    {
        TMTEntities db = new TMTEntities();

        [ForUsers]
        public ActionResult Index(string search)
        {
            List<Test> lstTest = null;

            var lstLanguage = db.Languages.OrderBy(o => o.SortOrder).ToList();

            ViewBag.Languages = lstLanguage;

            long languageId = 0;

            if (!String.IsNullOrEmpty(Request.Form["language"]))
            {
                languageId = Convert.ToInt64(Request.Form["language"]);
                Session["SelectedLanguage"] = languageId;

                var lstCommonDictionary = db.CommonDictionaries.Where(x => x.LanguageId == languageId).ToList();

                Session["IsValueSet"] = true;

                var cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-Myself-Today", StringComparison.OrdinalIgnoreCase));

                if (cd != null)
                {
                    Session["TestMyselfToday"] = cd.CDValue;
                }

                cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-New", StringComparison.OrdinalIgnoreCase));

                if (cd != null)
                {
                    Session["TestNew"] = cd.CDValue;
                }

                cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-Best", StringComparison.OrdinalIgnoreCase));

                if (cd != null)
                {
                    Session["TestBest"] = cd.CDValue;
                }

                cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-Random", StringComparison.OrdinalIgnoreCase));

                if (cd != null)
                {
                    Session["TestRandom"] = cd.CDValue;
                }

                cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Terms-Conditions", StringComparison.OrdinalIgnoreCase));

                if (cd != null)
                {
                    Session["TermsConditions"] = cd.CDValue;
                }

                cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Privacy", StringComparison.OrdinalIgnoreCase));

                if (cd != null)
                {
                    Session["Privacy"] = cd.CDValue;
                }

                cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Social-Facebook", StringComparison.OrdinalIgnoreCase));

                if (cd != null)
                {
                    Session["SocialFacebook"] = cd.CDValue;
                }

                cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-Other", StringComparison.OrdinalIgnoreCase));

                if (cd != null)
                {
                    Session["TestOther"] = cd.CDValue;
                }

                cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-Start", StringComparison.OrdinalIgnoreCase));

                if (cd != null)
                {
                    Session["TestStart"] = cd.CDValue;
                }

                cd = lstCommonDictionary.FirstOrDefault(x => x.CDKey.Equals("Test-Again", StringComparison.OrdinalIgnoreCase));

                if (cd != null)
                {
                    Session["TestAgain"] = cd.CDValue;
                }
            }
            else if (Session["SelectedLanguage"] == null)
            {
                if (lstLanguage != null && lstLanguage.Count > 0)
                {
                    languageId = lstLanguage.First().Id;
                    Session["SelectedLanguage"] = languageId;
                }
            }
            else
            {
                languageId = Convert.ToInt64(Session["SelectedLanguage"]);
            }            

            switch (search)
            {
                case "new":
                    {
                        lstTest = db.Tests.Where(x => x.IsActive && x.LanguageId == languageId).OrderByDescending(o => o.DateAndTime).ToList();
                        break;
                    }
                case "best":
                    {
                        lstTest = db.Tests.Where(x => x.IsActive && x.LanguageId == languageId).OrderByDescending(o => o.UsageCount).ToList();
                        break;
                    }
                default:
                    {
                        lstTest = db.Tests.Where(x => x.IsActive && x.LanguageId == languageId).OrderBy(o => o.SortOrder).ToList();
                        break;
                    }
            }
            return View(lstTest);
        }

        [ForAdminsOnly]
        public ActionResult ForAdmin()
        {
            return View();
        }

        public ActionResult RandomTest(long? languageId)
        {
            if (languageId == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            var lstTest = db.Tests.Where(x => x.IsActive && x.LanguageId == languageId).OrderByDescending(o => o.UsageCount).ToList();

            Test test = null;

            if (lstTest.Count > 0)
            {
                var r = new Random();

                var randomNo = r.Next(lstTest.Count - 1);

                test = lstTest.ElementAt(randomNo);
            }

            if (test == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View("Test", test);
        }

        [ForUsers]
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
            string friendlyUrl = string.Empty;

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
                        if (test.UsageCount != null)
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

                    if (String.IsNullOrEmpty(testResult.TextForSharing))
                    {
                        friendlyUrl = testResult.TitleText + "  " + testResult.ResultText;
                    }
                    else
                    {
                        friendlyUrl = testResult.TextForSharing;
                    }

                    if (!String.IsNullOrEmpty(friendlyUrl))
                    {
                        friendlyUrl = friendlyUrl.Replace(' ', '-');
                        friendlyUrl = friendlyUrl.Trim().ToLower();
                    }
                }
                else
                {
                    return RedirectToAction("Alert", "Home", new { id = 1 });
                }
            
            }

            return RedirectToAction("TestResult", "Home", new { id = testResultId, ignoreThis = friendlyUrl });
        }

        [ForUsers]
        public ActionResult TestResult(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestResult testResult = db.TestResults.Find(id);

            string urlReferrer = Convert.ToString(Request.UrlReferrer);
            urlReferrer = urlReferrer.ToLower();

            if (testResult == null)
            {
                return HttpNotFound();
            }
            else if (!String.IsNullOrEmpty(urlReferrer) && (urlReferrer.Contains("facebook") || urlReferrer.Contains("google") || urlReferrer.Contains("twitter")))
            {
                return RedirectToAction("Test", "Home", new { id = testResult.TestId });
            }
            return View(testResult);
        }

        //
        // GET: /Account/Login
        [AllowAnonymous]
        public ActionResult LoginForAdmin()
        {
            return View();
        }

        //
        // POST: /Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public ActionResult LoginForAdmin(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var count = db.Users.Count(x => x.Username.Equals(model.Username, StringComparison.OrdinalIgnoreCase) && x.Password.Equals(model.Password));
                if (count > 0)
                {
                    Session["IsTestMyTodayAdmin"] = true;

                    return RedirectToAction("ForAdmin", "Home");
                }
                else
                {
                    ModelState.AddModelError("", "Invalid username or password.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
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


        //Partial Views
        public PartialViewResult _NavBar()
        {
            return PartialView();
        }
    }
}