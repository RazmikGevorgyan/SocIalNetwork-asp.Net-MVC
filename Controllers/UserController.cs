using Soc.Models;
using Social.Controllers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Threading;
using System.Threading.Tasks;
using Owin;
using System.Data.Entity;

namespace Soc.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        private socialEntities context;
        public UserController()
        {
            this.context = new socialEntities();
        }
        public ActionResult Index()
        {
            return View();
        }

    [HttpPost]
        public async Task<ActionResult> Index(Log log)
        {
            string login = log.login;
            string password = log.password;
            passwordHash hash = new passwordHash();
            List<user> auth = new List<user>();
            auth =await (from item in context.users where item.login == login  select item).ToListAsync();
            if (auth.Count == 0 || hash.Validate(password, auth[0].password) == false)
            {
                if (auth[0] != null)
                {
                    int? counter = auth[0].try_count+1;
                    var user = new user { id = auth[0].id, try_count =counter};
                    using (var db = new socialEntities())
                    {
                        db.users.Attach(user);
                        db.Entry(user).Property(x => x.try_count).IsModified = true;
                        db.SaveChanges();
                    }
                }
                if(auth[0].block_time < DateTime.Now)
                {
                    if (auth[0] != null)
                    {
                        var user = new user { id = auth[0].id, try_count = 0 };
                        using (var db = new socialEntities())
                        {
                            db.users.Attach(user);
                            db.Entry(user).Property(x => x.try_count).IsModified = true;
                            db.SaveChanges();
                        }
                    }
                }
                if (auth[0].is_blocked == 1)
                {
                    auth[0].block_time = DateTime.Now.AddMinutes(30);
                    TimeSpan? time = auth[0].block_time - DateTime.Now;
                    ViewBag.error = string.Format("Administratin block your account");
                    return View();
                }
                if (auth[0].try_count > 3)
                {
                    auth[0].block_time = DateTime.Now.AddMinutes(30);
                    TimeSpan? time = auth[0].block_time - DateTime.Now;
                    ViewBag.error = string.Format("too many unauthorized attempts your accont has blocked in {0} minutes",time);
                    return View();
                }
                ViewBag.error = "please enter correct login or password";
                return View();
            }
            else if (auth[0].ConfirmedEmail=="false") {
                ViewBag.error ="we send confirmation message to your Email please confirm it";
                return View();
            }
            else
            {
                if (auth[0].stat == 1)
                {
                    int iD = auth[0].id;
                    Session["User" + iD.ToString()] = auth[0];
                    HttpCookie userId = new HttpCookie("id");
                    userId.Value = iD.ToString();
                    userId.Expires = DateTime.Now.AddHours(2);
                    HttpContext.Response.SetCookie(userId);
                    TempData["id"] = iD.ToString();
                    return RedirectToAction("Index","Admin");
                }
                if (auth[0] != null)
                {
                    var user = new user { id = auth[0].id, try_count = 0 };
                    using (var db = new socialEntities())
                    {
                        db.users.Attach(user);
                        db.Entry(user).Property(x => x.try_count).IsModified = true;
                        db.SaveChanges();
                    }
                    int iD = auth[0].id;
                    Session["User"+iD.ToString()] = auth[0];
                    HttpCookie userId = new HttpCookie("id");
                    userId.Value = iD.ToString();
                    userId.Expires = DateTime.Now.AddHours(2);
                    HttpContext.Response.SetCookie(userId);
                    TempData["id"] = iD.ToString();
                    return RedirectToAction("Index", "Profile");
                }
                    return View();
            }
        }
        [HttpPost]
        public void ClearCoockies()
        {
            string[] myCookies = Request.Cookies.AllKeys;
            foreach (string cookie in myCookies)
            {
                Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
            }
        }
        public ActionResult Signup(string Token)
        {
            if (Token != null)
            {
                Session["token"] = Token;
            }
            List<string> dbo = new List<string>();
            foreach (Country ct in context.Countries)
            {
                dbo.Add(ct.Name);
            }
            SelectList list = new SelectList(dbo);
            ViewBag.countries = list;
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Signup(userValidate valid)
        {
           
            List<string> dbo = new List<string>();
            foreach (Country ct in context.Countries)
            {
                dbo.Add(ct.Name);
            }
            SelectList list = new SelectList(dbo);
            ViewBag.countries = list;
            if (ModelState.IsValid)
            {
                foreach (user us in context.users)
                {
                    if (us.login == valid.login)
                    {
                        ViewBag.error = "that nickname already choosen please select other";
                        return View();
                    }
                }
                passwordHash hash = new passwordHash();
                string hashable = hash.CreateMd5(valid.password.ToString());
                valid.password = hashable;
                user usr = new user();
                usr.name = valid.name;
                usr.surname = valid.surname;
                usr.login = valid.login;
                usr.password = valid.password;
                usr.country = valid.country;
                usr.ConfirmedEmail = "false";
                usr.phone_number = valid.phone;
                usr.age = valid.age;
                if (Session["token"] != null)
                {
                    int inviterId =int.Parse(Session["token"].ToString());
                    usr.inviter_id = inviterId;
                    Session["token"] = null;
                }
                usr.email = valid.Email;
                List<user> users =await (from item in context.users where 
                                    item.email == valid.Email && item.from_facebook != 1
                                    select item).ToListAsync();
                if (users.Count > 0)
                {
                    ViewBag.error = "there is already have user with that Email";
                    return View();
                }
                usr.birthdate = valid.birthdate;
                if (valid.birthdate > DateTime.Now)
                {
                    ViewBag.error = "false datetime";
                    return View();
                }
                usr.gender = valid.gender;
                try
                {
                    HttpPostedFileBase file = Request.Files["Image"];
                    if (file != null && file.ContentLength >0  )
                    {
                        if (file.FileName.EndsWith(".png") || file.FileName.EndsWith(".jpg") || file.FileName.EndsWith(".jpeg"))
                        {
                            Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                            var fileName = Path.GetFileName(file.FileName);
                            string part = "~/Content/Photos/" + (unixTimestamp + 1).ToString() + fileName;
                            var path = Path.Combine(Server.MapPath("~/Content/Photos/"), (unixTimestamp + 1).ToString() + fileName);
                            file.SaveAs(path);
                            usr.profile_photo = part;
                        }
                        else
                        {
                            ViewBag.error = "please updlad image with extenion .jpg .png .jpeg";
                            return View();
                        }
                    }
                     else
                     {
                        if (valid.gender == "Male")
                        {
                            usr.profile_photo = "~/Content/Photos/avatarMale.png";
                        }
                        else
                        {
                            usr.profile_photo = "~/Content/Photos/avatarFemale.jpg";
                        }
                    }
                    this.context.users.Add(usr);
                    this.context.SaveChanges();
                   //email confirmation part
                    var senderEmail = new MailAddress("gamblermembler@gmail.com", "Gambler");
                    var receiverEmail = new MailAddress(valid.Email, "Receiver");
                    var password = "077449117077449117raz";
                    MailMessage m = new MailMessage(
                    new MailAddress("gamblermembler@gmail.com", "Web Registration"),
                    new MailAddress(valid.Email));
                    m.Subject = "Email confirmation";
                    m.Body = string.Format("Dear {0} <br/> Thank you for your registration, please click on the below link to complete your registration: <a href =\"{1}\">link</a>"
                    ,valid.name, Url.Action("Confirm","ConfimEmail", new{ Token = usr.id, Email = usr.email }, Request.Url.Scheme));
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
                }
                catch (Exception e)
                {
                    ViewBag.error = e.Message;
                    return View();
                }
                int iD = usr.id;
                Session["User" + iD.ToString()] =usr;
                TempData["id"] = usr.id;
                return RedirectToAction("Index", "ConfimEmail");
            }
            return View();
        }
    }
}