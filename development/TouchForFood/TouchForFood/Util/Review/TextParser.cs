using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using System.Web.Mvc;
using TouchForFood.Mappers;
using System.Text.RegularExpressions;

namespace TouchForFood.Util.Review
{
    public class TextParser
    {

        /// <summary>
        /// The review text should be separated by hash tags for each item.
        /// Search for the hash tags, then determine what element belongs to which review order item in the session list.
        /// If there are no hashtags, just add it to the first element.
        /// </summary>
        /// <param name="sessionList">The current information stored in the list</param>
        /// <param name="reviewText">Review text submitted by the user</param>
        public static void ParseReviewText(List<review_order_item> sessionList, string reviewText)
        {
            //THE P TAG IS HARDCODED INTO THE REVIEW/CREATE.CSHTML DUE TO A FF/CHROME BUG
            //IN THE $("span.review_order_item").click FUNCTION.
            //IF THAT CHANGES , CHANGE THIS
            reviewText = Regex.Replace(reviewText, @"<p[^>]*>", String.Empty);
            reviewText = reviewText.Replace("</p>", "");
            //get the ids of from the spans
            string[] ids = Regex.Split(reviewText, "(id=\"\\d+\")");
            //from the ids we need to get just the numeric values
            List<int> numericIDs = new List<int>();
            Dictionary<int, string> reviewsPerId = new Dictionary<int, string>();
            for (int i = 0; i < ids.Length; i++)
            {
                int id = -1;
                string review = "";
                if (ids[i].Contains("id="))
                {
                    string temp = Regex.Match(ids[i], @"\d+").Value;
                    id = Int32.Parse(temp);
                }
                //id was found
                if (id != -1)
                {
                    //the id was found now we can look ahead
                    for (int j = i; j < ids.Length; j++)
                    {
                        if (ids[j].Contains("</span>"))
                        {
                            string startTag = "</span>";
                            int startIndex = ids[j].IndexOf(startTag) + startTag.Length;
                            int endIndex = ids[j].IndexOf("<span", startIndex);
                            //if there is no opening span it means this is the last element. so just grab everything until the end of the string
                            if (endIndex > -1)
                            {
                                review = ids[j].Substring(startIndex, endIndex - startIndex);
                            }
                            else
                            {
                                review = ids[j].Substring(startIndex);
                            }

                            review = review.Replace("&nbsp;", " ");
                            review = Regex.Replace(review, "<.*?>", string.Empty);
                            reviewsPerId.Add(id, review);
                            break;
                        }
                    }
                }
            }

            //we can now build the reviews based on the order items that were passed in by name
            for (int i = 0; i < sessionList.Count; i++)
            {
                //get the order item id
                int orderItemID = sessionList[i].order_item.id;
                if (reviewsPerId.ContainsKey(orderItemID))
                {
                    string text = StringUtilities.ExceptBlanks(reviewsPerId[orderItemID]);
                    if (!string.IsNullOrEmpty(text))
                    {
                        sessionList[i].review_text = reviewsPerId[orderItemID];
                    }
                }
            }
        }

        /// <summary>
        /// Given the list of the review order items, set their submitted on dates to today.
        /// </summary>
        /// <param name="sessionList">The current information stored in the list</param>
        public static void SetDate(List<review_order_item> sessionList)
        {
            for (int i = 0; i < sessionList.Count; i++)
            {
                sessionList[i].submitted_on = System.DateTime.Now;
            }
        }

    }
}