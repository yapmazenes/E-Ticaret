using _191117Mvc_Template.App_Classes;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using ZeonTicaret.WebUI;
using ZeonTicaret.WebUI.Models;

namespace _191117Mvc_Template.Controllers
{
    public class KullaniciController : Controller
    {
        //
        // GET: /Kullanici/
        MembershipUserCollection users = Membership.GetAllUsers();
        public ActionResult Index()
        {

            return View(users);
        }
        Kullanici ku = new Kullanici();
        [AllowAnonymous]
        public ActionResult Ekle()
        {
            return View(ku);
        }

        public void KullaniciResimEkle(HttpPostedFileBase fileUpload, string kid)
        {
            int resimId = -1;
            if (fileUpload != null)
            {
                Image img = Image.FromStream(fileUpload.InputStream);
                int width = int.Parse(ConfigurationManager.AppSettings["KullaniciWidth"].ToString());
                int height = int.Parse(ConfigurationManager.AppSettings["KullaniciHeight"].ToString());
                string name = "/Content/AvatarResim/" + Guid.NewGuid() + Path.GetExtension(fileUpload.FileName);
                Bitmap bm = new Bitmap(img, width, height);
                bm.Save(Server.MapPath(name));
                Resim rsm = new Resim();
                rsm.KucukYol = name;         
                if (rsm.Id > 0)
                    resimId = rsm.Id;
                rsm.userKey = kid;
                Context.Baglanti.Resims.Add(rsm);
                Context.Baglanti.SaveChanges();

            }

        }


        [AllowAnonymous]
        [HttpPost]
        public ActionResult Ekle(Kullanici k, HttpPostedFileBase fileUpload)
        {
            MembershipCreateStatus durum;
            
             MembershipUser  usr= Membership.CreateUser(k.KullaniciAdi, k.Parola, k.Email, k.Gizlisoru, k.Gizlicevap,true, out durum);
             string id = usr.ProviderUserKey.ToString();
           KullaniciResimEkle(fileUpload,id);
                           
            string mesaj = "";
            switch (durum)
            {
                case MembershipCreateStatus.DuplicateEmail:
                    mesaj += "Email daha önce kullanılmış.";
                    k.Email = "";
                    break;
                case MembershipCreateStatus.DuplicateProviderUserKey:
                    break;
                case MembershipCreateStatus.DuplicateUserName:
                    mesaj += "Kullanıcı Adı daha önceden kullanılmış.";
                    k.KullaniciAdi = "";
                    break;
                case MembershipCreateStatus.InvalidAnswer:
                    mesaj += "Gizli soru cevabı geçersiz.";

                    break;
                case MembershipCreateStatus.InvalidEmail:
                    mesaj += "Email geçersiz.";

                    break;
                case MembershipCreateStatus.InvalidPassword:
                    mesaj += "Geçersiz parola";

                    break;
                case MembershipCreateStatus.InvalidProviderUserKey:
                    break;
                case MembershipCreateStatus.InvalidQuestion:
                    mesaj += "Geçersiz gizli soru";
                    break;
                case MembershipCreateStatus.InvalidUserName:
                    mesaj += "Geçersiz kullanıcı adı";
                    k.KullaniciAdi = "";
                    break;
                case MembershipCreateStatus.ProviderError:
                    mesaj += "Üye yönetimi sağlayıcı hatası";
                    break;
                case MembershipCreateStatus.Success:
                    break;
                case MembershipCreateStatus.UserRejected:
                    mesaj += "Kullanıcı engel hatası";
                    break;
                default:
                    break;
            }
            ViewBag.Mesaj = mesaj;
            if (durum == MembershipCreateStatus.Success)
                return RedirectToAction("Index");
            else
            {
                if (k != null)
                {
                    return View(k);
                }
                return View();
            }



        }

        //[Authorize(Users="Enes")]
        public ActionResult RolAta()
        {
            ViewBag.Roller = Roles.GetAllRoles();
            ViewBag.Kullanicilar = users;
            return View();
        }   
        //[Authorize(Roles = "Admin")]
        [HttpPost]
        public ActionResult RolAta(string Kullanici, string Rol)
        {
            Roles.AddUserToRole(Kullanici, Rol);
            Bildirim b = new Bildirim();
            b.Adi = "Sitedeki yetkinliğiniz değiştirildi";
            b.BildirimTarihi = DateTime.Now;
            b.Detay = "Sipariş Kargoda";
            Guid userID =(Guid) Membership.GetUser(Kullanici).ProviderUserKey;
            b.KullaniciID = (Guid)userID;
            Context.Baglanti.Bildirims.Add(b);
            Context.Baglanti.SaveChanges();
            return RedirectToAction("Index");
        }
        [HttpPost]
        public string Rolleri(string kadi)
        {
            List<String> roller = Roles.GetRolesForUser(kadi).ToList();
            string rol = "";
            foreach (string item in roller)
            {
                rol += item + "-";
            }
            if (rol.Length > 1)
            {
                rol = rol.Remove(rol.Length - 1, 1);
            }

            return rol;
        }
    }
}