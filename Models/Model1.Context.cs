﻿//------------------------------------------------------------------------------
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
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class socialEntities : DbContext
    {
        public socialEntities()
            : base("name=socialEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<Country> Countries { get; set; }
        public virtual DbSet<friend> friends { get; set; }
        public virtual DbSet<sysdiagram> sysdiagrams { get; set; }
        public virtual DbSet<user> users { get; set; }
        public virtual DbSet<follower> followers { get; set; }
        public virtual DbSet<NewsFeed> NewsFeeds { get; set; }
        public virtual DbSet<Profile> Profiles { get; set; }
        public virtual DbSet<messenger1> messenger1 { get; set; }
        public virtual DbSet<comment> comments { get; set; }
        public virtual DbSet<like> likes { get; set; }
        public virtual DbSet<notification> notifications { get; set; }
        public virtual DbSet<notification_text> notification_text { get; set; }
        public virtual DbSet<feedState> feedStates { get; set; }
        public virtual DbSet<intermediate_storage> intermediate_storage { get; set; }
        public virtual DbSet<advert> adverts { get; set; }
    }
}
