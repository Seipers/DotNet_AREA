using Area.DAT;
using Area.Models;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Octokit;
using Microsoft.AspNetCore.Http;
using System.Linq;

namespace Area.Controllers
{
    public class ConnectionController : Controller
    {
        public ActionResult Connection()
        {
            if (String.IsNullOrEmpty(HttpContext.Session.GetString("username")))
                return Redirect("http://localhost:5000");
            return View();
        }
    }
}