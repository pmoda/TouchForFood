using System.Collections.Generic;
using System.Web.Mvc;
using System.ComponentModel;
using TouchForFood.Models;

namespace TouchForFood.Util.Review
{
    public class Rating
    {
        private static List<SelectListItem> ratings;

        public enum ReviewRatings
        {
            [Description("1")]
            ONE = 1,
            [Description("2")]
            TWO = 2,
            [Description("3")]
            THREE = 3,
            [Description("4")]
            FOUR = 4,
            [Description("5")]
            FIVE = 5,
            LAST_STATUS
        };

        public static List<SelectListItem> GetRatings()
        {

            ratings = new List<SelectListItem>();

            const int max = (int)ReviewRatings.LAST_STATUS;
            for (int i = 1; i < max; i++)
            {
                SelectListItem temp = new SelectListItem();
                temp.Text = i + "";
                temp.Value = i + "";
                ratings.Add(temp);
            }
            ratings[0].Selected = true;
            return ratings;
        }

        public static decimal CalculateAverageRating(List<review_order_item> sessionList)
        {
            decimal average = 0.0m;
            for (int i = 0; i < sessionList.Count; i++)
            {
                if (sessionList[i].rating != null)
                {
                    average += (decimal)sessionList[i].rating;
                }
            }
            return average / sessionList.Count;
        }

        /// <summary>
        /// Get the ratings from the postback list and add them to the session list.
        /// </summary>
        /// <param name="sessionList">The current information stored in the list</param>
        /// <param name="postBackList">The information sent back by the user</param>
        public static void GetRatingReviewOrderItems(List<review_order_item> sessionList, List<review_order_item> postBackList)
        {
            if (postBackList != null)
            {
                for (int i = 0; i < postBackList.Count; i++)
                {
                    if (i < sessionList.Count)
                    {
                        //get the ratings
                        sessionList[i].rating = postBackList[i].rating;
                    }
                }
            }
            else
            {
                for (int i = 0; i < sessionList.Count; i++)
                {
                    //get the ratings
                    sessionList[i].rating = 1;
                }
            }
        }
    }
}