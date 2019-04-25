using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using HMS.Models;
using System.Dynamic;

namespace HMS.Controllers
{
    public class ExpertisesController : Controller
    {
        private HospitalEntities2 db = new HospitalEntities2();

        // GET: Expertises
        public ActionResult Index()
        {
            return View(db.Expertises.ToList());
        }

        // GET: Expertises/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expertise expertise = db.Expertises.Find(id);
            if (expertise == null)
            {
                return HttpNotFound();
            }
            return View(expertise);
        }

        // GET: Expertises/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Expertises/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ExpertiseID,Speciality")] Expertise expertise)
        {
            if (ModelState.IsValid)
            {
                db.Expertises.Add(expertise);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(expertise);
        }

        // GET: Expertises/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expertise expertise = db.Expertises.Find(id);
            if (expertise == null)
            {
                return HttpNotFound();
            }
            return View(expertise);
        }

        // POST: Expertises/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ExpertiseID,Speciality")] Expertise expertise)
        {
            if (ModelState.IsValid)
            {
                db.Entry(expertise).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(expertise);
        }

        // GET: Expertises/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Expertise expertise = db.Expertises.Find(id);
            if (expertise == null)
            {
                return HttpNotFound();
            }
            return View(expertise);
        }

        // POST: Expertises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Expertise expertise = db.Expertises.Find(id);
            db.Expertises.Remove(expertise);
            db.SaveChanges();
            return RedirectToAction("Index");
        }










        // My code starts here

        public ActionResult ExpertiseList()
        {
            HospitalEntities2 entities = new HospitalEntities2();
            List<Expertise> expertises = entities.Expertises.ToList();
            
            //Add a Dummy Row.
            expertises.Insert(0, new Expertise());
            if (User.IsInRole("Admin"))
                return View(expertises);
            return View("alt_expertise", expertises);
           
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public JsonResult InsertExpertise(Expertise expertise)
        {
            int value = -1;
            using (HospitalEntities2 entities = new HospitalEntities2())
            {

                value = int.Parse(entities.Expertises
                          .OrderByDescending(p => p.ExpertiseID)
                          .Select(r => r.ExpertiseID)
                          .First().ToString());
                expertise.ExpertiseID = value + 1;
                expertise.Speciality = expertise.Speciality.Trim();
                entities.Expertises.Add(expertise);
                entities.SaveChanges();
            }

            return Json(expertise);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteExpertise(int expertiseId)
        {
            using (HospitalEntities2 entities = new HospitalEntities2())
            {
                Expertise expertise = (from c in entities.Expertises
                                 where c.ExpertiseID == expertiseId
                                 select c).FirstOrDefault();
                entities.Expertises.Remove(expertise);
                entities.SaveChanges();
            }

            return new EmptyResult();
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateExpertise(Expertise expertise)
        {
            using (HospitalEntities2 entities = new HospitalEntities2())
            {
                Expertise updatedExpertise = (from c in entities.Expertises
                                        where c.ExpertiseID == expertise.ExpertiseID
                                        select c).FirstOrDefault();

                updatedExpertise.Speciality = expertise.Speciality;
                
                entities.SaveChanges();
            }

            return new EmptyResult();

        }










        
        public ActionResult Find(String degree)
        {
            dynamic mymodel = new ExpandoObject();
            var expert = db.Expertises.Where(x => x.Speciality == degree).FirstOrDefault();
            var id = expert.ExpertiseID;
            var data = db.Doctors.Where(x => x.Degree == id).ToList();
            List<Expertise> temp = new List<Expertise>();
            temp.Add(expert);
            mymodel.doctors = data;
            mymodel.expertises = temp;


            return View("~/Views/Doctors/alt_doctor.cshtml", mymodel); 
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
