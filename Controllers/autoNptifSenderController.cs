using Soc.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace Soc.Controllers
{
    public class autoNptifSenderController : Controller
    {
        private socialEntities context;
        public autoNptifSenderController()
        {
            this.context = new socialEntities();
        }
        // GET: autoNptifSender
        public ActionResult Index()
        {
            var waitHandle = new AutoResetEvent(false);
            ThreadPool.RegisterWaitForSingleObject(
                waitHandle,
                (state, timeout) =>
                {
                    foreach(user us in context.users)
                    {
                        notification not = new notification();
                        not.user_id = us.id;
                        not.text_id = 9;
                        not.state = 1;
                    }
                },
                null,
                TimeSpan.FromSeconds(5),
                false
            );
            return View();
        }
    }
}