using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Specialized;
using System.Net;
using System.Threading;
using Newtonsoft.Json;
using System.Collections.Generic;
using Area.DAT;
using Area.Models;
using Microsoft.AspNetCore.Http;

namespace Area.Controllers
{
    public class WoWController : Controller
    {
        public ActionResult WoWConnection()
        {
            var key = "ak8a3kw6ksu5y89t8bx5ct8f4kp5uzv5";
            var redurl = "https://8aec6d60.ngrok.io/WoW/WoWAuthorized";//"http://localhost:5000/WoW/WoWAuthorized";
            var authorize_uri = "https://eu.battle.net/oauth/authorize";
            var scope = "wow.profile";
            var state = HttpContext.Session.GetString("username");
            var response_type = "code";

            var red = authorize_uri + "?client_id=" + key + "&scope=" + scope + "&state=" + state + "&redirect_uri=" + redurl + "&response_type=" + response_type;
            return Redirect(red);
        }

        public ActionResult WoWAuthorized(string state, string code, [FromServices] AreaDbContext DB)
        {
            string username = HttpContext.Session.GetString("username");
            if (String.IsNullOrEmpty(username))
                username = state;
            if (String.IsNullOrEmpty(username))
                return Redirect("http://localhost:5000");
            var key = "ak8a3kw6ksu5y89t8bx5ct8f4kp5uzv5";
            var secret = "eTw9eyBDPMwE7PKKd2QX5pXBNx4VBZkt";
            var redirect_uri = "https://8aec6d60.ngrok.io/WoW/WoWAuthorized";//"http://localhost:5000/WoW/WoWAuthorized";
            var token_uri = "https://eu.battle.net/oauth/token";
            var scope = "wow.profile";;
            var client = new WebClient();
            client.UseDefaultCredentials = true;
            try {
                byte[] response = client.UploadValues(token_uri, new NameValueCollection()
                {
                    {"client_id", key},
                    {"client_secret", secret},
                    {"redirect_uri", redirect_uri},
                    {"scope", scope},
                    {"grant_type", "authorization_code"},
                    {"code", code}
                });
                string result = System.Text.Encoding.UTF8.GetString(response);
                dynamic obj = JsonConvert.DeserializeObject(result);
                DB.tokens.Add( new Models.Token {type="WoW", username=username, value=obj["access_token"]});
                DB.SaveChanges();
                WoWModuleModel WoW = new WoWModuleModel(obj["access_token"].ToString(), 0);
                foreach (var character in WoW.getCharacters())
                {
                    if (String.IsNullOrEmpty(character.guild))
                        character.guild = "None";
                    if (character.name != "Jtefearlol")
                    {
                        WoWCharacterModel ch = WoW.getAllInfos(character.realm, character.name);
                        DB.wowcharactermodels.Add(new Models.WoWCharacterModel {username = username, name=character.name, realm=character.realm,
                        level=character.level, guild=character.guild, deaths=ch.deaths, facepalmed=ch.facepalmed,
                        questsDone=ch.questsDone, fishCaught=ch.fishCaught});
                        DB.SaveChanges();
                    }
                }
            }
            catch
            {
                Console.WriteLine("Error in WoWAuthorize");
                return Redirect("http://localhost:5000");
            }
            return Redirect("http://localhost:5000");
        }
        
        public ActionResult WoWProfileInfo([FromServices] AreaDbContext DB)
        {
            WoWModuleModel WoW = new WoWModuleModel("vgf6ne9wt7buaewe48wby99u", 0);
            ViewData["Characters"] = WoW.getCharacters();
            return View();
        }

         public ActionResult WoWTestView(string realm, string name, [FromServices] AreaDbContext DB)
        {
            WoWModuleModel WoW = new WoWModuleModel("vgf6ne9wt7buaewe48wby99u", 0);
            WoWCharacterModel character = WoW.getAllInfos(realm, name);
            ViewData["Level"] = character.level;
            ViewData["Deaths"] = character.deaths;
            ViewData["Quests"] = character.questsDone;
            ViewData["Facepalmed"] = character.facepalmed;
            ViewData["Fish"] = character.fishCaught;
            return View();
        }
    }
}