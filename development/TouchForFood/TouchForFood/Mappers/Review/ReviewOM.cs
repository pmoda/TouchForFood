using System.Data;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Models;
using System.Web.Mvc;
namespace TouchForFood.Mappers
{
    public class ReviewOM : GenericOM
    {

        public ReviewOM():base(){}

        public ReviewOM(touch_for_foodEntities db):base(db){}

        /// <summary>
        /// Writes a review object to the database associated with a user
        /// </summary>
        /// <param name="review">The review object to write</param>
        /// <param name="user">The user to associate the review with</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Create(review review, user user)
        {
            review.user_id = user.id;
            return Create(review);
        }

        /// <summary>
        /// Writes a review object to the database
        /// </summary>
        /// <param name="review">The review object to write</param>
        /// <returns>True if successful, false otherwise</returns>
        public bool Create(review review)
        {
            db.reviews.Add(review);
            return (db.SaveChanges() == 1);
        }

        /// <summary>
        /// When deleting a review, the restaurant rating needs to be recalculated
        /// </summary>
        /// <param name="review">review to delete</param>
        /// <returns>the number of rows affected by the database change</returns>
        public override int delete(int id)
        {
            review review = db.reviews.Find(id);
            restaurant restaurant;
            int result = 0;

            //get the restaurant id this rating is associated to.
            restaurant = review.restaurant;

            //delete this review
            db.reviews.Remove(review);

            //recalculate the restaurants rating
            decimal rating = calculateRating(restaurant);
            restaurant.rating = rating;
            db.Entry(restaurant).State = EntityState.Modified;
            
            result = db.SaveChanges();
            return result;
        }

        /// <summary>
        /// Calculates the restaurant's rating
        /// </summary>
        /// <param name="restaurant">Restaurant who's rating we need to caluclate</param>
        /// <returns>rating - a decimal representing the restaurant rating</returns>
        private decimal calculateRating(restaurant restaurant)
        {
            if (restaurant.reviews.Count > 0)
            {
                decimal allRatings = 0.0m;

                foreach (review r in restaurant.reviews)
                {
                    allRatings += r.rating;
                }
                return allRatings / restaurant.reviews.Count;
            }
            else
            {
                return 0;
            }
        }

    }
}