using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.Attributes;
using System.Data.Entity.Validation;
using System.Diagnostics;
using TouchForFood.Mappers;
using TouchForFood.Util.Security;
using TouchForFood.App_GlobalResources;

namespace TouchForFood.Controllers
{ 
	public class TableController : Controller
	{
		private touch_for_foodEntities db = new touch_for_foodEntities();
		private TableIM im;
		private TableOM om;

		public TableController()
		{
			im = new TableIM(db);
			om = new TableOM(db);
		}

       // [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
		public ViewResult Index()
		{
            List<table> tables = new List<table>();
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
            if (authUser.restaurant_user.Count != 0)
            {
                try
                {
                    int resto_id = (int)authUser.restaurant_user.First().restaurant_id;
                    tables = db.tables.Where(t => t.restaurant_id == resto_id).ToList();
                }
                catch(InvalidOperationException){
                    
                }
            }
            else //if (authUser.user_role == (int)SiteRoles.Developer)
            {
                tables = db.tables.Include(t => t.restaurant).ToList();
            }
			return View(tables);
		}

        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
		public ViewResult ManageIndex(int? id)
		{
            List<table> tables = new List<table>();
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
            if (authUser.restaurant_user.Count != 0)
            {
                int resto_id = (int)authUser.restaurant_user.First().restaurant_id;
                tables = db.tables.Where(t => t.restaurant_id == resto_id).ToList();
            }
            else if(authUser.user_role == (int)SiteRoles.Developer)
            {
			    tables = db.tables.Include(t => t.restaurant).ToList();
            }

			var sortedTables = new List<table>();
			foreach (var t in tables)
			{
				var orders = t.orders
					.Where(p => p.order_status == (int)OrderStatusHelper.OrderStatusEnum.PLACED ||
					p.order_status == (int)OrderStatusHelper.OrderStatusEnum.EDITING ||
                    p.order_status == (int)OrderStatusHelper.OrderStatusEnum.PROCESSING).ToList();
				if (orders.Count > 0)
				{
					sortedTables.Add(t);
				}
			}

			// place tables in a dictionary for sorting
			IDictionary<DateTime, table> tableHashMap = new Dictionary<DateTime, table>();

			foreach (var t in sortedTables)
			{
				DateTime minTime = DateTime.Now;

				foreach (var o in t.orders.Where(p => p.order_status == (int)OrderStatusHelper.OrderStatusEnum.PLACED ||
					p.order_status == (int)OrderStatusHelper.OrderStatusEnum.EDITING ||
                    p.order_status == (int)OrderStatusHelper.OrderStatusEnum.PROCESSING))
				{
					int result = DateTime.Compare(o.timestamp, minTime);

					if (result < 0)
					{
						minTime = o.timestamp;
					}
				}

				tableHashMap.Add(minTime, t);
			}

			int minTableId = -1;

			if (tableHashMap.Count > 0)
			{
				minTableId = tableHashMap.ToList().OrderBy(i => i.Key).First().Value.id;
			}

			if (id == null)
			{
				id = minTableId;
			}
			
			ViewBag.table_id = id;
			ViewBag.table = db.tables.Find(id);
			return View("Manage", tableHashMap);
		}

