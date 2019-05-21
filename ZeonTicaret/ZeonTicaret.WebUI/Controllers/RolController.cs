using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace _191117Mvc_Template.Controllers
{
    [Authorize]
    public class RolController : Controller
    {
        //
        // GET: /Rol/
        public ActionResult Index()
        {
          List<String> roller=Roles.GetAllRoles().ToList();
          return View(roller);
        }
        public ActionResult Ekle()
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult Ekle(string Roladi)
        {
            Roles.CreateRole(Roladi);
            return RedirectToAction("Index");
        }
        public void RolSil(string rolAdi)
        {
            Roles.DeleteRole(rolAdi);
        }
	}
}