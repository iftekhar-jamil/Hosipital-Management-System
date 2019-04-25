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
    public class DoctorsController : Controller
    {
        private HospitalEntities2 db = new HospitalEntities2();

        // GET: Doctors
        public ActionResult Index()
        {
            return View(db.Doctors.ToList());
        }

        // GET: Doctors/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // GET: Doctors/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Doctors/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "DoctorID,LastName,FirstName,BirthDate,Degree")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Doctors.Add(doctor);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(doctor);
        }

        // GET: Doctors/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "DoctorID,LastName,FirstName,BirthDate,Degree")] Doctor doctor)
        {
            if (ModelState.IsValid)
            {
                db.Entry(doctor).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(doctor);
        }









        // GET: Doctors/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Doctor doctor = db.Doctors.Find(id);
            if (doctor == null)
            {
                return HttpNotFound();
            }
            return View(doctor);
        }

        // POST: Doctors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Doctor doctor = db.Doctors.Find(id);
            db.Doctors.Remove(doctor);
            db.SaveChanges();
            return RedirectToAction("Index");
        }





        // My code starts here
        public ActionResult newIndex()
        {
            HospitalEntities2 entities = new HospitalEntities2();
            List<Doctor> doctors = entities.Doctors.ToList();
            foreach (var doctor in doctors) {
                    
                   var a = (from c in entities.Expertises
                                    where c.ExpertiseID == doctor.Degree
                                    select c.Speciality).FirstOrDefault();
                
            }

            //Add a Dummy Row.
            doctors.Insert(0, new Doctor());
            return View(doctors);
        }

        public ActionResult DoctorList()
        {
            HospitalEntities2 entities = new HospitalEntities2();
            List<Doctor> doctors = entities.Doctors.ToList();
            List<Expertise> expertises = entities.Expertises.ToList();
            dynamic mymodel = new ExpandoObject();
            /*foreach (var doctor in doctors)
            {

                String degree = (from c in entities.Expertises
                                 where c.ExpertiseID == doctor.Degree
                                 select c.Speciality).FirstOrDefault();
                if (degree == null) continue;
              //  mymodel.Degree = degree;
            }*/


            //Add a Dummy Row.
            
            doctors.Insert(0, new Doctor());
            mymodel.doctors = doctors;
            mymodel.expertises = expertises;
            if (User.IsInRole("Admin"))
                return View(mymodel);
            return View("alt_doctor", mymodel);
            //return View(doctors);
        }

         public ActionResult Adapter(int id) {
            return RedirectToAction("viewDetails",new {id=-0, altId=id});
        }


        public ActionResult viewDetails(int? id,int? altId)
        {
            
            HospitalEntities2 entities = new HospitalEntities2();
            dynamic mymodel = new ExpandoObject();
            mymodel.doctors = entities.Doctors.ToList();
            mymodel.change = false;
            mymodel.altId = null;

            if (id == null || id==0) {
                if (altId != null)
                {
                    mymodel.change = true;
                    mymodel.altId = altId;
                }
                mymodel.select = false;
                if (User.IsInRole("Admin"))
                    return View(mymodel);
                return View("detailsList", mymodel);
            }
            Doctor doctor = db.Doctors.Find(id);
          
            var weekday = entities.Weekdays.ToList();
            var workdaysid = (from c in entities.DoctorsWorkingDays
             where c.DoctorID == id
             select c.WeekID).ToList();
            var workdays = new List<String>();
            foreach (var i in workdaysid) {
                workdays.Add(weekday[i-1].Name); 
            } 
           
            mymodel.doctor = doctor;
            mymodel.weekdays = weekday;
            mymodel.workdays = workdays;
            mymodel.select = true;
            if (User.IsInRole("Admin"))
                return PartialView("_schedule",mymodel);
            return View("_alt_schedule",mymodel);
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult updateDates(String days1, String days2, String days3, String days4, String days5, String days6, String days7, String doctorID)
        {
            HospitalEntities2 entities = new HospitalEntities2();
            var days = new List<String>();
            days.Add(days1); days.Add(days2); days.Add(days3); days.Add(days4);
            days.Add(days5); days.Add(days6); days.Add(days7); 
            var list = new List<int>();
            foreach (var day in days)
            {
                if (day != null) list.Add((from c in entities.Weekdays
                                           where c.Name == day
                                           select c.WeekId).FirstOrDefault());
            }

            for (var i = 1; i <= 7; i++)
            {
                if (!list.Contains(i)) {
                    DoctorsWorkingDay dwd = (from c in entities.DoctorsWorkingDays
                                             where c.DoctorID.ToString() == doctorID
                                             && c.WeekID == i
                                             select c).FirstOrDefault();
                    if (dwd == null) continue;
                    entities.DoctorsWorkingDays.Remove(dwd);
                    entities.SaveChanges();
                }

                

            }

            foreach (var workday in list)
            {
                DoctorsWorkingDay dwd = (from c in entities.DoctorsWorkingDays
                                         where c.DoctorID.ToString() == doctorID
                                         && c.WeekID == workday
                                         select c).FirstOrDefault();
                if (dwd == null)
                {
                    int value = int.Parse(entities.DoctorsWorkingDays
                            .OrderByDescending(p => p.Id)
                            .Select(r => r.Id)
                            .First().ToString());
                    entities.DoctorsWorkingDays.Add(new DoctorsWorkingDay() {Id=value+1, DoctorID = Int32.Parse(doctorID), WeekID = workday });
                    entities.SaveChanges();    
                }    
            }
            //Redirect("viewDetails");
            dynamic mymodel = new ExpandoObject();
            mymodel.doctors = entities.Doctors.ToList();
            mymodel.change = false;
            mymodel.altId = null;
            mymodel.select = false;
            return View("~/Views/Doctors/viewDetails.cshtml",mymodel);
        }



        [HttpPost]
        [Authorize(Roles ="Admin")]
        public JsonResult InsertDoctor(Doctor doctor)
        {
            int value = 0;
            var temp = doctor.BirthDate;
            using (HospitalEntities2 entities = new HospitalEntities2())
            {
                
                doctor.BirthDate = doctor.BirthDate.ToString();
                /*                int temp = doctor.Degree;
                                var id = (from c in entities.Expertises
                                                 where c.ExpertiseID == doctor.Degree
                                                 select c.ExpertiseID).FirstOrDefault();
                                doctor.Degree = id; */

                value = int.Parse(entities.Doctors
                          .OrderByDescending(p => p.DoctorID)
                          .Select(r => r.DoctorID)
                          .First().ToString());
                if(doctor.DoctorID==0)
                doctor.DoctorID = value + 1;
                entities.Doctors.Add(doctor);
                
                entities.SaveChanges();
               // doctor.Degree = temp;
            }
            //Doctor newdoctor = db.Doctors.Find(value + 1);
            doctor.BirthDate = temp;
            return Json(doctor, JsonRequestBehavior.AllowGet);
        }


        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult DeleteDoctor(int doctorId)
        {
            using (HospitalEntities2 entities = new HospitalEntities2())
            {
                Doctor doctor = (from c in entities.Doctors
                                     where c.DoctorID == doctorId
                                     select c).FirstOrDefault();
                entities.Doctors.Remove(doctor);
                entities.SaveChanges();
            }

            return new EmptyResult();
        }



        [HttpPost]
        [Authorize(Roles = "Admin")]
        public ActionResult UpdateDoctor(Doctor doctor)
        {
            using (HospitalEntities2 entities = new HospitalEntities2())
            {
                Doctor updatedDoctor = (from c in entities.Doctors
                                        where c.DoctorID == doctor.DoctorID
                                        select c).FirstOrDefault();

                updatedDoctor.FirstName = doctor.FirstName.Trim();
                updatedDoctor.LastName = doctor.LastName.Trim();
                updatedDoctor.BirthDate = doctor.BirthDate.ToString().Trim();
                updatedDoctor.Degree = doctor.Degree;
                //InsertDoctor(updatedDoctor);
                entities.SaveChanges();
            }

            return new EmptyResult();

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
