using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Area.Models;
using Area.DAT;
using Area.Controllers;
using Microsoft.AspNetCore.Http;



namespace Area.Controllers
{
    public class ManageAreaController : Controller
    {
        private List<AreaType> getActiveAreas(AreaDbContext DB, string username)
        {
            List<AreaType> active_areatypes = new List<AreaType>();
            List<AREA> active_areas = DB.areas.ToList();
            foreach (var active_area in active_areas)
            {
                if (active_area.username == username)
                {
                    AreaType active_areatype = AreaType.findAreaTypeById(DB, active_area.type);
                    if (active_areatype != null)
                        active_areatypes.Add(active_areatype);
                }
            }
            return active_areatypes;
        }

        private List<AreaType> getAvailableAreas(AreaDbContext DB, string username)
        {   
            List<AreaType> available_areatypes = new List<AreaType>();
            List<AreaType> all_areatypes = DB.areatypes.ToList();
            AREA tmp = new AREA();
            tmp.username = username;
            tmp.index_action = 0;
            tmp.index_reaction = 0;
            tmp.last_event = "";
            foreach (var area_type in all_areatypes)
            {
                tmp.type = area_type.id;
                IArea type = AreaFactory.create(tmp, DB);
                if (type != null && type.isAvailable())
                    available_areatypes.Add(area_type);
            }
            return (available_areatypes);
        }

        private List<AreaType> getUnavailableAreas(AreaDbContext DB, string username)
        {   
            List<AreaType> unavailable_areatypes = new List<AreaType>();
            List<AreaType> all_areatypes = DB.areatypes.ToList();
            AREA tmp = new AREA();
            tmp.username = username;
            tmp.index_action = 0;
            tmp.index_reaction = 0;
            tmp.last_event = "";
            foreach (var area_type in all_areatypes)
            {
                tmp.type = area_type.id;
                IArea type = AreaFactory.create(tmp, DB);
                if (type != null && !type.isAvailable())
                    unavailable_areatypes.Add(area_type);
            }
            return (unavailable_areatypes);
        }

        public ActionResult ManageArea([FromServices] AreaDbContext DB)
        {
            string username = HttpContext.Session.GetString("username");
            if (String.IsNullOrEmpty(username))
                return RedirectToAction("Login", "Home");     
            ViewData["active_areas"] = getActiveAreas(DB, username);
            ViewData["available_areas"] = getAvailableAreas(DB, username);
            ViewData["unavailable_areas"] = getUnavailableAreas(DB, username);
            return View();
        }
    }
}