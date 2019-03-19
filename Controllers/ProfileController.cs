using Soc.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Script.Serialization;
using Owin;
using Social.Controllers;

namespace Soc.Controllers
{
    public class ProfileController : Controller
    {
        socialEntities db;

        public object UserManager { get; private set; }

        public ProfileController()
        {
            this.db = new socialEntities();
        }
        // GET: Profile
        public ActionResult Index()
        {
            string iD;
            ViewBag.users = db.users;
            if (TempData["id"] != null)
            {
                iD = TempData["id"].ToString();
            }
            else 
            {
                iD = Request.Params["id"].ToString();
            }
            if (TempData["message"] != null)
            {
                ViewBag.error = TempData["message"];
            }
            user us = Session["User"+iD] as user;
            List<messenger1> mes = db.messenger1.Where(m => m.to_user_id == us.id).ToList();
            ViewBag.messages = mes;
            List<follower> folowers1 = db.followers.Where(f => f.user_id == us.id).ToList();
            ViewBag.folowers = folowers1;
            List<friend> friends = db.friends.Where(m => m.user_id == us.id || m.friend_user_id==us.id).ToList();
            ViewBag.friends = friends;
            List<notification> notif = (from item in db.notifications where item.user_id == us.id select item).ToList();
            notif.OrderByDescending(m => m.datetime);
            ViewBag.notifications = notif;
            List<follower> folowers = db.followers.Where(f => f.follower_id == us.id).ToList();
            ViewBag.followerOf = folowers;
            int unread=0;
            List<like> likes = db.likes.Where(l => l.liker_id == us.id).ToList();
            ViewBag.likes = likes;
            foreach (notification not in db.notifications)
            {
                if(not.user_id==us.id && not.state == 1)
                {
                    unread++;
                }
            }
            ViewBag.unreadNotif = unread;
            List < int?> folow = new List<int?>();
            folow = (from item in db.followers where item.follower_id == us.id select item.user_id).ToList();
            folow.Add(us.id);
            List<NewsFeed> feed = this.GetNews(folow);
            feed=feed.OrderByDescending(m => m.dateTime).ToList();
            ViewBag.currentuser = db.users.Find(us.id);
            ViewBag.NewsFeed = feed;
            List<NewsFeed> allfeed = db.NewsFeeds.ToList();
            ViewBag.allfeed = allfeed;
            ViewBag.advert = db.adverts.ToList() as List<advert>;
            List<user> onChat = new List<user>();
            foreach (messenger1 mese in db.messenger1)
            {
                if (mese.to_user_id == int.Parse(iD))
                {
                    onChat.Add(mese.user);
                }
                else if (mese.from_user_id == int.Parse(iD))
                {
                    onChat.Add(mese.user1);
                }
            }
            onChat = onChat.Distinct().ToList();
            ViewBag.onchat = onChat;
            return View();
        }
        public ActionResult FirstIn() {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            return View();
        }
        private List<NewsFeed> GetNews(List<int?> imFolowerOf)
        {
            List<NewsFeed> news = new List<NewsFeed>();
            foreach(int i in imFolowerOf)
            {
                foreach(NewsFeed n in db.NewsFeeds)
                {
                    if(n.user_id == i && n.feedState_id!=8)
                    {
                       news.Add(n);
                    }
                }
            }
           
            return news;
        }
        public ActionResult Messenger()
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            List<messenger1> mes = db.messenger1.Where(m => m.to_user_id == us.id).ToList();
            ViewBag.messages = mes;
            List<follower> folowers1 = db.followers.Where(f => f.user_id == us.id).ToList();
            ViewBag.folowers = folowers1;
            List<friend> friends = db.friends.Where(m => m.user_id == us.id || m.friend_user_id == us.id).ToList();
            ViewBag.friends = friends;
            List<notification> notif = (from item in db.notifications where item.user_id == us.id select item).ToList();
            notif.OrderByDescending(m => m.datetime);
            ViewBag.notifications = notif;
            List<follower> folowers = db.followers.Where(f => f.follower_id == us.id).ToList();
            ViewBag.followerOf = folowers;
            int unread = 0;
            List<like> likes = db.likes.Where(l => l.liker_id == us.id).ToList();
            ViewBag.likes = likes;
            foreach (notification not in db.notifications)
            {
                if (not.user_id == us.id && not.state == 1)
                {
                    unread++;
                }
            }
            ViewBag.unreadNotif = unread;
            List<int?> folow = new List<int?>();
            folow = (from item in db.followers where item.follower_id == us.id select item.user_id).ToList();
            folow.Add(us.id);
            List<NewsFeed> feed = this.GetNews(folow);
            feed = feed.OrderByDescending(m => m.dateTime).ToList();
            ViewBag.currentuser = db.users.Find(us.id);
            ViewBag.NewsFeed = feed;
            List<NewsFeed> allfeed = db.NewsFeeds.ToList();
            ViewBag.allfeed = allfeed;
            ViewBag.advert = db.adverts.ToList() as List<advert>;
            List<user> onChat = new List<user>();
            foreach (messenger1 mese in db.messenger1)
            {
                if (mese.to_user_id == int.Parse(currid))
                {
                    onChat.Add(mese.user);
                }
                else if (mese.from_user_id == int.Parse(currid))
                {
                    onChat.Add(mese.user1);
                }
            }
            onChat = onChat.Distinct().ToList();
            ViewBag.onchat = onChat;
            user admins = db.users.Where(m => m.stat == 1).ToList().First();
            ViewBag.messages = db.messenger1.Where(m => m.to_user_id == us.id || m.from_user_id == us.id).ToList();
            return View();
        }
        public JsonResult SearcheResult()
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            string cont;
            cont = Convert.ToString(Request.Params["text"]);
            List<user> onChat = new List<user>();
            foreach (messenger1 mese in db.messenger1)
            {
                if (mese.to_user_id == int.Parse(currid))
                {
                    onChat.Add(mese.user);
                }
                else if (mese.from_user_id == int.Parse(currid))
                {
                    onChat.Add(mese.user1);
                }
            }
            onChat = onChat.Distinct().ToList();
            List<user> users = onChat.Where(user => user.name.StartsWith(cont)).Distinct().ToList();
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

