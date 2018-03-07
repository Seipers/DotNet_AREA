using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Area.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Area.DAT;
using Microsoft.AspNetCore.Http;

namespace Area.Controllers
{
    public class FacebookController : Controller
    {
        public ActionResult FacebookConnection()
        {
            // Add param to URI in this disctionnary
            var redirectParams = new Dictionary<string, string>()
                {
                    { "client_id", "130733687642786" },
                    { "redirect_uri", "http://localhost:5000/Facebook/FacebookAuthorized" },
                    { "response_type", "code" },
                    { "scope", "public_profile,publish_actions,user_posts" }
                };

            var authorizeUri = redirectParams.Aggregate("https://www.facebook.com/v2.11/dialog/oauth?", (current, redirectParam) => current + (redirectParam.Key + "=" + redirectParam.Value + "&"));
            return Redirect(authorizeUri);
        }

        public ActionResult FacebookAuthorized(string state, string code, [FromServices] AreaDbContext DB)
        {
            string username = HttpContext.Session.GetString("username");
            if (String.IsNullOrEmpty(username))
                return Redirect("http://localhost:5000");
            var token_uri = "https://graph.facebook.com/v2.11/oauth/access_token";
            var client = new WebClient {UseDefaultCredentials = true};
            byte[] response = client.UploadValues(token_uri, new NameValueCollection()
            {
                { "client_id", "130733687642786" },
                { "redirect_uri", "http://localhost:5000/Facebook/FacebookAuthorized" },
                { "client_secret", "f5223fb63a69dfd04c6186ee184a1657"},
                { "code", code}
            });

            // Access_token stored in the following dictionnary
            var jsonResult = JsonConvert.DeserializeObject<Dictionary<string, string>>(System.Text.Encoding.UTF8.GetString(response));

            if (jsonResult["access_token"] != null)
            {
                DB.tokens.Add(new Models.Token {type="Facebook", username=username, value=jsonResult["access_token"]});
                DB.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
                return RedirectToAction("Index", "Home");
        }
    }
}