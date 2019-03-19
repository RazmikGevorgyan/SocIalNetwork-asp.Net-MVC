using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Web;

namespace Soc.Models
{
    public class NotifAutoSender
    {
        var waitHandle = new AutoResetEvent(false);
        ThreadPool.RegisterWaitForSingleObject(waitHandle,(state, timeout) =>{}, 
    null, 
    TimeSpan.from(5), 
    false
);
   }
}