        [HttpPost]
        public ActionResult Index(int id)
        {
            //add your shares into your newsfeed 
            user us = Session["User"+id.ToString()] as user;
            follower folower1 = new follower();
            NewsFeed feed1 = new NewsFeed();
            if (Request.Params["area1"]!=null && Request.Params["area1"].ToString() != "")
            {
                feed1.user_id = us.id;
                feed1.contetnfeed = Request.Params["area1"].ToString();
                feed1.dateTime = DateTime.Now;
                feed1.feedState_id = 1;
                db.NewsFeeds.Add(feed1);
                db.SaveChanges();
                TempData["id"] = id;
                return RedirectToAction("Index");
            }
            TempData["id"] = id;
            return RedirectToAction("Index");
        }
        [HttpPost]
        public async Task<JsonResult> UploadFile()
        {
            string id = Request.Params["id"].ToString();
            user us = Session["User"+id] as user;
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
                        string part = "~/Content/Photos/" + (unixTimestamp + 1).ToString() + fileName;
                        var path = Path.Combine(Server.MapPath("~/Content/Photos/"), (unixTimestamp + 1).ToString() + fileName);
                        file.SaveAs(path);
                        if (ext == ".png" || ext == ".jpg" || ext == ".jpeg")
                        {
                            result.AddLast(new
                            {
                                photo = part,
                                video = "",
                            });
                        }
                        else if (ext == ".ogg" || ext == ".mp4" || ext == ".webm")
                        {
                            result.AddLast(new
                            {
                                video = part,
                                photo = "",
                            });
                        }
                        else { ViewBag.inputError = "wrong extention"; }
                    }
                }
            }
            catch(Exception e)
            {

            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public void AddFollow()
        {
            string currId = Request.Params["id"].ToString();
            int id = int.Parse(Request.Params["id1"]);
            user us = Session["User"+currId] as user;
            follower folow = new follower();
            folow.follower_id = us.id;
            folow.user_id =id;
            db.followers.Add(folow);
            try
            {
                db.SaveChanges();
                Response.Write("ok");
            }catch(Exception e)
            {
                Response.Write("Sorry something wen't wrong - " + e.InnerException.ToString() + "x=" + id);
            }
        }
        public void UnFollow()
        {
            string currId = Request.Params["id"].ToString();

            int id = int.Parse(Request.Params["id1"]);
            user us = Session["User"+currId] as user;
            follower folow = new follower();
            folow = db.followers.Where(m => m.user_id == id && m.follower_id == us.id).ToList().First();
            
            try
            {
                db.followers.Remove(folow);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Response.Write("Sorry something wen't wrong - " + e.InnerException.ToString() + "x=" + id);
            }
        }
        public void UnFriend()
        {
            string currId = Request.Params["id"].ToString();
            int id = int.Parse(Request.Params["id1"]);
            user us = Session["User"+currId] as user;
            friend friend = new friend();
            friend = db.friends.Where(fr => (fr.friend_user_id == id && fr.user_id == us.id) || (fr.user_id == id && fr.friend_user_id == us.id)).ToList().First();
            try
            {
                db.friends.Remove(friend);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                Response.Write("Sorry something wen't wrong - " + e.InnerException.ToString() + "x=" + id);
            }
            Redirect("/Chrono/index/" + id);
        }
        public void SendRequest()
        {
            string currId = Request.Params["id"].ToString();
            int id = int.Parse(Request.Params["id1"]);
            user us = Session["User"+currId] as user;
                    notification notif = new notification();
                    notif.user_id = id;
                    notif.state =1;
                    notif.text_id =1;
                    notif.sender_id = us.id;
                    notif.datetime = DateTime.Now;
                    db.notifications.Add(notif);
                    try
                    {
                        db.SaveChanges();
                    }
                    catch (Exception e)
                    {
                        Response.Write("Sorry something wen't wrong - " + e.InnerException.ToString() + "x=" + id);
                    }
            }
        public void DenyRequest()
        {
            int notifID = int.Parse(Request.Params["notifID"]);
            notification not=db.notifications.Where(n=>n.id==notifID).ToList().First();
            db.notifications.Remove(not);
            db.SaveChanges();
        }
        public void ConfimRequest()
        {
            string currid = Request.Params["id"].ToString();
            int id = int.Parse(Request.Params["id1"].ToString());
            int notifID = int.Parse(Request.Params["notifID"]);
            user us = Session["User"+currid] as user;
            //delete current notification
            notification not = db.notifications.Where(n=>n.id==notifID).ToList().First();
            db.notifications.Remove(not);
            db.SaveChanges();
            //sent response notification
            notification notif =new notification();
            notif.user_id = id;
            notif.sender_id = us.id;
            notif.text_id = 4;
            notif.state = 1;
            notif.datetime = DateTime.Now;
            db.notifications.Add(notif);
            db.SaveChanges();
            //add to friend;
            friend friends = new friend();
            friends.user_id = us.id;
            friends.friend_user_id = id;
            db.friends.Add(friends);
            //add to followers 
            follower folow = new follower();
            folow.user_id = us.id;
            folow.follower_id = id;
            db.followers.Add(folow);
            follower folow2 = new follower();
            folow2.user_id = id;
            folow2.follower_id = us.id;
            db.followers.Add(folow2);
            try
            {
                db.SaveChanges();
                Response.Write("ok");
            }
            catch (Exception e)
            {
                Response.Write("Sorry something wen't wrong - " + e.InnerException.ToString() + "x=" + id);
            }
        }
        public void ClearNotifications()
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            foreach (notification notif in db.notifications)
            {
                if (notif.user.id == us.id && notif.state==1)
                {
                    notif.state = 0;
                }
            }
            db.SaveChanges();
        }
        public void DenyYourRequest()
        {
            int curId = int.Parse(Request.Params["id"].ToString());
            int id = int.Parse(Request.Params["id1"]);
            user us = Session["User"+curId] as user;
            List<notification> not = (from no in db.notifications where no.sender_id == us.id && no.user_id == id && no.state == 1 && no.text_id == 1 select no).ToList();
            foreach (notification no in not)
            {
                 db.notifications.Remove(no);
                 db.SaveChanges();
                 db.SaveChangesAsync();
            }
        }
        public JsonResult SeeMessages(int id)
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            List<messenger1> messages = db.messenger1.Where(m => ( (m.from_user_id == us.id  && m.to_user_id == id) || (m.from_user_id == id && m.to_user_id == us.id) )).ToList();
            messages.OrderByDescending(x => x.datetime);
            var result = new LinkedList<object>();
            foreach (var message in messages)
            {
                result.AddLast(new
                {
                    id = message.id,
                    from_id = message.from_user_id,
                    to_id = message.to_user_id,
                    state = message.status,
                    datetime = message.datetime.ToString(),
                    content = message.message,
                    from = message.user.profile_photo,
                    to = message.user1.profile_photo,
                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetMessages()
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            List<messenger1> messages = db.messenger1.Where(m =>( m.to_user_id == us.id || m.from_user_id == us.id) && m.status==1).ToList(); 
            messages.OrderByDescending(x => x.datetime);
            var result = new LinkedList<object>();
            foreach (var message in messages)
            {
                result.AddLast(new
                {
                    from_id = message.from_user_id,
                    to_id = message.to_user_id,
                    state = message.status,
                    datetime = message.datetime.ToString(),
                    content = message.message
                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void MakeReaded(){
            string currid = Request.Params["id"].ToString();
            int id = int.Parse(Request.Params["id1"]);
            user us = Session["User"+currid] as user;
            List<messenger1> mes = db.messenger1.Where(m => m.to_user_id == us.id && m.from_user_id == id && m.status == 1).ToList();
            foreach(messenger1 m in mes)
            {
                m.status = 0;
            }
            db.SaveChanges();
        }

        public void SendMessage()
        {
            string currid = Request.Params["id"].ToString();
            int id = int.Parse(Request.Params["id1"]);
            user us = Session["User"+currid] as user;
            string message=Request.Params["message"];
            if (message != null)
            {
            messenger1 newMessage = new messenger1();
            newMessage.from_user_id = us.id;
            newMessage.message = message;
            newMessage.status = 1;
            newMessage.datetime = DateTime.Now;
            newMessage.to_user_id = id;
            db.messenger1.Add(newMessage);
            db.SaveChanges();
            }

        }
        public void AddComment(int id) {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            NewsFeed feed = db.NewsFeeds.Find(id);
            comment com = new comment();
            com.newsfeed_id = id;
            com.datetime = DateTime.Now;
            com.comment1 =Request.Params["content"];
            com.commentator_id = us.id;
            db.comments.Add(com);

            if (feed.user_id != us.id)
            {
                notification notif = new notification();
                notif.datetime = DateTime.Now;
                notif.sender_id = us.id;
                notif.user_id = feed.user_id;
                notif.on_feed_id = id;
                notif.state = 1;
                notif.text_id = 5;
                db.notifications.Add(notif);
            }
            db.SaveChanges();
        }
        public JsonResult GetComments(int id)
        {
            int counter;
            if (Request.Params["counter"] != null)
            {
                counter = int.Parse(Request.Params["counter"]);
            }
            else {
                counter = 5;
            }
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            List<comment> comments = db.comments.Where(m => m.newsfeed_id ==id).OrderByDescending(m=>m.datetime).Take(counter).ToList();
            comments = comments.OrderBy(m => m.datetime).ToList();
            var result = new LinkedList<object>();
            foreach (var message in comments)
            {
                result.AddLast(new
                {
                    comment = message.comment1,
                    commentator = message.commentator_id,
                    datetime =message.datetime.ToString(),
                    name = message.user.name,
                    surname = message.user.surname,
                    photo = message.user.profile_photo,
                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public JsonResult SearchResult()
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            string cont; 
            cont= Convert.ToString(Request.Params["cont"]);
            List<user> users = db.users.Where(user => (user.name.StartsWith(cont) || user.surname.StartsWith(cont)) && user.id!=us.id).ToList();
            var result = new LinkedList<object>();
            foreach (var user in users)
            {
                result.AddLast(new
                {
                    id=user.id,
                    name = user.name,
                    surname = user.surname,
                    photo = user.profile_photo,
                });
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        public void AddLike(int id)
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            like likes = new like();
            NewsFeed feed = db.NewsFeeds.Find(id);
            notification notif = new notification();
            notif.state = 1;
            notif.sender_id = us.id;
            notif.user_id = feed.user_id;
            notif.text_id = 6;
            notif.on_feed_id = id; 
            db.notifications.Add(notif);
            likes.Newsfeed_id = id;
            likes.liker_id = us.id;
            db.likes.Add(likes);
            db.SaveChanges();
        }
        public void DeleteLike(int id)
        {
            string currid = Request.Params["id"].ToString();
            user us = Session["User" + currid] as user;
            like likes = new like();
            likes = db.likes.Where(m => m.liker_id == us.id && m.Newsfeed_id == id).ToList().First();
            db.likes.Remove(likes);
            db.SaveChanges();
        }
        public JsonResult ChangeExPas()
        {
            int id = int.Parse(Request.Params["id"].ToString());
            passwordHash hash = new passwordHash();
            user us = db.users.Find(id);
            string pasEx = Request.Params["pasEx"].ToString();
            string pas = Request.Params["pas"].ToString();
            string pasConf = Request.Params["pasConf"].ToString();
            if (pas != pasConf)
            {
                object res = new object();
                res= (new {
                    error="passwods are not same",
                });
            return Json(res, JsonRequestBehavior.AllowGet);
            }else if (hash.CreateMd5(pasEx) != us.password)
            {
                object res = new object();
                res = (new
                {
                    error = "enter correct password",
                });
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var user = new user { id = us.id, password = hash.CreateMd5(pas) };
                using (var db = new socialEntities())
                {
                    db.users.Attach(user);
                    db.Entry(user).Property(x => x.password).IsModified = true;
                    db.SaveChanges();
                }
                object res = new object();
                res = (new
                {
                    error = "success",
                });
                return Json(res, JsonRequestBehavior.AllowGet);
            }
        }
        public JsonResult Redact()
        {
            int id = int.Parse(Request.Params["id"].ToString());
            user us = db.users.Find(id);
            string name = Request.Params["name"].ToString();
            string suname = Request.Params["surname"].ToString();
            int age =int.Parse( Request.Params["age"].ToString());
            string login =Request.Params["login"].ToString();
            List<user> usrs = db.users.Where(m => m.login == login).ToList();
            object res = new object();
            if (login.Length <= 6 && login!="0")
            {
                res = (new
                {
                    error = "login will contain more than 6 symbol",
                });
                return Json(res, JsonRequestBehavior.AllowGet);
            } else if (usrs.Count> 0 && login != "0")
            {
                res = (new
                {
                    error = "login already excist choose another",
                });
                return Json(res, JsonRequestBehavior.AllowGet);
            }
            if (name!="0" || suname!= "0" || age.ToString()!= "0")
            {
                if(name!= "0")
                {
                var user = new user { id = us.id, name=name};
                using (var db = new socialEntities())
                {
                    db.users.Attach(user);
                    db.Entry(user).Property(x => x.name).IsModified = true;
                    db.SaveChanges();
                }
                }
                if (suname != "0")
                {
                    var user = new user { id = us.id, surname = suname };
                    using (var db = new socialEntities())
                    {
                        db.users.Attach(user);
                        db.Entry(user).Property(x => x.surname).IsModified = true;
                        db.SaveChanges();
                    }
                }
                if (age.ToString() != "0")
                {
                    var user = new user { id = us.id, age = age };
                    using (var db = new socialEntities())
                    {
                        db.users.Attach(user);
                        db.Entry(user).Property(x => x.age).IsModified = true;
                        db.SaveChanges();
                    }
                }
            }
            res = (new
            {
                error = "success",
            });
            return Json(res, JsonRequestBehavior.AllowGet);
        }
    }
 }
    
