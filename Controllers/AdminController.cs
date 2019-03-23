using Soc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Owin;
using System.IO;
using System.Data.Entity;
using System.Threading.Tasks;

namespace Soc.Controllers
{
    public class AdminController : Controller
    {
        socialEntities db;
        public AdminController()
        {
            this.db =new socialEntities();
        }
        // GET: Admin
        public async Task<ActionResult> Index()
        {
            string id;
            if (TempData["id"]!=null)
            {
                 id= TempData["id"].ToString();
            }else
            {
                id = Request.Params["id"];
            }
            user us = Session["User"+id] as user;
           List <NewsFeed> feed = await (from item in db.NewsFeeds
                                        where item.feedState_id == 8
                                        select item).ToListAsync();
            feed = feed.OrderByDescending(m => m.dateTime).ToList();
            List<NewsFeed> feedOwners = new List<NewsFeed>();
            foreach (NewsFeed fe in feed)
            {
                foreach (NewsFeed fed in db.NewsFeeds)
                {
                    if (fe.on_feed_id == fed.id)
                    {
                        feedOwners.Add(fed);
                    }
                }
            }
            feedOwners = feedOwners.OrderByDescending(m => m.dateTime).ToList();
            ViewBag.owner = feedOwners;
            ViewBag.feed = feed;
            ViewBag.currentuser = us;
            ViewBag.all = db.users;
            ViewBag.messages = db.messenger1.Where(m => m.to_user_id == us.id).ToList();
           
            ViewBag.unreadMessages = db.messenger1.Where(m => m.to_user_id ==us.id && m.status == 1).ToList().Count;
            return View();
        }
        public async void GetFeedback(int id)
        {
            NewsFeed feed = new NewsFeed();
            NewsFeed newsfeed =await db.NewsFeeds.FindAsync(id);
            string currid = Request.Params["id"].ToString();
            user us = Session["User"+currid] as user;
            int val = int.Parse(Request.Params["value"].ToString());
            feed.on_feed_id =id;
            feed.dateTime = DateTime.Now;
            feed.feedState_id = 8;
            feed.user_id = us.id;
            notification_text text = new notification_text();
            if (val == 1)
            {
                feed.contetnfeed = db.notification_text.Find(11).notification_text1.ToString();
            }
            else if (val == 2)
            {
                feed.contetnfeed = db.notification_text.Find(12).notification_text1.ToString();
            }
            else if (val == 3)
            {
                feed.contetnfeed = db.notification_text.Find(13).notification_text1.ToString();
            }
            else if (val == 4)
            {
                feed.contetnfeed = db.notification_text.Find(14).notification_text1.ToString();
            }
            else if (val == 5)
            {
                feed.contetnfeed = db.notification_text.Find(15).notification_text1.ToString();
            }
            db.NewsFeeds.Add(feed);
            db.SaveChanges();
        }
        [HttpPost]
        public async Task<JsonResult> UploadFile()
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user; 
            var result = new LinkedList<object>();
            try
            {
                if (Request.Files.Count != 0)
                {
                    foreach (string fil in Request.Files)
                    {
                        follower folower = new follower();
                        NewsFeed feed = new NewsFeed();
                        HttpPostedFileBase file = Request.Files[fil] as HttpPostedFileBase;
                        if (file.ContentLength == 0)
                        {
                            continue;
                        }
                        Int32 unixTimestamp = (Int32)(DateTime.UtcNow.Subtract(new DateTime(1970, 1, 1))).TotalSeconds;
                        var fileName = Path.GetFileName(file.FileName);
                        var ext = Path.GetExtension(file.FileName);
                        fileName = unixTimestamp.ToString() + fileName.ToString();
                        string part = "~/Content/advertPhoto/" + (unixTimestamp + 1).ToString() + fileName;
                        var path = Path.Combine(Server.MapPath("~/Content/advertPhoto/"), (unixTimestamp + 1).ToString() + fileName);
                        file.SaveAs(path);
                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg" || ext == ".gif")
                        {
                            result.AddLast(new
                            {
                                photo = part,
                                video = "",
                            });
                        }
                        else { ViewBag.inputError = "wrong extention"; }
                    }
                }
            }
            catch (Exception e)
            {

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public async void AddAdv(string id)
        {
            int advId = int.Parse(id);
            string src = Request.Params["src"].ToString();
            string cost = Request.Params["cost"].ToString();
            string url = Request.Params["url"].ToString();
            url = url.Replace("/,/Admin/AddAdv/1", "");
            List<advert> ads =await (from item in db.adverts where item.advNo == advId && item.status == 1 select item).ToListAsync();
            if (ads.Count != 0)
            {
                var adv = new advert { id = ads[0].id, advert1 = src,cost=int.Parse(cost),datetime=DateTime.Now,url=url};
                using (var db = new socialEntities())
                {
                    db.adverts.Attach(adv);
                    db.Entry(adv).Property(x => x.advert1).IsModified = true;
                    db.Entry(adv).Property(x => x.cost).IsModified = true;
                    db.Entry(adv).Property(x => x.url).IsModified = true;
                    db.Entry(adv).Property(x => x.datetime).IsModified = true;
                    db.SaveChanges();
                }
            }
            else
            {
                advert ad = new advert();
                ad.advNo = advId;
                ad.status = 1;
                ad.advert1 = src;
                ad.url = url;
                ad.datetime = DateTime.Now;
                ad.cost =int.Parse(cost);
                db.adverts.Add(ad);
                db.SaveChanges();
            }
        }
      
        public async Task<ActionResult> Messenger()
        {
            string currid = Request.Params["id"].ToString();
            user usr = Session["User"+currid] as user;
            user admins =db.users.Where(m => m.stat == 1).ToList().First();
            ViewBag.messages =await db.messenger1.Where(m => m.to_user_id == admins.id || m.from_user_id == admins.id).ToListAsync();
            List<user> notMe=new List<user>();
            List<int?> usersId =await db.messenger1.Where(mes => mes.to_user_id == usr.id).Select(m => m.from_user_id).ToListAsync();
            foreach (int i in usersId.Distinct())
            {
                notMe.Add(db.users.Find(i));
            }
            ViewBag.notme = notMe;
            return View();
        }
        public async void Block(string id)
        {
            int iD = int.Parse(id);
            user usr=await db.users.FindAsync(iD);
            var user = new user { id = iD, is_blocked = 1 };
            using (var db = new socialEntities())
            {
                db.users.Attach(user);
                db.Entry(user).Property(x => x.is_blocked).IsModified = true;
                db.SaveChanges();
            }
        }
        public async void UnBlock(string id)
        {
            int iD = int.Parse(id);
            user usr =await db.users.FindAsync(iD);
            var user = new user { id = iD, is_blocked = 0 };
            using (var db = new socialEntities())
            {
                db.users.Attach(user);
                db.Entry(user).Property(x => x.is_blocked).IsModified = true;
                db.SaveChanges();
            }
        }
        public async Task<JsonResult> SearcheResult()
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            string cont;
            cont = Convert.ToString(Request.Params["text"]);
            List<user> mesUsers =await db.messenger1.Where(m => m.to_user_id == us.id).Select(m=>m.user).Distinct().ToListAsync();
            List<user> users =mesUsers.Where(user => user.name.StartsWith(cont)).Distinct().ToList();
            var result = new LinkedList<object>();
            foreach (var user in users)
            {
                result.AddLast(new
                {
                    id = user.id,
                    name = user.name,
                    surname = user.surname,
                    photo = user.profile_photo,
                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}