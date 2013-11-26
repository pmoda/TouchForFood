using System;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using TouchForFood.Mappers;
using TouchForFood.Models;
using TouchForFood.Util.Security;
using TouchForFood.Util.User;
using System.Collections.Generic;
using TouchForFood.App_GlobalResources;
using TouchForFood.ViewModels;
using TouchForFood.Util.Item;

namespace TouchForFood.Controllers
{
    public class UserController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();
        private UserIM im;
        private UserOM om;

        public UserController()
        {
            im = new UserIM(db);
            om = new UserOM(db);
        }
        //
        // GET: /User/
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Developer)]
        public ViewResult Index()
        {
            ViewBag.Message = Global.TouchForFoodUserList;
            return View(im.find());
        }

        public ViewResult SuggestItems()
        {            
            UserIM uim = new UserIM(db);
            TableIM im = new TableIM(db);            
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
            if (authUser == null)
            {
                return View("LogOn", "User");
            }
            user dbUser = uim.find(authUser.id);
            int t_id = (int)dbUser.current_table_id;
            restaurant r = db.tables.Find(t_id).restaurant;

            ViewBag.Suggest = "Items are suggested based on your ratings and order history.";

            return View("SuggestedItems", UserUtil.GetSuggestions(authUser, r));
        }

        public ViewResult PopularItems()
        {
            UserIM uim = new UserIM(db);
            TableIM im = new TableIM(db);
            ReportsIM rim = new ReportsIM(db);
            MenuItemIM miIM = new MenuItemIM(db);

            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
            if (authUser == null)
            {
                return View("LogOn", "User");
            }
            user dbUser = uim.find(authUser.id);
            int t_id = (int)dbUser.current_table_id;
            restaurant r = db.tables.Find(t_id).restaurant;

            IEnumerable<MostPopularDishViewModel> mostPopular = rim.findMostPopularCustomer(r);
            List<KeyValuePair<TouchForFood.Models.menu_item, int>> list = new List<KeyValuePair<TouchForFood.Models.menu_item, int>>();
            foreach(var dish in mostPopular){
                menu_item mi = miIM.find(dish.menuItemId);

                list.Add(new KeyValuePair<menu_item,int>(mi,dish.timesOrdered));
            }

            ViewBag.Suggest = "Items are ranked according to the number of times they have been ordered.";

            return View("SuggestedItems", list);
        }

        public ViewResult PopularItemsByRating(menu menu)
        {
            UserIM uim = new UserIM(db);
            TableIM im = new TableIM(db);
            ReportsIM rim = new ReportsIM(db);
            MenuItemIM miIM = new MenuItemIM(db);
            
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
            if (authUser == null)
            {
                return View("LogOn", "User");
            }
            user dbUser = uim.find(authUser.id);
            int t_id = (int)dbUser.current_table_id;
            restaurant r = db.tables.Find(t_id).restaurant;

            IEnumerable<MostPopularDishViewModel> mostPopular = rim.findMostPopularCustomer(r);
            List<KeyValuePair<TouchForFood.Models.menu_item, double>> list = new List<KeyValuePair<TouchForFood.Models.menu_item, double>>();
            foreach(var dish in mostPopular){
                menu_item mi = miIM.find(dish.menuItemId);

                list.Add(new KeyValuePair<menu_item,double>(mi,ItemUtil.getAverageRating(mi)));
            }

            list.Sort((firstPair, nextPair) =>
            {
                return nextPair.Value.CompareTo(firstPair.Value);
            }
            );

            ViewBag.Suggest = "Items are ranked according to rating.";

            return View("SuggestedByRating", list);
        }

        //
        // GET: /User/Details/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Customer | SiteRoles.Developer)]
        public ViewResult Details(int id)
        {
            return View(im.find(id));
        }

        //
        // GET: /User/Create
        public ActionResult Create()
        {
            ViewBag.Message = Global.UserCreateProfile;
            return View();
        }

        private void HandleDbUpdateException(DbUpdateException e)
        {
            /* We can either have: 
            - A duplicate username (defined by the "username_is_unique" index in the DB)
            - A duplicate email address (defined by the "email_is_unique" index)
            Anything else is handled by a generic error message 
            */
            if (e.InnerException.InnerException.Message.Contains("username_is_unique"))
            {
                ModelState.AddModelError(Global.User_Username, Global.UserNameAlreadyExists);
            }
            else if (e.InnerException.InnerException.Message.Contains("email_is_unique"))
            {
                ModelState.AddModelError(Global.User_Email, Global.EmailAlreadyExists);
            }
            else
            {
                ModelState.AddModelError(string.Empty, Global.ServerError);
            }
        }

        //
        // POST: /User/Create
        [HttpPost]
        public ActionResult Create(user user)
        {
            if (ModelState.IsValid)
            {
                // Add the customer role to the new user object
                user.user_role = (int)SiteRoles.Customer;

                // Encrypt the user's password
                AES aes = new AES();
                user.password = aes.EncryptToString(user.password);
                user.ConfirmPassword = aes.EncryptToString(user.ConfirmPassword);
                
                // Try to add the user to the database and save the changes
                // Exception is thrown in case of errors (ex: unique field value is not respected)
                try
                {
                    if (!om.Create(user))
                    {
                        return View("Create");
                    }
                }
                catch (DbUpdateException e)
                {
                    HandleDbUpdateException(e);
                    // Return to the original page we were at. Any errors added into the model will be shown automatically by the view
                    return View("Create");
                }
                catch (Exception)
                {
                    ViewBag.Error = Global.ServerError;
                    return View("Create");
                }
                // Create a cookie with our user information
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.username, 
                                                                                DateTime.Now, DateTime.Now.AddDays(1), 
                                                                                false, user.id.ToString(), FormsAuthentication.FormsCookiePath); 

                // Now that the user was properly created we can add the customer role to the session
                HttpContext.Session["role"] = SiteRoles.Customer;

                // Encrypt the ticket
                string hashedTicket = FormsAuthentication.Encrypt(ticket);

                // Create the new cookie and add it into the response
                Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, hashedTicket));

                return RedirectToAction("Index", "Home");
            }

            // If we got to this point then something went wrong
            ViewBag.Error = Global.ServerError;
            return View("Create");
        }

        //
        // GET: /User/LogOff
        public ActionResult LogOff()
        {
            // Remove cookie authentication
            FormsAuthentication.SignOut();

            // Kill the session
            HttpContext.Session.Abandon();

            // Take'em home
            return RedirectToAction("Index", "Home");
        }

        //
        // GET: /User/Edit/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult Edit(int id)
        {
            SiteRoles currentUserRole = (SiteRoles)Util.User.UserUtil.getAuthenticatedUser(Request).user_role;

            if (UserUtil.getAuthenticatedUser(Request).id == id || currentUserRole == SiteRoles.Developer)
            {
                return View(im.find(id));
            }

            return RedirectToAction("Index", "Home");
        }

        //
        // POST: /User/Edit/5
        [HttpPost]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult Edit(user user, HttpPostedFileBase file)
        {
            // Get array of errors (if any)
            var errors = ModelState.Where(x => x.Value.Errors.Count > 0)
                                    .Select(x => new { x.Key, x.Value.Errors })
                                    .ToArray();

            // Only allow entry if the ModelState is valid or if we have an invalid ModelState that's caused by a blank (null) password
            if (ModelState.IsValid || 
                (!ModelState.IsValid && errors.Length == 1 
                && errors[0].Key.Equals("password", StringComparison.Ordinal)
                && (user.password == null && user.ConfirmPassword == null)))
            {
                try
                {
                    if (file != null && file.ContentLength > 0)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/uploads/user_images/"), "user_" + user.id + Path.GetExtension(fileName));
                        //Save the file in given location
                        file.SaveAs(path);
                        //Update the db to show where profile image is located
                        user.image_url = Path.Combine("~/uploads/user_images/", "user_" + user.id + Path.GetExtension(fileName));
                    }

                    // If the user did enter passwords, we hash them
                    if (user.password != null && user.ConfirmPassword != null)
                    {
                        // Encrypt the user's password
                        AES aes = new AES();
                        user.password = aes.EncryptToString(user.password);
                        user.ConfirmPassword = aes.EncryptToString(user.ConfirmPassword);
                    }


                    if (om.edit(user))
                        return RedirectToAction("Index", "Home");
                    else
                    {
                        ViewBag.Error = Global.VersioningError;
                    }
                }
                catch (Exception)
                {
                    ViewBag.Error = Global.ServerError;
                }
            }

            return View(user);
        }

        public PartialViewResult GetAllReviews(user u)
        {
            List<review_order_item> reviews = new List<review_order_item>();
            foreach (review rev in u.reviews)
            {
                reviews.AddRange(rev.review_order_item);
            }
            reviews.OrderByDescending(r => r.submitted_on).ToList();

            return PartialView("PastReviewsPartial", reviews);
        }

        //
        // GET: /User/Delete/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Developer)]
        public ActionResult Delete(int id)
        {
            return View(im.find(id));
        }

        //
        // POST: /User/Delete/5
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Developer)]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                om.delete(id);
            }
            catch (Exception) 
            {
                ViewBag.Error = Global.ServerError;
            }
            
            return RedirectToAction("Index");
        }

        // GET: /User/Login
        public ViewResult LogOn()
        {
            return View("LogOn");
        }

        [HttpPost]
        public ActionResult LogOn(string username, string password) // need something for remember me
        {
            if (ModelState.IsValid)
            {
                // Create our AES object so we can encrypt the password and compare
                AES aes = new AES();
                password = aes.EncryptToString(password);

                user user = db.users.FirstOrDefault(m => m.username.Equals(username, StringComparison.Ordinal) && 
                                                         m.password.Equals(password, StringComparison.Ordinal));

                if (user != null)
                {
                    FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.username,
                                                                                    DateTime.Now, DateTime.Now.AddDays(1),
                                                                                    false, user.id.ToString(), 
                                                                                    FormsAuthentication.FormsCookiePath);
                    // Encrypt the ticket
                    string hashedTicket = FormsAuthentication.Encrypt(ticket);

                    // Create the new cookie and add it into the response
                    Response.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, hashedTicket));

                    // Add the custom role
                    HttpContext.Session["role"] = user.user_role;

                    return RedirectToAction("Index", "Home"); 
                }
                else
                {
                    ModelState.AddModelError(string.Empty, Global.UsernamePasswordIncorrect);
                    return View("LogOn");
                }

            }
            return RedirectToAction("LogOn");
        }

        public List<restaurant> GetRestaurants()
        {
            return Util.Table.TableUtil.GetRestaurants(Request);
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

    }
}