using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.ViewModels;
using TouchForFood.Util.Search;
using TouchForFood.Util.Security;
using TouchForFood.Mappers.Search;

namespace TouchForFood.Controllers
{
    public class SearchController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();

        private IDictionary<int, SearchViewModel> dictionary = new Dictionary<int, SearchViewModel>();

        private IDictionary<string, IDictionary<int, IList<int>>> invertedIndex = new Dictionary<string, IDictionary<int, IList<int>>>();

        // constants for positions in inverted index
        private const int constNameDesc = 0;
        private const int constMetadata = 1;
        private const int constCatName = 2;
        private const int constMenuName = 3;

        //
        // Should be able to search:
        // Item:
        // - name, description, metadata
        // Category:
        // - name
        // Menu:
        // - name
        // GET: /Search/Search/
        public ActionResult Search(string searchString)
        {
            IDictionary<SearchViewModel, int> searchResults = new Dictionary<SearchViewModel, int>();

            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);

            if (authUser == null)
            {
                return RedirectToAction("LogOn", "User");
            }

            int restoId = -1;

            try
            {
                if (authUser.user_role == (int)SiteRoles.Admin || authUser.user_role == (int)SiteRoles.Restaurant)
                {
                    // if user has admin or restaurant role, get that restaurant's id
                    restoId = new SearchIM(db).findByUser(authUser.id);
                }
                else if (authUser.table.restaurant_id.HasValue) // check if the user is in a restaurant
                {
                    restoId = authUser.table.restaurant_id.Value;
                }

                if (restoId != -1)
                {
                    try
                    {
                        // get the dictionary and inverted index from the application
                        dictionary = ((Dictionary<int, IDictionary<int, SearchViewModel>>)HttpContext.Application["dictionary"])[restoId];
                        invertedIndex = ((Dictionary<int, IDictionary<string, IDictionary<int, IList<int>>>>)HttpContext.Application["invertedIndex"])[restoId];

                        IList<string> searchList = SearchUtil.Normalize(searchString);

                        foreach (var word in searchList)
                        {
                            if (invertedIndex.ContainsKey(word))
                            {
                                foreach (var list in invertedIndex[word])
                                {
                                    int counter = 0;

                                    counter = (list.Value[constNameDesc] * 2) + counter;
                                    counter = (list.Value[constMetadata] * 2) + counter;
                                    counter = (list.Value[constCatName]) + counter;
                                    counter = (list.Value[constMenuName]) + counter;

                                    if (searchResults.ContainsKey(dictionary[list.Key]))
                                    {
                                        searchResults[dictionary[list.Key]] += counter;
                                    }
                                    else
                                    {
                                        searchResults[dictionary[list.Key]] = counter;
                                    }
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                        // do nothing. Catches exceptions so nothing is thrown. No results will be returned.
                    }
                }
            }
            catch (NullReferenceException)
            {
                // do nothing. catches exception so nothing is thrown. No results will be returned.
            }

            ViewBag.SearchString = searchString;
            return View(searchResults);
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}
