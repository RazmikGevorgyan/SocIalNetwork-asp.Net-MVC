using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Soc.Models
{
    public class ConfimEmailController : Controller
    {
        private socialEntities context;

        public ConfimEmailController()
        {
            this.context = new socialEntities();
        }
        // GET: ConfimEmail
        public ActionResult Index()
        {
            return View();
        }
        [AllowAnonymous]
        public ActionResult Confirm(string Token, string Email)
        {
            user us = context.users.Find(int.Parse(Token));
            if (us != null)
            {
                string id = TempData["id"].ToString();
                us.ConfirmedEmail = "true";
                Session["User"+id] = us;
                if (us.ConfirmedEmail != "true")
                {
                    var user = new user { id = us.id, ConfirmedEmail = "true" };
                    using (var db = new socialEntities())
                    {
                        db.users.Attach(user);
                        db.Entry(user).Property(x => x.ConfirmedEmail).IsModified = true;
                        db.SaveChanges();
                    }
                    if (us.inviter_id != null || us.inviter_id.ToString() != "")
                    {
                        user usr = context.users.Find(us.inviter_id);
                        int count = usr.invited_by_me ?? default(int);
                        var inviter = new user { id = usr.id, invited_by_me = ++count };
                        using (var db = new socialEntities())
                        {
                            db.users.Attach(inviter);
                            db.Entry(inviter).Property(x => x.invited_by_me).IsModified = true;
                            db.SaveChanges();
                        }
                        if (count> 3)
                        {
                            notification notif = new notification();
                            notif.state = 1;
                            notif.text_id = 10;
                            notif.sender_id = 25;
                            notif.user_id = usr.id;
                            notif.datetime = DateTime.Now;
                            context.notifications.Add(notif);
                            context.SaveChanges();
                        }
                    }
                }
                HttpCookie userId = new HttpCookie("id");
                userId.Value = id.ToString();
                userId.Expires = DateTime.Now.AddHours(2);
                HttpContext.Response.SetCookie(userId);
                TempData["id"] = id;
                return Redirect("/Profile/Index/");
            }
            else
            {
                return RedirectToAction("index","ConfimEmail");
            }
        }
    }
}