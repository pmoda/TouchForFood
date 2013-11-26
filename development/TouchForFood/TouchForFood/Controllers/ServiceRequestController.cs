using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using TouchForFood.App_GlobalResources;
using TouchForFood.Mappers;
using TouchForFood.Models;
using TouchForFood.Util.Security;
using TouchForFood.Util.ServiceRequest;
using TouchForFood.ViewModels;

namespace TouchForFood.Controllers
{
    public class ServiceRequestController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();

        private ServiceRequestIM im;
        private ServiceRequestOM om;

        public ServiceRequestController()
        {
            im = new ServiceRequestIM(db);
            om = new ServiceRequestOM(db);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        #region Public
        /// <summary>
        /// GET: /ServiceRequest/Cancel
        /// </summary>
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Cancel(int id)
        {
            return View(im.find(id));
        }

        /// <summary>
        /// POST: /ServiceRequest/Cancel
        /// </summary>
        [HttpPost, ActionName("Cancel"), Authorize]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult CancelConfirmed(service_request service_request)
        {
            try
            {
                service_request.status = (int)ServiceRequestUtil.ServiceRequestStatus.CANCELLED;

                if (ModelState.IsValid)
                {

                    if (om.edit(service_request))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = Global.ServiceRequestLockError;
                    }
                }
                else
                {
                    ViewBag.Error = Global.ServerError;
                }
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException || e is ArgumentNullException)
                {
                    ViewBag.Error = Global.ServerError;
                }
                else
                {
                    throw e;
                }
            }

