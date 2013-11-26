using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TouchForFood.Models;
using System.Web.Mvc;
using System.Web.Security;
using System.Web;

namespace TouchForFood.Tests.Classes
{
    class Session
    {
        touch_for_foodEntities db;
        Controller controller;
        ContextMocks mock;
        public Session(touch_for_foodEntities database, Controller target)
        {
            db = database;
            controller = target;
            ContextMocks mock = new ContextMocks(controller);
        }

        public void simulateLogin(string username, string password)
        {
            user user = db.users.FirstOrDefault(m => m.username.Equals(username, StringComparison.Ordinal) && m.password.Equals(password, StringComparison.Ordinal));

            if (user != null)
            {
                FormsAuthenticationTicket ticket = new FormsAuthenticationTicket(1, user.username,
                                                                                DateTime.Now, DateTime.Now.AddDays(1),
                                                                                false, user.id.ToString(), FormsAuthentication.FormsCookiePath);
                // Encrypt the ticket
                string hashedTicket = FormsAuthentication.Encrypt(ticket);

                // Create the new cookie and add it into the response
                controller.Request.Cookies.Add(new HttpCookie(FormsAuthentication.FormsCookieName, hashedTicket));
            }

        }

        public void addContoller(Controller target)
        {
            mock.addController(target);
        }
    }
}
