using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Soc.Models
{
    public class auto_notification_Sender
    {
        var waitHandle = new AutoResetEvent(false);
        ThreadPool.RegisterWaitForSingleObject(
                waitHandle,
                (state, timeout) =>
                {
                    foreach (user us in context.users)
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
    }
}