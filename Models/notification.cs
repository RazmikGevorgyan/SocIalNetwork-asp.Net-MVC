//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Soc.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class notification
    {
        public int id { get; set; }
        public Nullable<int> user_id { get; set; }
        public Nullable<int> text_id { get; set; }
        public Nullable<int> state { get; set; }
        public Nullable<System.DateTime> datetime { get; set; }
        public Nullable<int> sender_id { get; set; }
        public Nullable<int> on_feed_id { get; set; }
    
        public virtual user user { get; set; }
        public virtual user user1 { get; set; }
        public virtual notification_text notification_text { get; set; }
    }
}
