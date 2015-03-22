using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TestMyselfToday.Models;

namespace TestMyselfToday.Controllers
{
    public class TestsController : Controller
    {
        private TMTEntities db = new TMTEntities();

        // GET: Tests
        public ActionResult Index()
        {
            var tests = db.Tests.Include(t => t.Language).Include(t => t.Section);
            return View(tests.ToList());
        }

        // GET: Tests/Create
        public ActionResult Create()
        {
            ViewBag.LanguageId = new SelectList(db.Languages, "Id", "Title");
            ViewBag.SectionId = new SelectList(db.Sections, "Id", "Title");

            Test test = new Test();

            return View(test);
        }

        // POST: Tests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Test test)
        {
            if(ModelState.IsValid)
            {
                test.DateAndTime = DateTime.Now;

                db.Tests.Add(test);
                db.SaveChanges();
                return RedirectToAction("Index", "Tests");
            }

            ViewBag.LanguageId = new SelectList(db.Languages, "Id", "Title", test.LanguageId);
            ViewBag.SectionId = new SelectList(db.Sections, "Id", "Title", test.SectionId);
            return View(test);
        }

        // GET: AdminTests/Edit/5
        public ActionResult Edit(long? id)
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
            ViewBag.LanguageId = new SelectList(db.Languages, "Id", "Title", test.LanguageId);
            ViewBag.SectionId = new SelectList(db.Sections, "Id", "Title", test.SectionId);
            return View(test);
        }

        // POST: AdminTests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Test test)
        {
            if (ModelState.IsValid)
            {
                test.DateAndTime = DateTime.Now;    

                foreach(var question in test.Questions)
                {
                    foreach (var option in question.QuestionOptions)
                    {
                        if (option.Id > 0)
                        {
                            db.Entry(option).State = EntityState.Modified;
                        }
                        else
                        {
                            db.Entry(option).State = EntityState.Added;
                        }
                    }


                    if (question.Id > 0)
                    {
                        db.Entry(question).State = EntityState.Modified;
                    }
                    else
                    {
                        db.Entry(question).State = EntityState.Added;
                    }
                }

                if(!String.IsNullOrEmpty(Request.Form["deletedQuestionIds"]))
                {
                    var questionIds = Request.Form["deletedQuestionIds"].Split(',');

                    foreach(var id in questionIds)
                    {
                        if (!String.IsNullOrEmpty(id))
                        {
                            long questionId = 0;
                            Int64.TryParse(id, out questionId);

                            if (questionId > 0)
                            {
                                var question = db.Questions.Find(questionId);

                                if (question != null)
                                {
                                    db.QuestionOptions.RemoveRange(question.QuestionOptions);
                                    db.Questions.Remove(question);
                                }
                            }
                        }                        
                    }
                }

                if (!String.IsNullOrEmpty(Request.Form["deletedQuestionOptionIds"]))
                {
                    var questionOptionIds = Request.Form["deletedQuestionOptionIds"].Split(',');

                    foreach (var id in questionOptionIds)
                    {
                        if (!String.IsNullOrEmpty(id))
                        {
                            long questionOptionId = 0;
                            Int64.TryParse(id, out questionOptionId);

                            if (questionOptionId > 0)
                            {
                                var questionOption = db.QuestionOptions.Find(questionOptionId);

                                if (questionOption != null)
                                {
                                    db.QuestionOptions.Remove(questionOption);
                                }
                            }
                        }
                    }
                }

                db.Entry(test).State = EntityState.Modified;

                db.SaveChanges();
                return RedirectToAction("Index");
            }

            string messages = string.Join("; ", ModelState.Values
                                        .SelectMany(x => x.Errors)
                                        .Select(x => x.ErrorMessage));

            ViewBag.LanguageId = new SelectList(db.Languages, "Id", "Title", test.LanguageId);
            ViewBag.SectionId = new SelectList(db.Sections, "Id", "Title", test.SectionId);
            return View(test);
        }

        // GET: AdminTests/Delete/5
        public ActionResult Delete(long? id)
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

        // POST: AdminTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Test test = db.Tests.Find(id);
            
            foreach(var question in test.Questions)
            {
                db.QuestionOptions.RemoveRange(question.QuestionOptions);
            }

            db.Questions.RemoveRange(test.Questions);

            db.Tests.Remove(test);


            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
