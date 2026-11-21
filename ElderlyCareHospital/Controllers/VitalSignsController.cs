using System;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using ElderlyCareHospital.Models;

namespace ElderlyCareHospital.Controllers
{
    public class VitalSignsController : Controller
    {
        private MyDatabaseModels db = new MyDatabaseModels();

        // =========================================================
        // GET: VitalSigns
        // =========================================================
        public ActionResult Index()
        {
            var vitalSigns = db.VitalSigns
                .Include(v => v.Patient)
                .OrderByDescending(v => v.DateRecorded);

            return View(vitalSigns.ToList());
        }


        // =========================================================
        // GET: VitalSigns/GetLiveVitalSigns
        // =========================================================
        public ActionResult GetLiveVitalSigns()
        {
            var vitalSigns = db.VitalSigns
                .Include(v => v.Patient)
                .OrderByDescending(v => v.DateRecorded)
                .ToList();

            return PartialView("_VitalSignRows", vitalSigns);
        }


        // =========================================================
        // GET: VitalSigns/Details/5
        // =========================================================
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest,
                    "Vital sign ID is required."
                );
            }

            VitalSign vitalSign = db.VitalSigns
                .Include(v => v.Patient)
                .FirstOrDefault(v => v.VitalID == id);

            if (vitalSign == null)
            {
                return HttpNotFound();
            }

            return View(vitalSign);
        }


        // =========================================================
        // GET: VitalSigns/Create
        // =========================================================
        // GET: VitalSigns/Create
        public ActionResult Create()
        {
            var vitalSign = new VitalSign
            {
                DateRecorded = DateTime.Now
            };

            ViewBag.PatientID = new SelectList(
                db.Patients,
                "PatientID",
                "FirstName"
            );

            return View(vitalSign);
        }


        // =========================================================
        // POST: VitalSigns/Create
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(
            [Bind(Include =
                "VitalID,Temperature,BloodPressure,PulseRate,OxygenLevel,SugarLevel,Weight,DateRecorded,PatientID")]
            VitalSign vitalSign)
        {
            // -----------------------------------------------------
            // PATIENT VALIDATION
            // -----------------------------------------------------

            if (vitalSign.PatientID <= 0)
            {
                ModelState.AddModelError(
                    "PatientID",
                    "Please select a patient before recording vital signs."
                );
            }
            else
            {
                bool patientExists = db.Patients.Any(
                    p => p.PatientID == vitalSign.PatientID
                );

                if (!patientExists)
                {
                    ModelState.AddModelError(
                        "PatientID",
                        "The selected patient does not exist."
                    );
                }
            }


            // -----------------------------------------------------
            // TEMPERATURE VALIDATION
            // -----------------------------------------------------

            string temperatureText =
                Convert.ToString(vitalSign.Temperature)?.Trim();

            if (string.IsNullOrWhiteSpace(temperatureText))
            {
                ModelState.AddModelError(
                    "Temperature",
                    "Temperature is required."
                );
            }
            else
            {
                decimal temperature;

                if (!decimal.TryParse(
                    temperatureText,
                    out temperature))
                {
                    ModelState.AddModelError(
                        "Temperature",
                        "Temperature must be a valid number, for example 36.8."
                    );
                }
                else if (temperature < 25 || temperature > 45)
                {
                    ModelState.AddModelError(
                        "Temperature",
                        "Temperature must be between 25°C and 45°C."
                    );
                }
            }


            // -----------------------------------------------------
            // BLOOD PRESSURE VALIDATION
            // -----------------------------------------------------

            string bloodPressure =
                Convert.ToString(vitalSign.BloodPressure)?.Trim();

            if (string.IsNullOrWhiteSpace(bloodPressure))
            {
                ModelState.AddModelError(
                    "BloodPressure",
                    "Blood pressure is required."
                );
            }
            else
            {
                Match bpMatch = Regex.Match(
                    bloodPressure,
                    @"^\s*(\d{2,3})\s*\/\s*(\d{2,3})\s*$"
                );

                if (!bpMatch.Success)
                {
                    ModelState.AddModelError(
                        "BloodPressure",
                        "Enter blood pressure in the correct format, for example 120/80."
                    );
                }
                else
                {
                    int systolic;
                    int diastolic;

                    int.TryParse(
                        bpMatch.Groups[1].Value,
                        out systolic
                    );

                    int.TryParse(
                        bpMatch.Groups[2].Value,
                        out diastolic
                    );

                    if (systolic < 70 || systolic > 250)
                    {
                        ModelState.AddModelError(
                            "BloodPressure",
                            "Systolic blood pressure must be between 70 and 250 mmHg."
                        );
                    }

                    if (diastolic < 40 || diastolic > 150)
                    {
                        ModelState.AddModelError(
                            "BloodPressure",
                            "Diastolic blood pressure must be between 40 and 150 mmHg."
                        );
                    }

                    if (diastolic >= systolic)
                    {
                        ModelState.AddModelError(
                            "BloodPressure",
                            "Diastolic pressure must be lower than systolic pressure."
                        );
                    }
                }
            }


            // -----------------------------------------------------
            // PULSE RATE VALIDATION
            // -----------------------------------------------------

            string pulseText =
                Convert.ToString(vitalSign.PulseRate)?.Trim();

            if (string.IsNullOrWhiteSpace(pulseText))
            {
                ModelState.AddModelError(
                    "PulseRate",
                    "Pulse rate is required."
                );
            }
            else
            {
                decimal pulse;

                if (!decimal.TryParse(
                    pulseText,
                    out pulse))
                {
                    ModelState.AddModelError(
                        "PulseRate",
                        "Pulse rate must be a valid number."
                    );
                }
                else if (pulse < 20 || pulse > 250)
                {
                    ModelState.AddModelError(
                        "PulseRate",
                        "Pulse rate must be between 20 and 250 bpm."
                    );
                }
            }


            // -----------------------------------------------------
            // OXYGEN LEVEL VALIDATION
            // -----------------------------------------------------

            string oxygenText =
                Convert.ToString(vitalSign.OxygenLevel)?.Trim();

            if (string.IsNullOrWhiteSpace(oxygenText))
            {
                ModelState.AddModelError(
                    "OxygenLevel",
                    "Oxygen saturation level is required."
                );
            }
            else
            {
                decimal oxygen;

                if (!decimal.TryParse(
                    oxygenText,
                    out oxygen))
                {
                    ModelState.AddModelError(
                        "OxygenLevel",
                        "Oxygen level must be a valid number."
                    );
                }
                else if (oxygen < 50 || oxygen > 100)
                {
                    ModelState.AddModelError(
                        "OxygenLevel",
                        "Oxygen level must be between 50% and 100%."
                    );
                }
            }


            // -----------------------------------------------------
            // SUGAR LEVEL VALIDATION
            // -----------------------------------------------------

            string sugarText =
                Convert.ToString(vitalSign.SugarLevel)?.Trim();

            if (string.IsNullOrWhiteSpace(sugarText))
            {
                ModelState.AddModelError(
                    "SugarLevel",
                    "Blood sugar level is required."
                );
            }
            else
            {
                decimal sugar;

                if (!decimal.TryParse(
                    sugarText,
                    out sugar))
                {
                    ModelState.AddModelError(
                        "SugarLevel",
                        "Blood sugar level must be a valid number."
                    );
                }
                else if (sugar < 0.5m || sugar > 40)
                {
                    ModelState.AddModelError(
                        "SugarLevel",
                        "Blood sugar level must be between 0.5 and 40 mmol/L."
                    );
                }
            }


            // -----------------------------------------------------
            // WEIGHT VALIDATION
            // -----------------------------------------------------

            string weightText =
                Convert.ToString(vitalSign.Weight)?.Trim();

            if (string.IsNullOrWhiteSpace(weightText))
            {
                ModelState.AddModelError(
                    "Weight",
                    "Weight is required."
                );
            }
            else
            {
                decimal weight;

                if (!decimal.TryParse(
                    weightText,
                    out weight))
                {
                    ModelState.AddModelError(
                        "Weight",
                        "Weight must be a valid number."
                    );
                }
                else if (weight <= 0)
                {
                    ModelState.AddModelError(
                        "Weight",
                        "Weight must be greater than 0 kg."
                    );
                }
                else if (weight > 500)
                {
                    ModelState.AddModelError(
                        "Weight",
                        "Please enter a realistic patient weight below 500 kg."
                    );
                }
            }


            // -----------------------------------------------------
            // DATE AND TIME VALIDATION
            // -----------------------------------------------------

            DateTime recordedDate;

            if (vitalSign.DateRecorded == null)
            {
                vitalSign.DateRecorded = DateTime.Now;
            }

            recordedDate = Convert.ToDateTime(
                vitalSign.DateRecorded
            );

            // Do not allow a future date/time.
            if (recordedDate > DateTime.Now.AddMinutes(1))
            {
                ModelState.AddModelError(
                    "DateRecorded",
                    "The recorded date and time cannot be in the future."
                );
            }

            // Do not allow an obviously invalid/default date.
            if (recordedDate == DateTime.MinValue)
            {
                ModelState.AddModelError(
                    "DateRecorded",
                    "A valid date and time is required."
                );
            }


            // -----------------------------------------------------
            // SAVE
            // -----------------------------------------------------

            if (ModelState.IsValid)
            {
                db.VitalSigns.Add(vitalSign);
                db.SaveChanges();

                TempData["Success"] =
                    "Vital signs were recorded successfully.";

                return RedirectToAction("Index");
            }


            // -----------------------------------------------------
            // RETURN FORM WITH VALIDATION ERRORS
            // -----------------------------------------------------

            ViewBag.PatientID = new SelectList(
                db.Patients
                    .OrderBy(p => p.FirstName)
                    .ToList(),
                "PatientID",
                "FirstName",
                vitalSign.PatientID
            );

            return View(vitalSign);
        }


        // =========================================================
        // GET: VitalSigns/Edit/5
        // =========================================================
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }

            VitalSign vitalSign = db.VitalSigns.Find(id);

            if (vitalSign == null)
            {
                return HttpNotFound();
            }

            ViewBag.PatientID = new SelectList(
                db.Patients
                    .OrderBy(p => p.FirstName)
                    .ToList(),
                "PatientID",
                "FirstName",
                vitalSign.PatientID
            );

            return View(vitalSign);
        }


        // =========================================================
        // POST: VitalSigns/Edit/5
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            [Bind(Include =
                "VitalID,Temperature,BloodPressure,PulseRate,OxygenLevel,SugarLevel,Weight,DateRecorded,PatientID")]
            VitalSign vitalSign)
        {
            // Patient validation
            if (vitalSign.PatientID <= 0)
            {
                ModelState.AddModelError(
                    "PatientID",
                    "Please select a valid patient."
                );
            }


            // Temperature validation
            string temperatureText =
                Convert.ToString(vitalSign.Temperature)?.Trim();

            decimal temperature;

            if (string.IsNullOrWhiteSpace(temperatureText))
            {
                ModelState.AddModelError(
                    "Temperature",
                    "Temperature is required."
                );
            }
            else if (!decimal.TryParse(
                temperatureText,
                out temperature))
            {
                ModelState.AddModelError(
                    "Temperature",
                    "Temperature must be a valid number."
                );
            }
            else if (temperature < 25 || temperature > 45)
            {
                ModelState.AddModelError(
                    "Temperature",
                    "Temperature must be between 25°C and 45°C."
                );
            }


            // Blood pressure validation
            string bloodPressure =
                Convert.ToString(vitalSign.BloodPressure)?.Trim();

            Match bpMatch = null;

            if (string.IsNullOrWhiteSpace(bloodPressure))
            {
                ModelState.AddModelError(
                    "BloodPressure",
                    "Blood pressure is required."
                );
            }
            else
            {
                bpMatch = Regex.Match(
                    bloodPressure,
                    @"^\s*(\d{2,3})\s*\/\s*(\d{2,3})\s*$"
                );

                if (!bpMatch.Success)
                {
                    ModelState.AddModelError(
                        "BloodPressure",
                        "Enter blood pressure in the correct format, for example 120/80."
                    );
                }
                else
                {
                    int systolic;
                    int diastolic;

                    int.TryParse(
                        bpMatch.Groups[1].Value,
                        out systolic
                    );

                    int.TryParse(
                        bpMatch.Groups[2].Value,
                        out diastolic
                    );

                    if (systolic < 70 || systolic > 250)
                    {
                        ModelState.AddModelError(
                            "BloodPressure",
                            "Systolic pressure must be between 70 and 250 mmHg."
                        );
                    }

                    if (diastolic < 40 || diastolic > 150)
                    {
                        ModelState.AddModelError(
                            "BloodPressure",
                            "Diastolic pressure must be between 40 and 150 mmHg."
                        );
                    }

                    if (diastolic >= systolic)
                    {
                        ModelState.AddModelError(
                            "BloodPressure",
                            "Diastolic pressure must be lower than systolic pressure."
                        );
                    }
                }
            }


            // Pulse validation
            string pulseText =
                Convert.ToString(vitalSign.PulseRate)?.Trim();

            decimal pulse;

            if (string.IsNullOrWhiteSpace(pulseText))
            {
                ModelState.AddModelError(
                    "PulseRate",
                    "Pulse rate is required."
                );
            }
            else if (!decimal.TryParse(
                pulseText,
                out pulse))
            {
                ModelState.AddModelError(
                    "PulseRate",
                    "Pulse rate must be a valid number."
                );
            }
            else if (pulse < 20 || pulse > 250)
            {
                ModelState.AddModelError(
                    "PulseRate",
                    "Pulse rate must be between 20 and 250 bpm."
                );
            }


            // Oxygen validation
            string oxygenText =
                Convert.ToString(vitalSign.OxygenLevel)?.Trim();

            decimal oxygen;

            if (string.IsNullOrWhiteSpace(oxygenText))
            {
                ModelState.AddModelError(
                    "OxygenLevel",
                    "Oxygen level is required."
                );
            }
            else if (!decimal.TryParse(
                oxygenText,
                out oxygen))
            {
                ModelState.AddModelError(
                    "OxygenLevel",
                    "Oxygen level must be a valid number."
                );
            }
            else if (oxygen < 50 || oxygen > 100)
            {
                ModelState.AddModelError(
                    "OxygenLevel",
                    "Oxygen level must be between 50% and 100%."
                );
            }


            // Sugar validation
            string sugarText =
                Convert.ToString(vitalSign.SugarLevel)?.Trim();

            decimal sugar;

            if (string.IsNullOrWhiteSpace(sugarText))
            {
                ModelState.AddModelError(
                    "SugarLevel",
                    "Blood sugar level is required."
                );
            }
            else if (!decimal.TryParse(
                sugarText,
                out sugar))
            {
                ModelState.AddModelError(
                    "SugarLevel",
                    "Blood sugar level must be a valid number."
                );
            }
            else if (sugar < 0.5m || sugar > 40)
            {
                ModelState.AddModelError(
                    "SugarLevel",
                    "Blood sugar level must be between 0.5 and 40 mmol/L."
                );
            }


            // Weight validation
            string weightText =
                Convert.ToString(vitalSign.Weight)?.Trim();

            decimal weight;

            if (string.IsNullOrWhiteSpace(weightText))
            {
                ModelState.AddModelError(
                    "Weight",
                    "Weight is required."
                );
            }
            else if (!decimal.TryParse(
                weightText,
                out weight))
            {
                ModelState.AddModelError(
                    "Weight",
                    "Weight must be a valid number."
                );
            }
            else if (weight <= 0 || weight > 500)
            {
                ModelState.AddModelError(
                    "Weight",
                    "Weight must be greater than 0 kg and below 500 kg."
                );
            }


            // Date validation
            if (vitalSign.DateRecorded == null)
            {
                vitalSign.DateRecorded = DateTime.Now;
            }

            DateTime dateRecorded =
                Convert.ToDateTime(vitalSign.DateRecorded);

            if (dateRecorded > DateTime.Now.AddMinutes(1))
            {
                ModelState.AddModelError(
                    "DateRecorded",
                    "The recorded date and time cannot be in the future."
                );
            }


            if (ModelState.IsValid)
            {
                db.Entry(vitalSign).State =
                    EntityState.Modified;

                db.SaveChanges();

                TempData["Success"] =
                    "Vital signs were updated successfully.";

                return RedirectToAction("Index");
            }


            ViewBag.PatientID = new SelectList(
                db.Patients
                    .OrderBy(p => p.FirstName)
                    .ToList(),
                "PatientID",
                "FirstName",
                vitalSign.PatientID
            );

            return View(vitalSign);
        }


        // =========================================================
        // GET: VitalSigns/Delete/5
        // =========================================================
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(
                    HttpStatusCode.BadRequest
                );
            }

            VitalSign vitalSign = db.VitalSigns
                .Include(v => v.Patient)
                .FirstOrDefault(v => v.VitalID == id);

            if (vitalSign == null)
            {
                return HttpNotFound();
            }

            return View(vitalSign);
        }


        // =========================================================
        // POST: VitalSigns/Delete/5
        // =========================================================
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            VitalSign vitalSign =
                db.VitalSigns.Find(id);

            if (vitalSign == null)
            {
                return HttpNotFound();
            }

            db.VitalSigns.Remove(vitalSign);
            db.SaveChanges();

            TempData["Success"] =
                "Vital signs were deleted successfully.";

            return RedirectToAction("Index");
        }


        // =========================================================
        // DISPOSE
        // =========================================================
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