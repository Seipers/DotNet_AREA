using System;
using System.Threading;
using Microsoft.AspNetCore.Mvc;
using SpotifyAPI.Web.Auth;
using SpotifyAPI.Web.Enums;
using SpotifyAPI.Web.Models;
using SpotifyAPI.Web;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Area.DAT;
using Area.Models;

namespace Area.Controllers
{
    public class SpotifyController : Controller
    {
        public ActionResult Auth()
        {
            return Redirect(GetAuthUri());
        }

        public string GetAuthUri()
        {
            string clientId = "88755bb9978e47b29bcb13e084db4015";
            string redirectUri = "http://localhost:5000/Spotify/Connected";
            Scope _scope = Scope.Streaming | Scope.UserReadEmail | Scope.PlaylistModifyPrivate | Scope.PlaylistModifyPublic | Scope.PlaylistReadCollaborative |
                Scope.PlaylistReadPrivate | Scope.UserFollowModify | Scope.UserFollowRead | Scope.UserLibraryModify | Scope.UserLibraryRead | Scope.UserModifyPlaybackState |
                Scope.UserReadBirthdate | Scope.UserReadPlaybackState | Scope.UserReadPrivate | Scope.UserReadRecentlyPlayed | Scope.UserTopRead;

            return "https://accounts.spotify.com/en/authorize?client_id=" + clientId +
                "&response_type=token&redirect_uri=" + redirectUri +
                "&state=&scope=" + _scope.GetStringAttribute(" ") +
                "&show_dialog=true";
        }

        public ActionResult AuthResponse()
        {
            ViewBag.AuthUri = GetAuthUri();
            Console.WriteLine(ViewBag.AuthUri);
            return View();  
        }

        public ActionResult Connected(string access_token, string token_type, string expires_in, string state, [FromServices] AreaDbContext DB)
        {
            ViewData["Token"] = access_token;
            if (access_token != null)
            {
                DB.tokens.Add(new Models.Token {type="Spotify", username=HttpContext.Session.GetString("username"), value=access_token});
                DB.SaveChanges();
                SpotifyModule spo = new SpotifyModule(access_token, 0);
                spo.getCurrentMusic();
                return RedirectToAction("Index", "Home");
            }
            return View();
        }
    }
}