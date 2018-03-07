using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Area.Models;
using Area.DAT;
using Microsoft.AspNetCore.Http;
using System.Net.Mail;

namespace Area.Controllers
{
    class ClientGmail
    {
        public void SendMail(string mailAddr, string code)
        {
            MailMessage msg = new MailMessage();
            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient();
            try
            {
                msg.Subject = "Confirmation nouveau compte Area";
                msg.Body = "Voici le code de confirmation pour la création de votre nouveau compte : " + code;
                msg.From = new MailAddress("area.confirmation@gmail.com");
                msg.To.Add(mailAddr);
                msg.IsBodyHtml = true;
                client.Host = "smtp.gmail.com";
                System.Net.NetworkCredential basicauthenticationinfo = new System.Net.NetworkCredential("area.confirmation@gmail.com", "qfm21579513");
                client.Port = int.Parse("587");
                client.EnableSsl = true;
                client.UseDefaultCredentials = false;
                client.Credentials = basicauthenticationinfo;
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Send(msg);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }
    }

    public class HomeController : Controller
    {
        private bool IsLog([FromServices] AreaDbContext DB)
        {
            string userName = "";
            string passwd = "";
            bool logFind = false;

            userName = HttpContext.Session.GetString("username");
            passwd = HttpContext.Session.GetString("passwd");

            var users = DB.users.ToList();
            foreach (var user in users)
            {
                if (user.username == userName
                    && user.password == passwd
                    && user.enabled)
                {
                    logFind = true;
                    break;
                }
            }
            return (logFind);
        }
        public IActionResult Index([FromServices] AreaDbContext DB)
        {
            if (IsLog(DB))
                return View();
            return RedirectToAction("Login");
        }
        public IActionResult Login([FromServices] AreaDbContext DB)
        {
            List<Models.ErrorLog> error = new List<Models.ErrorLog>();
            Models.ErrorLog elem = new Models.ErrorLog();
            elem.message = "Wrong User or Password";

            elem.enabl = !(String.IsNullOrEmpty(HttpContext.Session.GetString("username")) && String.IsNullOrEmpty(HttpContext.Session.GetString("password")));
            HttpContext.Session.SetString("username", "");
            HttpContext.Session.SetString("passwd", "");
            error.Add(elem);
            ViewData["ErrorLog"] = error;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Login([FromServices] AreaDbContext DB,  FormLogin model)
        {
            if (!String.IsNullOrEmpty(model.username) && !String.IsNullOrEmpty(model.password))
            {
                HttpContext.Session.SetString("username", model.username);
                HttpContext.Session.SetString("passwd", model.password);
            }
            return RedirectToAction("Index");            
        }

        public IActionResult Disconnect([FromServices] AreaDbContext dbContext)
        {
            HttpContext.Session.SetString("username", "");
            HttpContext.Session.SetString("passwd", "");
            return RedirectToAction("Index");
        }

        public IActionResult Register([FromServices] AreaDbContext DB)
        {
            List<Models.ErrorLog> error = new List<Models.ErrorLog>();
            Models.ErrorLog elem = new Models.ErrorLog();
            elem.message = "Passwords don't match";

            elem.enabl = !(HttpContext.Session.GetString("tmpPass") == HttpContext.Session.GetString("tmpConfirm"));
            HttpContext.Session.SetString("tmpConfirm", "");
            HttpContext.Session.SetString("tmpPass", "");
            error.Add(elem);
            ViewData["ErrorReg"] = error;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Register([FromServices] AreaDbContext DB,  FormRegister model)
        {
            if (!String.IsNullOrEmpty(model.username) && !String.IsNullOrEmpty(model.password)
                && !String.IsNullOrEmpty(model.confirmation) && !String.IsNullOrEmpty(model.email))
            {
                HttpContext.Session.SetString("tmpUser", model.username);
                HttpContext.Session.SetString("tmpPass", model.password);
                HttpContext.Session.SetString("tmpConfirm", model.confirmation);            
                HttpContext.Session.SetString("tmpEmail", model.email);
                if (model.password == model.confirmation)
                {
                    return RedirectToAction("ConfirmRegister");            
                }
            }
            return RedirectToAction("Register");
        }

        public IActionResult ConfirmRegister([FromServices] AreaDbContext DB)
        {
            List<Models.ErrorLog> error = new List<Models.ErrorLog>();
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("tmpCode")))
            {            
                ClientGmail client = new ClientGmail();
                Random rnd = new Random();
                string code = rnd.Next(1000, 9999).ToString();

                HttpContext.Session.SetString("tmpCode", code);
                client.SendMail(HttpContext.Session.GetString("tmpEmail"), code);
            }
            else
            {
                Models.ErrorLog elem = new Models.ErrorLog();
                elem.message = "Wrong code entered";

                elem.enabl = true;
                HttpContext.Session.SetString("tmpCode", "");
                error.Add(elem);
            }
            ViewData["ErrorConf"] = error;
            return View();
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult ConfirmRegister([FromServices] AreaDbContext DB,  FormCode model)
        {
            if (!String.IsNullOrEmpty(model.code))
            {
                if (model.code == HttpContext.Session.GetString("tmpCode"))
                {
                    DB.users.Add( new Models.User {username=HttpContext.Session.GetString("tmpUser"), password=HttpContext.Session.GetString("tmpPass"), email=HttpContext.Session.GetString("tmpEmail"), enabled=true});
                    DB.SaveChanges();
                    HttpContext.Session.SetString("tmpCode", "");
                    return RedirectToAction("Index");            
                }
            }
            return RedirectToAction("ConfirmRegister");
        }

        public  IActionResult ManageArea([FromServices] AreaDbContext DB)
        {
            if (IsLog(DB))
                return RedirectToAction("ManageArea", "ManageArea");
            return RedirectToAction("Login");
        }

        public IActionResult About([FromServices] AreaDbContext DB)
        {
            if (IsLog(DB))
                return View();            
            return RedirectToAction("Login");
        }

        public IActionResult Contact([FromServices] AreaDbContext DB)
        {
            if (IsLog(DB))
                return View();            
            return RedirectToAction("Login");
        }

        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
