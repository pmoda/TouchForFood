using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using TouchForFood.Util.Security;
using System.Web.Security;
using System.Web.Mvc;
using TouchForFood.Mappers;

namespace TouchForFood.Util.User
{
    public class UserUtil
    {
        private static touch_for_foodEntities db = new touch_for_foodEntities();

        private const int MAX_RESULTS = 10;
        private const float MIN_CONFIDENCE = 0.75f;

        //Counters for User History stats
        private static int totalOrdersCount = 0;
        private static int totalOrderItemsCount;
        private static int totalReviewsCount;
        private static int metaDataCount;
        
        public static user getAuthenticatedUser(HttpRequestBase requestBase)
        {            
            try
            {
                db = new touch_for_foodEntities();
                HttpCookie formsCookie = requestBase.Cookies[FormsAuthentication.FormsCookieName];
                FormsAuthenticationTicket auth = FormsAuthentication.Decrypt(formsCookie.Value);
                int userID = int.Parse(auth.UserData);

                UserIM im = new UserIM();
                return im.find(userID);
            }
            catch (Exception)
            {
                return null;
            }
 
        }

        public static user getAuthenticatedUser(HttpContextBase httpContext)
        {
            try
            {
                db = new touch_for_foodEntities();
                String username = httpContext.User.Identity.Name;
                return db.users.FirstOrDefault(m => m.username.Equals(username, StringComparison.Ordinal));
            }
            catch (Exception)
            {
                return null;
            }

        }

        public static bool isUserRole(SiteRoles role, HttpContext httpContext)
        {
            return (int)role == (int)httpContext.Session["role"];
        }

        /// <summary>
        /// Gets suggested Dishes based on User History
        /// First builds a dictionary for metascores, then find all potential dishes
        /// It then computes a score for each of the dishes based on the metascore values
        /// Finally sorts and returns the best 10 dishes
        /// </summary>
        /// <param name="u"></param>
        /// <param name="r"></param>
        public static List<KeyValuePair<menu_item, int>> GetSuggestions(user u, restaurant r)
        {
            totalOrdersCount = 0;
            totalOrderItemsCount = 0;
            totalReviewsCount = 0;
            metaDataCount = 0;
            List<menu> menus = db.menus.Where(m => m.is_active == true && m.is_deleted == false && m.resto_id == r.id).ToList();
            List<menu_category> menu_categories = new List<menu_category>();
            List<menu_item> menu_items_available = new List<menu_item>();
            Dictionary<menu_item, int> ItemScores;
            Dictionary<string, int[]> MetaScores;
            //Build the Metascores using all past orders and reviews
            MetaScores = BuildMetaScores(u);

            //Find all available items we can order
            foreach (menu m in menus)
            {
                menu_categories.AddRange(m.menu_category.Where(mc => mc.is_active == true && mc.is_deleted == false));
            }
            foreach (menu_category mc in menu_categories)
            {
                menu_items_available.AddRange(mc.menu_item.Where(mi => mi.is_active == true && mi.is_deleted == false));
            }

            //Give a score to every potential menu item currently available
            ItemScores = CalculateItemScores(MetaScores, menu_items_available);
            
            return SortByScore(ItemScores);
        }

        /// <summary>
        /// Builds up the meta score dictionary which associates data to each metadata word
        /// The Values of this Dictionary are
        /// 0 - Total times found (count)
        /// 1 - Total times rated (rated)
        /// 2 - Total Rating value (sum)        
        /// </summary>
        /// <param name="u"></param>
        private  static Dictionary<string, int[]> BuildMetaScores(user u)
        {
            List<order> Orders = u.orders.Where(o => o.order_status == (int)OrderStatusHelper.OrderStatusEnum.COMPLETE).ToList();
            //List<order> Orders = u.orders.ToList();
            Dictionary<string, int[]> MetaScores = new Dictionary<string, int[]>();
            foreach (order o in Orders)
            {
                UserUtil.totalOrdersCount++;
                foreach (order_item oi in o.order_item)
                {
                    int reviewRating = 0;
                    if (oi.review_order_item.Count() > 0)
                    {
                        reviewRating = (int)oi.review_order_item.First().rating;
                    }
                    totalOrderItemsCount++;
                    string metaData = oi.menu_item.item.metadata;
                    string[] metaWords = null;
                    if (metaData != null)
                    {
                        metaWords = metaData.Split(' ');
                    }
                    foreach (string word in metaWords)
                    {
                        metaDataCount++;
                        if (MetaScores.ContainsKey(word))
                        {
                            MetaScores[word][0]++;
                        }
                        else
                        {
                            MetaScores[word] = new int[]{1,0,0};
                        }
                        if (reviewRating != 0)
                        {
                            MetaScores[word][1]++;
                            MetaScores[word][2] += reviewRating;
                        }
                    }
                }

            }
            return MetaScores;
        }

        /// <summary>
        /// Maps each possible item to a score based on the values in MetaScores
        /// (see BuildMetaScores)
        /// Rating% = sum * 100 / rated * 5
        /// Score = count * Rating% / totOrders
        /// We then take the average score for all non zero meta scores
        /// </summary>
        private static Dictionary<menu_item, int> CalculateItemScores(Dictionary<string, int[]> scores, List<menu_item> available_items)
        {
            Dictionary<menu_item, int> ItemScores = new Dictionary<menu_item, int>();
            foreach (menu_item mi in available_items)
            {
                if (mi.item.metadata.Length != 0)
                {
                    int totalScore = 0;
                    int count = 0;
                    int wordMisses = 0;
                    string[] metaWords = mi.item.metadata.Split(' ');
                    foreach (string word in metaWords)
                    {
                        ++count;
                        if (scores.ContainsKey(word))
                        {
                            //word is ther so check for a rating
                            if (scores[word][1] == 0)
                            {
                                totalScore += Math.Min(scores[word][0] * 100 / totalOrdersCount, 100);
                                ++wordMisses;
                                //Only half a miss because we found the word so score is still incremnted
                            }
                            else
                            {
                                int ratingPercent = (scores[word][2] * 100) / (scores[word][1] * 5);
                                int score = scores[word][0] * ratingPercent / totalOrdersCount;
                                totalScore += Math.Min(score, 100);
                            }
                        }
                        //Complete miss of the word
                        else { ++wordMisses; }
                    }
                    //Not a single match with the metadate for this item
                    if (count == 0) { ItemScores.Add(mi, 0); }
                    else
                    {
                        totalScore = (int)Math.Min((totalScore / count) * ConfidenceValue(wordMisses, count), 100);
                        ItemScores.Add(mi, Math.Max(0, totalScore));
                    }
                }
            }
            return ItemScores;
        }

        //The confidence interval is based on words in the menu item's metadata that were not found or had no rating
        //  versus the total words contained in the object's data

        //Since there is no user data linked to the word other than the fact that it was ordered, we reduce our confidence
        // that this is a desired attribute for a meal.
        private static float ConfidenceValue(int wordMisses, int total)
        {
            float c = 1;
            c = 1 - ((1 - MIN_CONFIDENCE) * (wordMisses / total));
            return c;
        }
        private static List<KeyValuePair<menu_item, int>> SortByScore(Dictionary<menu_item, int> dictionary)
        {
            List<KeyValuePair<menu_item, int>> myList = dictionary.ToList();

            myList.Sort((firstPair, nextPair) =>
            {
                return nextPair.Value.CompareTo(firstPair.Value);
            }
            );
            
            //Take the top 10 ranked items
            myList = myList.GetRange(0, Math.Min(MAX_RESULTS, myList.Count));
            return myList;
        }
        
    };
}