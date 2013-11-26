using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace TouchForFood.Models
{
    [MetadataType(typeof(OrderItemMetadata))]
    public partial class order_item
    {
       /**public override bool Equals(object obj)
        {
            order_item other = (order_item)obj;
            return (int)other.id == (int)this.id;
        }**/
    }

    public class OrderItemMetadata
    {
        //[Display(Name = "Price")]
        //[Required]
        //public decimal rating { get; set; }
    }

}