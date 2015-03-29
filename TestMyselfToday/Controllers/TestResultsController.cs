using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TestMyselfToday.Models;
using TestMyselfToday.Models.Code;

namespace TestMyselfToday.Controllers
{
    [ForAdminsOnly]
    public class TestResultsController : Controller
    {
        private TMTEntities db = new TMTEntities();

        // GET: TestResults
        public ActionResult Index()
        {
            var testResults = db.TestResults.Include(t => t.Test);
            return View(testResults.ToList());
        }

        // GET: TestResults/Create
        public ActionResult Create()
        {
            ViewBag.TestId = new SelectList(db.Tests, "Id", "Title");
            return View();
        }

        // POST: TestResults/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TitleText,ResultText,ResultDetail,ImagePath,TestId,TextForSharing,RangeStart,RangeEnd")] TestResult testResult)
        {
            if (ModelState.IsValid)
            {
                db.TestResults.Add(testResult);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TestId = new SelectList(db.Tests, "Id", "Title", testResult.TestId);
            return View(testResult);
        }

        // GET: TestResults/Edit/5
        public ActionResult Edit(long? id)
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
            ViewBag.TestId = new SelectList(db.Tests, "Id", "Title", testResult.TestId);
            return View(testResult);
        }

        // POST: TestResults/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TitleText,ResultText,ResultDetail,ImagePath,TestId,TextForSharing,RangeStart,RangeEnd")] TestResult testResult)
        {
            if (ModelState.IsValid)
            {
                db.Entry(testResult).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TestId = new SelectList(db.Tests, "Id", "Title", testResult.TestId);
            return View(testResult);
        }

        // GET: TestResults/Delete/5
        public ActionResult Delete(long? id)
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

        // POST: TestResults/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            TestResult testResult = db.TestResults.Find(id);
            db.TestResults.Remove(testResult);
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
