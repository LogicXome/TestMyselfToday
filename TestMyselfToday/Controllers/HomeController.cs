using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
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
        public ActionResult Index(string search, int page=1, int pageSize = 4)
        {
            List<Test> lstTest = null;

            ViewBag.Search = search;

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
                        if (String.IsNullOrEmpty(search))
                        {
                            lstTest = db.Tests.Where(x => x.IsActive && x.LanguageId == languageId).OrderBy(o => o.SortOrder).ToList();
                        }
                        else
                        {
                            long tempSectionId = 0;
                            if (Int64.TryParse(search, out tempSectionId))
                            {
                                lstTest = db.Tests.Where(x => x.IsActive && x.LanguageId == languageId && x.SectionId == tempSectionId).OrderBy(o => o.SortOrder).ToList();
                            }
                        }
                        break;
                    }
            }


            var lstSection = db.Sections.Where(x => x.LanguageId == languageId && x.SortOrder > 0).OrderBy(o => o.SortOrder).ToList();

            ViewBag.Sections = lstSection;


            //Pagination

            return View(lstTest.ToPagedList(page, pageSize));
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

            var tests = db.Tests.Where(x => x.LanguageId == test.LanguageId && x.IsActive == true && x.Id != test.Id).OrderByDescending(o => o.UsageCount).Take(3).ToList();

            ViewBag.OtherTests = tests;

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

            var tests = db.Tests.Where(x => x.LanguageId == testResult.Test.LanguageId && x.IsActive == true && x.Id != testResult.TestId).OrderByDescending(o => o.UsageCount).Take(3).ToList();

            ViewBag.OtherTests = tests;

            return View(testResult);
        }

        //
        // GET: /Account/Login
        [ForUsers]
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

        [HttpPost]
        public JsonResult SendEmail(string fullname, string email, string comment, bool isSuggestion)
        {
            string message = string.Empty;
            try
            {
                string body = WebUtility.UrlDecode(comment);
                string fromEmail = System.Configuration.ConfigurationManager.AppSettings["FromEmail"].ToString();
                string toEmail = System.Configuration.ConfigurationManager.AppSettings["ToEmail"].ToString();
                string ccEmail = System.Configuration.ConfigurationManager.AppSettings["CCEmail"].ToString();
                string subject1 = System.Configuration.ConfigurationManager.AppSettings["SuggestionSubject"].ToString();
                string subject2 = System.Configuration.ConfigurationManager.AppSettings["ContactSubject"].ToString();
                string smtpServer = System.Configuration.ConfigurationManager.AppSettings["SMTPServer"].ToString();
                string username = System.Configuration.ConfigurationManager.AppSettings["EmailUserName"].ToString();
                string password = System.Configuration.ConfigurationManager.AppSettings["EmailPassword"].ToString();

                System.Net.Mail.SmtpClient smtpClient = new System.Net.Mail.SmtpClient(smtpServer);
                smtpClient.Credentials = new System.Net.NetworkCredential(username, password);
                smtpClient.EnableSsl = true;

                using (MailMessage mailMsg = new MailMessage())
                {
                    mailMsg.From = new MailAddress(fromEmail);

                    mailMsg.To.Add(new MailAddress(toEmail));

                    if (!String.IsNullOrEmpty(ccEmail))
                    {
                        mailMsg.CC.Add(new MailAddress(ccEmail));
                    }

                    if(isSuggestion)
                    {
                        mailMsg.Subject = subject1;
                    }
                    else
                    {
                        mailMsg.Subject = subject2;
                    }
                   
                    string fromDetails = fullname + "<br/>" + email + "<br/>";

                    body = body + "<br/><br/><br/>" + fromDetails;

                    mailMsg.Body = body;
                    mailMsg.IsBodyHtml = true;
                    mailMsg.BodyEncoding = Encoding.UTF8;


                    smtpClient.Send(mailMsg);
                    return Json("success", JsonRequestBehavior.AllowGet);
                }

            }
            catch (Exception ex)
            {
                message = ex.Message;

            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }
        //Partial Views
        public PartialViewResult _NavBar()
        {
            return PartialView();
        }
    }
}