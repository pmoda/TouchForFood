using System.Data;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Models;

namespace TouchForFood.Mappers
{
    public class Review_Order_ItemOM : GenericOM
    {
        public Review_Order_ItemOM():base(){}

        public Review_Order_ItemOM(touch_for_foodEntities db) : base(db) { }

        public override int delete(int id)
        {
            review_order_item review_order_item = db.review_order_item.Find(id);
            db.review_order_item.Remove(review_order_item);
            return db.SaveChanges();
        }

        public int Create(review_order_item roi)
        {
            if(roi.rating == null)
            {
                roi.rating = 1;
            }
            db.review_order_item.Add(roi);
            return db.SaveChanges();
        }
    }
}