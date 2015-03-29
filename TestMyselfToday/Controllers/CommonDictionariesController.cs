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
    public class CommonDictionariesController : Controller
    {
        private TMTEntities db = new TMTEntities();

        // GET: CommonDictionaries
        public ActionResult Index()
        {
            var commonDictionaries = db.CommonDictionaries.Include(c => c.Language).OrderBy(o => o.LanguageId);
            return View(commonDictionaries.ToList());
        }

        // GET: CommonDictionaries/Details/5
        public ActionResult Details(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommonDictionary commonDictionary = db.CommonDictionaries.Find(id);
            if (commonDictionary == null)
            {
                return HttpNotFound();
            }
            return View(commonDictionary);
        }

        // GET: CommonDictionaries/Create
        public ActionResult Create()
        {
            ViewBag.LanguageId = new SelectList(db.Languages, "Id", "Title");
            return View();
        }

        // POST: CommonDictionaries/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CDKey,CDValue,LanguageId,SortOrder")] CommonDictionary commonDictionary)
        {
            if (ModelState.IsValid)
            {
                db.CommonDictionaries.Add(commonDictionary);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.LanguageId = new SelectList(db.Languages, "Id", "Title", commonDictionary.LanguageId);
            return View(commonDictionary);
        }

        // GET: CommonDictionaries/Edit/5
        public ActionResult Edit(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommonDictionary commonDictionary = db.CommonDictionaries.Find(id);
            if (commonDictionary == null)
            {
                return HttpNotFound();
            }
            ViewBag.LanguageId = new SelectList(db.Languages, "Id", "Title", commonDictionary.LanguageId);
            return View(commonDictionary);
        }

        // POST: CommonDictionaries/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CDKey,CDValue,LanguageId,SortOrder")] CommonDictionary commonDictionary)
        {
            if (ModelState.IsValid)
            {
                db.Entry(commonDictionary).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.LanguageId = new SelectList(db.Languages, "Id", "Title", commonDictionary.LanguageId);
            return View(commonDictionary);
        }

        // GET: CommonDictionaries/Delete/5
        public ActionResult Delete(long? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CommonDictionary commonDictionary = db.CommonDictionaries.Find(id);
            if (commonDictionary == null)
            {
                return HttpNotFound();
            }
            return View(commonDictionary);
        }

        // POST: CommonDictionaries/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            CommonDictionary commonDictionary = db.CommonDictionaries.Find(id);
            db.CommonDictionaries.Remove(commonDictionary);
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
