using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using TestMyselfToday.Models;

namespace TestMyselfToday.Controllers
{
    public class RelatedTestsController : Controller
    {
        private TMTEntities db = new TMTEntities();

        // GET: RelatedTests
        public ActionResult Index()
        {
            var relatedTests = db.RelatedTests.Include(r => r.Test).Include(r => r.Test1);
            return View(relatedTests.ToList());
        }

        // GET: RelatedTests/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelatedTest relatedTest = db.RelatedTests.Find(id);
            if (relatedTest == null)
            {
                return HttpNotFound();
            }
            return View(relatedTest);
        }

        // GET: RelatedTests/Create
        public ActionResult Create()
        {
            ViewBag.TestId = new SelectList(db.Tests, "Id", "Title");
            ViewBag.RelatedTestId = new SelectList(db.Tests, "Id", "Title");
            return View();
        }

        // POST: RelatedTests/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,TestId,RelatedTestId,SortOrder")] RelatedTest relatedTest)
        {
            if (ModelState.IsValid)
            {
                db.RelatedTests.Add(relatedTest);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.TestId = new SelectList(db.Tests, "Id", "Title", relatedTest.TestId);
            ViewBag.RelatedTestId = new SelectList(db.Tests, "Id", "Title", relatedTest.RelatedTestId);
            return View(relatedTest);
        }

        // GET: RelatedTests/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelatedTest relatedTest = db.RelatedTests.Find(id);
            if (relatedTest == null)
            {
                return HttpNotFound();
            }
            ViewBag.TestId = new SelectList(db.Tests, "Id", "Title", relatedTest.TestId);
            ViewBag.RelatedTestId = new SelectList(db.Tests, "Id", "Title", relatedTest.RelatedTestId);
            return View(relatedTest);
        }

        // POST: RelatedTests/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,TestId,RelatedTestId,SortOrder")] RelatedTest relatedTest)
        {
            if (ModelState.IsValid)
            {
                db.Entry(relatedTest).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.TestId = new SelectList(db.Tests, "Id", "Title", relatedTest.TestId);
            ViewBag.RelatedTestId = new SelectList(db.Tests, "Id", "Title", relatedTest.RelatedTestId);
            return View(relatedTest);
        }

        // GET: RelatedTests/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            RelatedTest relatedTest = db.RelatedTests.Find(id);
            if (relatedTest == null)
            {
                return HttpNotFound();
            }
            return View(relatedTest);
        }

        // POST: RelatedTests/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            RelatedTest relatedTest = db.RelatedTests.Find(id);
            db.RelatedTests.Remove(relatedTest);
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
