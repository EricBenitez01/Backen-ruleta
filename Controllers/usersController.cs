using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http.Cors;
using System.Web.Mvc;
using backend_Ruleta.Models;

namespace backend_Ruleta.Controllers
{
    [EnableCors("*", "*", "*")]
    public class UsersController : Controller
    {
        private ruletaEntities db = new ruletaEntities();

        // GET: users
        public ActionResult Index()
        {
            var users = db.user.ToList();
            var response = new
            {
                status = "ok",
                data = users
            };

            return Json(response, JsonRequestBehavior.AllowGet);
        }

        // GET: users/Details/5
        public ActionResult Details(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.user.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // GET: users/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: users/Create
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        [HttpOptions]
        [Route("users/CreateOrUpdateUser")]
        public ActionResult Options()
        {
            return new HttpStatusCodeResult(HttpStatusCode.OK);
        }


        [EnableCors(origins: "*", headers: "*", methods: "*")]
        [HttpPost]
        [Route("users/CreateOrUpdateUser")]
        public ActionResult CreateOrUpdateUser(user newUser)
        {
            if (!ModelState.IsValid)
            {
                return Json(new { Error = "Error al crear o actualizar el usuario" });
            }

            var existingUser = db.user.FirstOrDefault(u => u.name.ToLower() == newUser.name.ToLower());

            if (existingUser != null)
            {
                return Json(existingUser, JsonRequestBehavior.AllowGet);
            }
            else
            {
                View();
                db.user.Add(newUser);
                db.SaveChanges();
                var response = new
                {
                    status = "ok",
                    data = newUser
                };

                return Json(response, JsonRequestBehavior.AllowGet);
            }
        }


        // GET: users/Edit/5
        public ActionResult Edit(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.user.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: users/Edit/5
        // Para protegerse de ataques de publicación excesiva, habilite las propiedades específicas a las que quiere enlazarse. Para obtener 
        // más detalles, vea https://go.microsoft.com/fwlink/?LinkId=317598.
        public ActionResult UpdateUser(string name, decimal newAmount)
        {
            var user = db.user.FirstOrDefault(u => u.name.ToLower() == name.ToLower());

            if (user != null)
            {
                user.amount = (int)newAmount;
                db.SaveChanges();

                return Json(user, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { Error = "Usuario no encontrado" });
            }
        }

        private Random random = new Random();
        public ActionResult Random()
        {
            int numeroAleatorio = random.Next(0, 37);
            string colorAleatorio = ObtenerColorAzar();

            var resultado = new { Numero = numeroAleatorio, Color = colorAleatorio };
            return Json(resultado, JsonRequestBehavior.AllowGet);
        }

        private string ObtenerColorAzar()
        {
            Color color = Color.FromArgb(random.Next(256), random.Next(256), random.Next(256));

            if (color.GetBrightness() < 0.5)
            {
                return "Negro";
            }
            else
            {
                return "Rojo";
            }
        }

        // GET: users/Delete/5
        public ActionResult Delete(string id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            user user = db.user.Find(id);
            if (user == null)
            {
                return HttpNotFound();
            }
            return View(user);
        }

        // POST: users/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id)
        {
            user user = db.user.Find(id);
            db.user.Remove(user);
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
