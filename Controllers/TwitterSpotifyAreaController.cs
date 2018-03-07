using System;
using System.Collections.Generic;
using System.Linq;
using Area.DAT;
using Area.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Area.Controllers
{
    public class TwitterSpotifyAreaController : Controller
    {
        public ActionResult TwitterSpotifyAreaCreation(string action_index, string reaction_index, [FromServices] AreaDbContext DB)
        {
            int index1;
            int index2;

            if (String.IsNullOrEmpty(HttpContext.Session.GetString("username")))
                return Redirect("http://localhost:5000");
            if (Int32.TryParse(action_index, out index1) && Int32.TryParse(reaction_index, out index2))
            {
                if (index1 >= 0 && index1 <= 1 && index2 >= 0 && index2 <= 5)
                {
                    DB.areas.Add(new Models.AREA {type=5, username=HttpContext.Session.GetString("username"), index_action=index1,
                    index_reaction=index2, last_event=""});
                    DB.SaveChanges();
                }
                else
                {
                    ViewData["Message"] = "Please select an action and a reaction to create the Area";
                    return View();
                } 
            }
            else
            {   
                ViewData["Message"] = "Please select an action and a reaction to create the Area";
                return View();
            }
            return RedirectToAction("ManageArea", "ManageArea");
        }
        public ActionResult TwitterSpotifyAreaDeletion(string action_index, string reaction_index, [FromServices] AreaDbContext DB)
        {
            string username = HttpContext.Session.GetString("username");
            if (String.IsNullOrEmpty(username))
                return Redirect("http://localhost:5000");
            List<AREA> areas = DB.areas.ToList();
            foreach (var area in areas)
            {
                if (area.type == 5 && area.username == username)
                    {
                        DB.areas.Remove(area);
                        DB.SaveChanges();
                    }
            }
            return RedirectToAction("ManageArea", "ManageArea");
        }   
    }
}