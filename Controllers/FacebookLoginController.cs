using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Facebook;
using Newtonsoft.Json;
using System.Web.Security;
using Owin;
using Soc.Models;

namespace Soc.Controllers
{
    public class FacebookLoginController : Controller
    {
        socialEntities db;
        public FacebookLoginController()
        {
            this.db = new socialEntities();
        }
        // GET: FacebookLogin
        public ActionResult Index()
        {
            return View();
        }
        private Uri RedirectUri
        {
            get
            {
                var uriBuilder = new UriBuilder(Request.Url);
                uriBuilder.Query = null;
                uriBuilder.Fragment = null;
                uriBuilder.Path = Url.Action("facebookCallback");
                return uriBuilder.Uri;
            }
        }

        [AllowAnonymous]

        public ActionResult Facebook()
        {
            var fb = new FacebookClient();

            var loginUrl = fb.GetLoginUrl(new

            {
                client_id = "2324915974199076",

                client_secret = "64f8416b5e2aa304cf40ae38770cc36a",

                redirect_uri = RedirectUri.AbsoluteUri,

                response_type = "code",

                scope = "email"
            });

            return Redirect(loginUrl.AbsoluteUri);
        }

        public ActionResult FacebookCallback(string code)

        {
            var fb = new FacebookClient();

            dynamic result = fb.Post("oauth/access_token", new

            {

                client_id = "2324915974199076",

                client_secret = "64f8416b5e2aa304cf40ae38770cc36a",

                redirect_uri = RedirectUri.AbsoluteUri,

                code = code
            });

            var accessToken = result.access_token;

            Session["AccessToken"] = accessToken;

            fb.AccessToken = accessToken;

            dynamic me = fb.Get("me?fields=link,first_name,currency,last_name,email,gender,locale,timezone,verified,picture,age_range");

            string email = me.email;
            List<user> users = db.users.Where(usr => usr.email == email).ToList();
            if (users.Count == 0)
            {
                user us = new user();
                us.email = me.email;
                us.name = me.first_name;
                us.surname = me.last_name;
                us.from_facebook = 1;
                us.profile_photo = "~/Content/Backgrounds/images.png";
                db.users.Add(us);
                db.SaveChanges();
                Session["User"+us.id] = us;
                TempData["id"] = us.id;
                HttpCookie userId = new HttpCookie("id");
                userId.Value = us.id.ToString();
                userId.Expires = DateTime.Now.AddHours(2);
                HttpContext.Response.SetCookie(userId);
                FormsAuthentication.SetAuthCookie(email, false);
                return RedirectToAction("Index", "Profile");
            }
            else
            {
                FormsAuthentication.SetAuthCookie(email, false);
                Session["User"+users[0].id] = users[0];
                HttpCookie userId = new HttpCookie("id");
                userId.Value = users[0].id.ToString();
                userId.Expires = DateTime.Now.AddHours(2);
                HttpContext.Response.SetCookie(userId);
                TempData["id"] = users[0].id;
                return RedirectToAction("Index", "Profile");
            }
        }
    }
}