            return View(service_request);
        }

        /// <summary>
        /// GET: /ServiceRequest/Complete
        /// </summary>
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Complete(int id)
        {
            return View(im.find(id));
        }

        /// <summary>
        /// POST: /ServiceRequest/Complete
        /// </summary>
        [HttpPost, ActionName("Complete"), Authorize]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult CompleteConfirmed(service_request service_request)
        {
            try
            {
                service_request.status = (int)ServiceRequestUtil.ServiceRequestStatus.COMPLETED;

                if (ModelState.IsValid)
                {
                    if (om.edit(service_request))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = Global.ServiceRequestLockError;
                    }
                }
                else
                {
                    ViewBag.Error = Global.ServerError;
                }
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException || e is ArgumentNullException)
                {
                    ViewBag.Error = Global.ServerError;
                }
                else
                {
                    throw e;
                }
            }

            return View(service_request);
        }

        /// <summary>
        /// GET: /ServiceRequest/Create
        /// </summary>
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult Create()
        {
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
            if (authUser.user_role == (int)SiteRoles.Customer)
            {
                if (authUser.current_table_id == null)
                {
                    ViewBag.Information = Global.ServiceRequestClientNotAssignedTable;
                    return View();
                }

                ICollection<service_request> requests = im.FindByOpenRequestByTable((int)authUser.current_table_id);
                if (requests.Count > 0)
                {
                    ViewBag.Information = Global.ServiceRequestDuplicateRequestClient;
                }
            }

            ViewBag.table_id = new SelectList(CreateTableList(), "id", "name");
            return View();
        }

        /// <summary>
        /// POST: /ServiceRequest/Create
        /// </summary>
        [HttpPost, Authorize]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Customer | SiteRoles.Developer)]
        public ActionResult Create(service_request service_request)
        {
            try
            {
                user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);

                //Assign values to request attributes
                if (authUser.user_role == (int)SiteRoles.Customer && authUser.current_table_id != null)
                {
                    service_request.table_id = authUser.current_table_id;
                }
                service_request.created = DateTime.Now;
                service_request.status = (int)ServiceRequestUtil.ServiceRequestStatus.OPEN;
                service_request.version = 0;

                //Table should never be null.
                if (service_request.table_id == null)
                {
                    ViewBag.Error = Global.ServiceRequestTableNullError;
                }
                else if (!ServiceRequestExists(service_request) && ModelState.IsValid)
                {
                    if (om.Create(service_request))
                    {
                        if (authUser.user_role != (int)SiteRoles.Customer)
                        {
                            HttpContext.Session["message"] = Global.ServiceRequestAdminSuccess;
                            return RedirectToAction("Index");
                        }
                        HttpContext.Session["message"] = Global.ServiceRequestSuccess;
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ViewBag.Error = Global.ServiceRequestDuplicateRequestError;
                }

                ViewBag.table_id = new SelectList(CreateTableList(), "id", "name", service_request.table_id);
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException || e is ArgumentNullException)
                {
                    ViewBag.Error = Global.ServerError;
                }
                else
                {
                    throw e;
                }
            }

            return View(service_request);
        }

        /// <summary>
        /// GET: /ServiceRequest/Delete/5 
        /// </summary>
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Developer)]
        public ActionResult Delete(int id)
        {
            return View(im.find(id));
        }

        /// <summary>
        /// POST: /ServiceRequest/Delete/5
        /// </summary>
        [HttpPost, ActionName("Delete"), Authorize]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Developer)]
        public ActionResult DeleteConfirmed(int id)
        {
            try
            {
                om.delete(id);
            }
            catch (InvalidOperationException)
            {
                ViewBag.Error = Global.ServerError;
            }

            return RedirectToAction("Index");
        }

        /// <summary>
        /// GET: /ServiceRequest/Details/5
        /// </summary>
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ViewResult Details(int id)
        {
            service_request service_request = im.find(id);
            return View(service_request);
        }

        /// <summary>
        /// GET: /ServiceRequest/Edit/5
        /// </summary>
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(int id)
        {
            
            service_request service_request = im.find(id);
            try
            {
                //Make list of statuses from enum
                var statuses = from ServiceRequestUtil.ServiceRequestStatus s
                               in Enum.GetValues(typeof(ServiceRequestUtil.ServiceRequestStatus))
                               select new { Value = (int)s, Name = s.ToString() };

                ViewData["status"] = new SelectList(statuses, "Value", "Name", service_request.status);
                ViewBag.table_id = new SelectList(CreateTableList(), "id", "name", service_request.table_id);
            }
            catch(ArgumentNullException)
            {
                ViewBag.Error = Global.ServerError;
            }
            return View(service_request);
        }

        /// <summary>
        /// POST: /ServiceRequest/Edit/5
        /// </summary>
        [HttpPost, Authorize]
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ActionResult Edit(service_request service_request)
        {
            try
            {
                if(service_request.table_id == null)
                {
                    ViewBag.Error = Global.ServiceRequestTableNullError;
                }
                else if (!ServiceRequestExists(service_request) && ModelState.IsValid)
                {

                    if (om.edit(service_request))
                    {
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        ViewBag.Error = Global.ServiceRequestLockError;
                    }
                }         
                else
                {
                    ViewBag.Error = Global.ServiceRequestDuplicateRequestError;
                }

                //Make list of statuses from enum
                var statuses = from ServiceRequestUtil.ServiceRequestStatus s
                               in Enum.GetValues(typeof(ServiceRequestUtil.ServiceRequestStatus))
                               select new { Value = (int)s, Name = s.ToString() };

                ViewData["status"] = new SelectList(statuses, "Value", "Name", service_request.status);
                ViewBag.table_id = new SelectList(CreateTableList(), "id", "name", service_request.table_id);
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException || e is ArgumentNullException)
                {
                    ViewBag.Error = Global.ServerError;
                }
                else
                {
                    throw e;
                }
            }

            return View(service_request);
        }

        /// <summary>
        /// GET: /ServiceRequest/
        /// </summary>
        [CustomAuthorizationAttribute(Roles = SiteRoles.Admin | SiteRoles.Restaurant | SiteRoles.Developer)]
        public ViewResult Index()
        {
            ServiceRequestVM requestVM = new ServiceRequestVM();
            requestVM.OpenServiceRequests = CreateOpenServiceRequest();
            requestVM.AllServiceRequests = CreateAllServiceRequest();
            return View(requestVM);
        }
        #endregion

        #region Private
        /// <summary>
        /// This method checks if a service request for a given table is already open.
        /// </summary>
        /// <param name="tableID">The table identifier</param>
        /// <returns>True if a request already exists and false if the request does not exist.</returns>
        private bool ServiceRequestExists(service_request request)
        {
            if(request.status != (int)ServiceRequestUtil.ServiceRequestStatus.OPEN)
            {
                return false;
            }

            ICollection<service_request> requests = im.FindByOpenRequestByTable((int)request.table_id);

            if (requests.Count <= 0 || (requests.Count == 1 && requests.ElementAt(0).id == request.id))
            {
                return false;
            }
            return true;
        }

        /// <summary>
        /// Make a list of tables that could be associated to creating a service request depending on
        /// the user.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        private List<table> CreateTableList()
        {
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);
            List<table> tableList = new List<table>();

            if (authUser.user_role == (int)SiteRoles.Restaurant)
            {
                foreach (restaurant_user ru in authUser.restaurant_user)
                {
                    tableList.AddRange(ru.restaurant.tables);
                }
            }
            else if (authUser.user_role == (int)SiteRoles.Admin ||
                    authUser.user_role == (int)SiteRoles.Developer)
            {
                tableList = db.tables.ToList<table>();
            }

            return tableList;
        }

        /// <summary>
        /// Creats a list of sevice requests dependent on the user.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        private List<service_request> CreateAllServiceRequest()
        {
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);

            List<service_request> serviceList = new List<service_request>();

            if (authUser.user_role == (int)SiteRoles.Restaurant)
            {
                List<table> tableList = new List<table>();
                foreach (restaurant_user ru in authUser.restaurant_user)
                {
                    tableList.AddRange(ru.restaurant.tables);
                }

                serviceList = tableList.SelectMany(i => i.service_request)
                                       .OrderByDescending(ru => ru.created)
                                       .OrderBy(ru => ru.status)
                                       .ToList<service_request>();
            }
            else if (authUser.user_role == (int)SiteRoles.Admin ||
                     authUser.user_role == (int)SiteRoles.Developer)
            {
                serviceList = db.service_request.Include(s => s.table)
                                                .OrderByDescending(ru => ru.created)
                                                .ToList<service_request>();
            }

            return serviceList;
        }

        /// <summary>
        /// Creats a list of sevice requests dependent on the user.
        /// </summary>
        /// <exception cref="ArgumentNullException"/>
        private List<service_request> CreateOpenServiceRequest()
        {
            user authUser = Util.User.UserUtil.getAuthenticatedUser(Request);

            List<service_request> serviceList = new List<service_request>();

            if (authUser.user_role == (int)SiteRoles.Restaurant)
            {
                List<table> tableList = new List<table>();
                foreach (restaurant_user ru in authUser.restaurant_user)
                {
                    tableList.AddRange(ru.restaurant.tables);
                }

                serviceList = tableList.SelectMany(i => i.service_request)
                                       .Where(s => s.status == (int)ServiceRequestUtil.ServiceRequestStatus.OPEN)
                                       .OrderBy(ru => ru.created)
                                       .OrderBy(ru => ru.status)
                                       .ToList<service_request>();
            }
            else if (authUser.user_role == (int)SiteRoles.Admin ||
                     authUser.user_role == (int)SiteRoles.Developer)
            {
                serviceList = db.service_request.Include(s => s.table)
                                                .Where(s => s.status == (int)ServiceRequestUtil.ServiceRequestStatus.OPEN)
                                                .OrderBy(ru => ru.created)
                                                .ToList<service_request>();
            }

            return serviceList;
        }
        #endregion
    }
}