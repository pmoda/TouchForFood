using System.Collections.Generic;
using TouchForFood.Models;
using TouchForFood.Util.Review;
namespace TouchForFood.ViewModels
{
    public class ReviewVM
    {
        public string reviewText { get; set; }
        public List<review_order_item> review_order_items { get; set; }
        public review review { get; set; }
        public bool is_anonymous {get;set;}
        public order order;
        public ReviewVM(review review,order order)
        {
            this.is_anonymous = false;
            this.review = review;
            this.review_order_items = new List<review_order_item>();
            this.order = order;
            createReviewOrderItemList(order);
        }

        public ReviewVM()
        {
        }
        /// <summary>
        /// for each order item in the order, create a place holder in the review order items
        /// </summary>
        /// <param name="order"></param>
        private void createReviewOrderItemList(order order)
        {
            foreach (order_item oi in order.order_item)
            {
                if (oi.review_order_item.Count == 0)
                {
                    review_order_item roi = new review_order_item();
                    roi.order_item = oi;
                    roi.order_item_id = oi.id;
                    review_order_items.Add(roi);
                }
            }
        }

        public static string Check(double lower, double upper, double toCheck)
        {
            return toCheck > lower && toCheck <= upper ? " checked=\"checked\"" : null;
        }
    }
}