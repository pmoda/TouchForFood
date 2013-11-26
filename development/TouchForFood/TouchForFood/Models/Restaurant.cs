//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace TouchForFood.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class restaurant
    {
        public restaurant()
        {
            this.version = 1;
            this.menus = new HashSet<menu>();
            this.restaurant_user = new HashSet<restaurant_user>();
            this.reviews = new HashSet<review>();
            this.tables = new HashSet<table>();
            this.waiters = new HashSet<waiter>();
        }
    
        public int id { get; set; }
        public string name { get; set; }
        public string address { get; set; }
        public string postal_code { get; set; }
        public string city { get; set; }
        public Nullable<decimal> rating { get; set; }
        public int version { get; set; }
        public bool is_deleted { get; set; }
    
        public virtual ICollection<menu> menus { get; set; }
        public virtual ICollection<restaurant_user> restaurant_user { get; set; }
        public virtual ICollection<review> reviews { get; set; }
        public virtual ICollection<table> tables { get; set; }
        public virtual ICollection<waiter> waiters { get; set; }
    }
}
