using Soc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Owin;
using System.Data.Entity;

namespace Soc.Controllers
{
    public class ChronoController : Controller
    {
        string userID;
        socialEntities db;
        public ChronoController()
        {
            this.db = new socialEntities();
        }
        public ActionResult Index(int id)
        {
            string currId;
            if (TempData["id"] != null)
            {
                currId = TempData["id"].ToString(); 

            }
            else 
            {
                currId= Request.Params["id"].ToString();
            }

            List<NewsFeed> MyAll = db.NewsFeeds.Where(m => m.user_id == id).ToList();
            user toUser = db.users.Find(id);
            List<friend> friends = db.friends.Where(fr => fr.friend_user_id == toUser.id || fr.user_id == toUser.id).ToList();
            ViewBag.friends = friends;
            user user = Session["User"+currId] as user;
            ViewBag.currentuser = db.users.Find(user.id);
            List<friend> myfriend= db.friends.Where(m => m.friend_user_id == user.id || m.user_id == user.id).ToList();
            ViewBag.myFriends = myfriend;
            ViewBag.user = toUser;
            ViewBag.photos = MyAll;
            List<NewsFeed> feed = db.NewsFeeds.Where(fe => fe.user_id ==id && fe.feedState_id!=8).ToList();
            feed = feed.OrderByDescending(m => m.dateTime).ToList();
            ViewBag.feed = feed;
            List<like> likes = db.likes.Where(l => l.liker_id == user.id).ToList();
            ViewBag.likes = likes;
            List<follower> folowers = db.followers.Where(f => f.follower_id == user.id && f.user_id==id).ToList();
            ViewBag.followerOf = folowers;
            List<notification> notif = (from item in db.notifications where item.user_id == user.id select item).ToList();
            notif=notif.OrderByDescending(n => n.datetime).ToList();
            ViewBag.notifications = notif;
            List<NewsFeed> allfeed = db.NewsFeeds.Where(m=>m.feedState_id!=8).ToList();
            ViewBag.allfeed = allfeed;
            List<user> onChat = new List<user>(); 
            foreach(messenger1 mes in db.messenger1)
            {
                if(mes.to_user_id==int.Parse(currId))
                {
                    onChat.Add(mes.user);
                }
                else if(mes.from_user_id==int.Parse(currId))
                {
                    onChat.Add(mes.user1);
                }
            }
            onChat = onChat.Distinct().ToList();
            ViewBag.onchat = onChat;
            int unread = 0;
            foreach (notification not in db.notifications)
            {
                if (not.user_id == user.id && not.state == 1)
                {
                    unread++;
                }
            }
            ViewBag.unreadNotif = unread;
            return View();
        }
        public JsonResult GetFriends(int id)
        {
            string currId = Request.Params["id"];
            user us = Session["User"+currId] as user;
            List<friend> friends = db.friends.Where(m => m.friend_user_id==id || m.user_id==id).ToList();
            var result = new LinkedList<object>();
            foreach (var friend in friends)
            {
                if (friend.user.id == id)
                {
                    result.AddLast(new
                    {
                        id1=friend.user1.id,
                        name = friend.user1.name,
                        surname = friend.user1.surname,
                        photo = friend.user1.profile_photo,
                    });
                }
                else {
                    result.AddLast(new
                    {
                        id1 = friend.user.id,
                        name = friend.user.name,
                        surname = friend.user.surname,
                        photo = friend.user.profile_photo,
                    });
                }
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetAbout(int id)
        {
            user us = db.users.Find(id);
            var result = new object();
                    result=(new
                    {
                        name = us.name,
                        surname = us.surname,
                        age=us.age,
                        email=us.email,
                        gender=us.gender,
                        phone=us.phone_number,
                        country= us.country,
                    });
                return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPhoto(int id)
        {
            List<NewsFeed> photos = db.NewsFeeds.Where(m => m.user_id == id && m.photos!=null && m.feedState_id==1).ToList();
            var result = new LinkedList<object>();
            foreach (var photo in photos)
            {
                result.AddLast(new
                {
                    name=photo.user.name,
                    surname=photo.user.surname,
                    id1=photo.id,
                    photo = photo.photos,
                    profile_photo=photo.user.profile_photo,
                });
           }
                return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetOnePhoto(int id)
        {
            NewsFeed feed = db.NewsFeeds.Find(id);
            var result = new object();
            result = new
            {
                name = feed.user.name,
                surname = feed.user.surname,
                description = feed.contetnfeed,
                likes=feed.likes.Count,
                photo = feed.photos,
                video = feed.videos,
                profile_photo = feed.user.profile_photo,
            };
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPrevItem(int id)
        {
            var result = new object();
            NewsFeed feed = db.NewsFeeds.Find(id);
            NewsFeed newFedd = new NewsFeed();
            List<NewsFeed> feeds = db.NewsFeeds.Where(m => m.user_id == feed.user_id && (m.photos!=null || m.videos!=null)).ToList();
            if (feeds.Count > 1)
            {
                if (feed.id == feeds.First().id)
                {
                   newFedd=feeds.Last();
                    result = new
                    {
                        description = newFedd.contetnfeed,
                        likes = newFedd.likes.Count,
                        feedId = newFedd.id,
                        photo = newFedd.photos,
                        video = newFedd.videos,
                    };
                }
                else
                {
                   int index=feeds.IndexOf(feed);
                   newFedd=feeds[index - 1];
                   result = new
                   {
                       description = newFedd.contetnfeed,
                       likes = newFedd.likes.Count,
                       feedId = newFedd.id,
                       photo = newFedd.photos,
                       video = newFedd.videos,
                   };
                }
            }
            else
            {
                result = new
                {
                    id = id,
                };
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetNextItem(int id)
        {
            var result = new object();
            NewsFeed feed = db.NewsFeeds.Find(id);
            NewsFeed newFedd = new NewsFeed();
            List<NewsFeed> feeds = db.NewsFeeds.Where(m => m.user_id == feed.user_id && (m.photos != null || m.videos != null)).ToList();
            if (feeds.Count > 1)
            {
                if (feed.id == feeds.Last().id)
                {
                    newFedd = feeds.First();
                    result = new
                    {
                        description = newFedd.contetnfeed,
                        likes = newFedd.likes.Count,
                        feedId = newFedd.id,
                        photo = newFedd.photos,
                        video = newFedd.videos,
                    };
                }
                else
                {
                    int index = feeds.IndexOf(feed);
                    newFedd = feeds[index + 1];
                    result = new
                    {
                        description = newFedd.contetnfeed,
                        likes = newFedd.likes.Count,
                        feedId = newFedd.id,
                        photo = newFedd.photos,
                        video = newFedd.videos,
                    };
                }
            }
            else {
                result = new
                {
                    id = id,
                };
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public async Task<ActionResult> SendMessage()
        {
            string currid = Request.Params["id"].ToString();
            if (Request.Params["admin-message"] != null)
            {
                string text = Request.Params["admin-message"].ToString();
                List<user> admmins =await db.users.Where(m => m.stat == 1).ToListAsync();
                user us = Session["User" + currid] as user;
                foreach (user usr in admmins)
                {
                    messenger1 mes = new messenger1();
                    mes.status = 1;
                    mes.message = text;
                    mes.datetime = DateTime.Now;
                    mes.from_user_id = us.id;
                    mes.to_user_id = usr.id;
                    db.messenger1.Add(mes);
                }
                db.SaveChanges();
            }
            return Redirect("/Profile/Index/" + currid);
        }
        public void ChangeCover(int id)
        {
            string currId = Request.Params["id"];
            user us = Session["User"+currId] as user;
            NewsFeed feed = db.NewsFeeds.Find(id);
            var user = new user {id=us.id,back_photo=feed.photos};
            using (var db = new socialEntities())
            {
                db.users.Attach(user);
                db.Entry(user).Property(x => x.back_photo).IsModified = true;
                try
                {
                    NewsFeed feeds = new NewsFeed();
                    feeds.dateTime = DateTime.Now;
                    feeds.user_id = us.id;
                    feeds.photos = feed.photos;
                    feeds.feedState_id = 5;
                    db.NewsFeeds.Add(feeds);
                    db.SaveChanges();
                }
                catch(Exception e)
                {
                    string st = e.Message;
                }
            }
        }
        public void ChangeProfile(int id)
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            NewsFeed feed = db.NewsFeeds.Find(id);
            var user = new user { id = us.id, profile_photo = feed.photos };
            using (var db = new socialEntities())
            {
                db.users.Attach(user);
                db.Entry(user).Property(x => x.profile_photo).IsModified = true;
                try
                {
                    //add to feed
                    NewsFeed feeds = new NewsFeed();
                    feeds.dateTime = DateTime.Now;
                    feeds.user_id = us.id;
                    feeds.photos = feed.photos;
                    feeds.feedState_id = 6;
                    db.NewsFeeds.Add(feeds);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    string st = e.Message;
                }
            }
        }
        public void SharePost(int id)
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            NewsFeed feed = db.NewsFeeds.Find(id);
            NewsFeed myfeed = new NewsFeed();
            myfeed.dateTime = DateTime.Now;
            myfeed.user_id = us.id;
            myfeed.feedState_id = 3;
            myfeed.on_feed_id = feed.id;
            myfeed.contetnfeed = feed.contetnfeed;
            db.NewsFeeds.Add(myfeed);
            if (feed.user_id != us.id)
            {
                notification not = new notification();
                not.sender_id = us.id;
                not.user_id = feed.user_id;
                not.datetime = DateTime.Now;
                not.text_id = 7;
                not.state = 1;
                not.on_feed_id = feed.id;
                db.notifications.Add(not);
            }
            db.SaveChanges();
        }
        public void SharePhoto(int id)
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User"+currid] as user;
            NewsFeed feed = db.NewsFeeds.Find(id);
            NewsFeed myfeed = new NewsFeed();
            myfeed.dateTime = DateTime.Now;
            myfeed.user_id = us.id;
            myfeed.feedState_id = 3;
            if (feed.on_feed_id == null)
            {
                myfeed.on_feed_id = feed.id;
            }
            else
            {
                myfeed.on_feed_id = feed.on_feed_id;
            }
            myfeed.photos = feed.photos;
            myfeed.contetnfeed = feed.contetnfeed;
            db.NewsFeeds.Add(myfeed);
            if (feed.user_id != us.id)
            {
                notification not = new notification();
                not.sender_id = us.id;
                not.datetime = DateTime.Now;
                not.user_id = feed.user_id;
                not.text_id = 7;
                not.state = 1;
                not.on_feed_id = feed.id;
                db.notifications.Add(not);
            }
            db.SaveChanges();
        }
        public void ShareUrl(int id)
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            NewsFeed feed = db.NewsFeeds.Find(id);
            NewsFeed myfeed = new NewsFeed();
            myfeed.dateTime = DateTime.Now;
            myfeed.user_id = us.id;
            myfeed.feedState_id = 1;
            if (feed.on_feed_id == null)
            {
                myfeed.on_feed_id = feed.id;
            }
            else
            {
                myfeed.on_feed_id = feed.on_feed_id;
            }
            myfeed.url = feed.url;
            db.NewsFeeds.Add(myfeed);
            if (feed.user_id != us.id)
            {
                notification not = new notification();
                not.sender_id = us.id;
                not.datetime = DateTime.Now;
                not.user_id = feed.user_id;
                not.text_id = 7;
                not.state = 1;
                not.on_feed_id = feed.id;
                db.notifications.Add(not);
            }
            db.SaveChanges();
        }
        public void ShareViedo(int id)
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            NewsFeed feed = db.NewsFeeds.Find(id);
            NewsFeed myfeed = new NewsFeed();
            myfeed.dateTime = DateTime.Now;
            myfeed.user_id = us.id;
            myfeed.feedState_id = 4;
            if (feed.on_feed_id == null)
            {
                myfeed.on_feed_id = feed.id;
            }
            else
            {
                myfeed.on_feed_id = feed.on_feed_id;
            }
            myfeed.videos = feed.videos;
            myfeed.contetnfeed = feed.contetnfeed;
            db.NewsFeeds.Add(myfeed);
            if (feed.user_id != us.id)
            { 
                notification not = new notification();
                not.sender_id = us.id;
                not.datetime = DateTime.Now;
                not.user_id = feed.user_id;
                not.text_id = 7;
                not.state = 1;
                not.on_feed_id = feed.id;
                db.notifications.Add(not);
            }
            db.SaveChanges();
        }
        public void DeleteFeed(int id)
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            NewsFeed feed = db.NewsFeeds.Find(id);
            if (us.stat == 1)
            {
                int val = int.Parse(Request.Params["value"].ToString());
                notification not = new notification();
                not.sender_id = us.id;
                not.user_id = feed.user_id;
                if (val == 1)
                {
                    not.text_id = 11;
                }
                else if (val == 2)
                {
                    not.text_id = 12;
                }
                else if (val == 3)
                {
                    not.text_id = 13;
                }
                else if (val == 4)
                {
                    not.text_id = 14;
                }
                else if (val == 5)
                {
                    not.text_id = 15;
                }
                not.datetime = DateTime.Now;
                not.state = 1;
                db.notifications.Add(not);
                db.SaveChanges();
            }
            List<NewsFeed> allfeeds = db.NewsFeeds.Where(m => m.on_feed_id == id).ToList();
            db.NewsFeeds.RemoveRange(allfeeds);
            db.NewsFeeds.Remove(feed);
            db.SaveChanges();
        }
        public void deletPhoto() {
            string path = Request.Params["path"];
            try
            {
                var filePath = Server.MapPath(path);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                db.SaveChanges();
            }
            catch (Exception e) { }
        }
        public void deleteFeedWithPhoto(int id)
        {
            NewsFeed feed = db.NewsFeeds.Find(id);
            if (feed != null)
            {
                db.NewsFeeds.Remove(feed);
            }
            string path = Request.Params["src"].ToString();
            try
            {
                var filePath = Server.MapPath(path);
                if (System.IO.File.Exists(filePath))
                {
                    System.IO.File.Delete(filePath);
                }
                db.SaveChanges();
            }
            catch(Exception e) { }
        }
       public void ShareNewPost()
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            string descr = Request.Params["text"].ToString();
            string part = Request.Params["src"].ToString();
            NewsFeed feed = new NewsFeed();
            feed.user_id = us.id;
            feed.feedState_id = 1;
            feed.dateTime = DateTime.Now;
            if (descr != "" && descr != null)
            {
                feed.contetnfeed = descr;
            }
            if (part.EndsWith(".png") || part.EndsWith(".jpg") || part.EndsWith(".jpeg"))
            {
                feed.photos = part;
            }
            else if (part.EndsWith(".ogg") || part.EndsWith(".mp4") || part.EndsWith(".webm"))
            {
                feed.videos = part;
            }
            db.NewsFeeds.Add(feed);
            db.SaveChanges();
        }
        public void setUrl()
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            string path = Request.Params["url"].ToString();
            string description = Request.Params["text"].ToString();
            path = path.Replace(",/Chrono/setUrl/", "");
            NewsFeed feed = new NewsFeed();
            feed.user_id = us.id;
            feed.feedState_id = 7;
            feed.url = path;
            if(description!="" && description != null)
            {
                feed.contetnfeed = description;
            }
            feed.dateTime = DateTime.Now;
            db.NewsFeeds.Add(feed);
            db.SaveChanges();
        }
        public ActionResult ChangeColor(int id)
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            string color=Request.Params["color"].ToString();
            var user = new user { id = us.id, color = color };
            using (var db = new socialEntities())
            {
                db.users.Attach(user);
                db.Entry(user).Property(x => x.color).IsModified = true;
                db.SaveChanges();
            }
            return Redirect("/Profile/Index/");
        }
    }
}