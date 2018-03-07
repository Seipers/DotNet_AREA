using System;
using Area.DAT;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TweetSharp;

namespace Area.Controllers
{
    public class TwitterController : Controller
    {
        public ActionResult TwitterAuth()
        {
            string Key = "CI6yhnDqdfVvm8TVckYlIfadl";
            string Secret = "MvCtuuS9nPLHPLx7yRsrM8x7DvArtW46pPgfn4okXwfL5UThL5";

            TwitterService service = new TwitterService(Key, Secret);

            //Obtaining a request token
            OAuthRequestToken requestToken = service.GetRequestToken
                                             ("http://localhost:5000/Twitter/TwitterCallback");

            Uri uri = service.GetAuthenticationUrl(requestToken);

            //Redirecting the user to Twitter Page
            return Redirect(uri.ToString());
        }

        public ActionResult TwitterCallback(string oauth_token, string oauth_verifier, [FromServices] AreaDbContext DB)
{
            var requestToken = new OAuthRequestToken { Token = oauth_token };

            string Key = "CI6yhnDqdfVvm8TVckYlIfadl";
            string Secret = "MvCtuuS9nPLHPLx7yRsrM8x7DvArtW46pPgfn4okXwfL5UThL5";

            try
            {
                TwitterService service = new TwitterService(Key, Secret);

                //Get Access Tokens
                OAuthAccessToken accessToken = 
                           service.GetAccessToken(requestToken, oauth_verifier);

                service.AuthenticateWith(accessToken.Token, accessToken.TokenSecret);

                VerifyCredentialsOptions option = new VerifyCredentialsOptions();

                //According to Access Tokens get user profile details
                TwitterUser user = service.VerifyCredentials(option);
                string username = HttpContext.Session.GetString("username");
                DB.tokens.Add( new Models.Token {type="TwitterAccessToken", username=username, value=accessToken.Token});
                DB.tokens.Add( new Models.Token {type="TwitterAccessTokenSecret", username=username, value=accessToken.TokenSecret});
                DB.SaveChanges();
                return RedirectToAction("Index", "Home");
            }
            catch
            {
                return RedirectToAction("Index", "Home");
            }            
        }
    }
}