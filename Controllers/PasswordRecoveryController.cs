using Soc.Models;
using Social.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using Owin;


namespace Soc.Controllers
{
    public class PasswordRecoveryController : Controller
    {
        private socialEntities context;
        public PasswordRecoveryController()
        {
            this.context = new socialEntities();
        }
        // GET: PasswordRecovery
        public ActionResult Index()
        {
            if (TempData["error"] != null)
            {
                ViewBag.error = TempData["error"].ToString();
            }
            if (TempData["success"] != null)
            {
                ViewBag.success = TempData["success"].ToString();
            }
            return View();
        }
       
    [HttpPost]
        public ActionResult CheckEmail()
        {
            try
            {
                string email = Request.Params["recovery-email"].ToString();
                List<user> users = (from item in context.users where item.email == email select item).ToList();
                if (users[0].from_facebook != 1)
                {
                    if (users.Count != 0)
                    {
                        var senderEmail = new MailAddress("gamblermembler@gmail.com", "Gambler");
                        var receiverEmail = new MailAddress(email, "Receiver");
                        var password = "077449117077449117raz";
                        MailMessage m = new MailMessage(
                        new MailAddress("gamblermembler@gmail.com", "Web Registration"),
                        new MailAddress(email));
                        m.Subject = "Password Recovery";
                        m.Body = string.Format("Dear {0} <br/>  please click on the below link to complete password changing: <a href =\"{1}\">link</a>"
                        , users[0].name, Url.Action("EmailConfirmed", "PasswordRecovery", new { Token = users[0].id }, Request.Url.Scheme));
                        m.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient
                        {
                            Host = "smtp.gmail.com",
                            Port = 587,
                            EnableSsl = true,
                            DeliveryMethod = SmtpDeliveryMethod.Network,
                            UseDefaultCredentials = false,
                            Credentials = new NetworkCredential(senderEmail.Address, password)
                        };
                        {
                            smtp.Send(m);
                        }
                        TempData["success"] = "We send Confirmation llink to your Email";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        TempData["error"] = "there in no user with that Email";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["error"] = "you can't recover your profile you migrate your data from facebook";
                    return RedirectToAction("Index");
                }
            }
          
            catch (Exception e)
            {
                ViewBag.error = e.Message;
                return RedirectToAction("Index");
            }
        }
        [HttpPost]
        public ActionResult EmailConfirmed(ChangePassword chng)
        {
            string password = chng.password;
            if (TempData["id"] != null)
            {
                int id = int.Parse(TempData["id"].ToString());
                user us = context.users.Find(id);
                passwordHash hash = new passwordHash();
                string newpass =hash.CreateMd5(chng.password);
                var user = new user { id = id, password = newpass };
                using (var db = new socialEntities())
                {
                    db.users.Attach(user);
                    db.Entry(user).Property(x => x.password).IsModified = true;
                    db.SaveChanges();
                }
                TempData["id"] = us.id;
                HttpCookie userId = new HttpCookie("id");
                userId.Value = us.id.ToString();
                userId.Expires = DateTime.Now.AddHours(2);
                HttpContext.Response.SetCookie(userId);
                Session["User"+us.id] = us;
                return Redirect("/Profile/Index/");
            }
            return View();
        }
 
        public ActionResult EmailConfirmed(string Token)
        {
            int id = int.Parse(Token);
            TempData["id"] = Token;
            return View();
        }
    }
  }