		//
		// GET: /Table/Details/5
		[CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
		public ViewResult Details(int id)
		{
			//table table = db.tables.Find(id);
			return View(im.find(id));
		}

		//
		// GET: /Table/Create
		[CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
		public ActionResult Create()
		{
			List<restaurant> restoForCurrentUser = Util.Table.TableUtil.GetRestaurants(Request);
            //if the list of resto for current user is empty return an error
            if (restoForCurrentUser.Count != 0)
            {
                ViewBag.restaurant_id = new SelectList(CreateRestaurantList(), "id", "name");
            }
            else
            {
                ViewBag.restaurant_id = new SelectList(restoForCurrentUser);
                ViewBag.error = Global.TableCreateError;
            }

			return View();
		} 

		//
		// POST: /Table/Create
		[HttpPost]
		[CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
		public ActionResult Create(table table)
		{
            if (ModelState.IsValid)
            {
                try
                {
                    if (table.restaurant_id != null)
                    {
                        if (om.Create(table))
                        {
                            return RedirectToAction("Index");
                        }
                    }
                }
                catch (Exception) { }
            }
            ViewBag.Error = Global.TableCreateError;
            ViewBag.restaurant_id = new SelectList(CreateRestaurantList(), "id", "name");
           // ViewBag.restaurant_id = new SelectList(db.restaurants, "id", "name", table.restaurant_id);
                       
			return View(table);
		}
		
		//
		// GET: /Table/Edit/5
		[CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
		public ActionResult Edit(int id)
		{
			table table = im.find(id);
			ViewBag.restaurant_id = new SelectList(db.restaurants, "id", "name", table.restaurant_id);
			return View(table);
		}

		//
		// POST: /Table/Edit/5
		[HttpPost]
		[CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
		public ActionResult Edit(table table)
		{
			if (ModelState.IsValid)
			{
				db.Entry(table).State = EntityState.Modified;
				db.SaveChanges();
				return RedirectToAction("Index");
			}
            ViewBag.Error = Global.ServerError;
            try
            {
                ViewBag.restaurant_id = new SelectList(db.restaurants, "id", "name", table.restaurant_id);
            }
            catch (Exception)
            {
            }
			return View(table);
		}

		//
		// GET: /Table/Delete/5
		[CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
		public ActionResult Delete(int id)
		{
			//table table = db.tables.Find(id);
			return View(im.find(id));
		}

		//
		// POST: /Table/Delete/5
		[HttpPost, ActionName("Delete")]
		[CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
		public ActionResult DeleteConfirmed(int id)
		{
			try
			{
				om.delete(id);
				return RedirectToAction("Index");
			}
			catch (Exception)
			{
                ViewBag.Error = Global.ServerError;
			}
			return View(im.find(id));           
		}

		//
		// GET: /Table/Assign
		[HttpGet, Ajax(false)]
		public ActionResult Assign(string id)
		{
			UserIM uim = new UserIM(db);
			Util.Security.AES aes = new Util.Security.AES();
			int tableId = Int16.Parse(aes.DecryptString(id));
			table tab = im.find(tableId);
			user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);

			if (authUser == null)
			{
				return RedirectToAction("LogOn", "User");
			}

			user dbUser = uim.find(authUser.id);
			dbUser.current_table_id = tab.id;

			// TODO: This is a hack, the User crud is broken.
			dbUser.ConfirmPassword = dbUser.password;
			try
			{
				db.SaveChanges();
			}
			catch (DbEntityValidationException dbEx)
			{
				foreach (var validationErrors in dbEx.EntityValidationErrors)
				{
					foreach (var validationError in validationErrors.ValidationErrors)
					{
						Trace.TraceInformation("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);
					}
				}
			}

			return RedirectToAction("UserMenu", "Menu");
		}

		protected override void Dispose(bool disposing)
		{
			db.Dispose();
			base.Dispose(disposing);
		}

        /// <summary>
        /// Make a list of tables that could be associated to creating a service request depending on
        /// the user.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        private List<restaurant> CreateRestaurantList()
        {
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
            List<restaurant> restaurantList = new List<restaurant>();

            if (authUser.user_role == (int)SiteRoles.Restaurant)
            {
                foreach (restaurant_user ru in authUser.restaurant_user)
                {
                    restaurantList.Add(ru.restaurant);
                }
            }
            else if (authUser.user_role == (int)SiteRoles.Admin ||
                    authUser.user_role == (int)SiteRoles.Developer)
            {
                restaurantList = db.restaurants.ToList<restaurant>();
            }

            return restaurantList;
        }
	}          
}