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
    
    public partial class bill
    {
        public bill()
        {
            this.version = 1;
            this.order_item = new HashSet<order_item>();
        }
    
        public int id { get; set; }
        public Nullable<int> order_id { get; set; }
        public Nullable<decimal> tvq { get; set; }
        public Nullable<decimal> tps { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<System.DateTime> timestamp { get; set; }
        public Nullable<bool> paid { get; set; }
        public Nullable<bool> is_deleted { get; set; }
        public int version { get; set; }
    
        public virtual order order { get; set; }
        public virtual ICollection<order_item> order_item { get; set; }
    }
}
