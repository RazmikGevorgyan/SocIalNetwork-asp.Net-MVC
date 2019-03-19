using Soc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;
using Owin;


namespace Soc.Controllers
{
    public class Auto_Notification_SenderController : Controller
    {
        socialEntities db;
        public Auto_Notification_SenderController()
        {
            this.db = new socialEntities();
        }
        // GET: Auto_Notification_Sender
        public void Index()
        {
            foreach (user us in db.users)
            {
                if (us.stat == 1)
                {
                    continue;
                }
                notification not = new notification();
                not.sender_id = 25;
                not.user_id = us.id;
                not.text_id = 9;
                not.state = 1;
                not.datetime = DateTime.Now;
                db.notifications.Add(not);
              
            }
            db.SaveChanges();
        }
    }
}