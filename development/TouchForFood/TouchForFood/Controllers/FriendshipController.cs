using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.App_GlobalResources;

namespace TouchForFood.Controllers
{ 
    public class FriendshipController : Controller
    {
        private touch_for_foodEntities db = new touch_for_foodEntities();

        //
        // GET: /Friendship/
        public ViewResult Index()
        {
            var friendships = db.friendships.Include(f => f.user).Include(f => f.user1);
            return View(friendships.ToList());
        }

        //
        // GET: /Friendship/Details/5
        public ViewResult Details(long id)
        {
            friendship friendship = db.friendships.Find(id);
            return View(friendship);
        }

        //
        // GET: /Friendship/Create
        public ActionResult Create()
        {
            ViewBag.first_user = new SelectList(db.users, "id", "username");
            ViewBag.second_user = new SelectList(db.users, "id", "username");
            return View();
        } 

        //
        // POST: /Friendship/Create
        [HttpPost]
        public ActionResult Create(friendship friendship)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.friendships.Add(friendship);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception) { }
            }

            ViewBag.Error = Global.Error_Title;
            ViewBag.first_user = new SelectList(db.users, "id", "username", friendship.first_user);
            ViewBag.second_user = new SelectList(db.users, "id", "username", friendship.second_user);
            return View("Create");
        }
        
        //
        // GET: /Friendship/Edit/5
        public ActionResult Edit(long id)
        {
            friendship friendship = db.friendships.Find(id);
            ViewBag.first_user = new SelectList(db.users, "id", "username", friendship.first_user);
            ViewBag.second_user = new SelectList(db.users, "id", "username", friendship.second_user);
            return View(friendship);
        }

        //
        // POST: /Friendship/Edit/5
        [HttpPost]
        public ActionResult Edit(friendship friendship)
        {
            if (ModelState.IsValid)
            {
                db.Entry(friendship).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Error = Global.Error_Title;
            ViewBag.first_user = new SelectList(db.users, "id", "username", friendship.first_user);
            ViewBag.second_user = new SelectList(db.users, "id", "username", friendship.second_user);
            return View(friendship);
        }

        //
        // GET: /Friendship/Delete/5
        public ActionResult Delete(long id)
        {
            friendship friendship = db.friendships.Find(id);
            return View(friendship);
        }

        //
        // POST: /Friendship/Delete/5
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {            
            friendship friendship = db.friendships.Find(id);
            db.friendships.Remove(friendship);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}