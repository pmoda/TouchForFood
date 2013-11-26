using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;
using System.Web.Mvc;

namespace TouchForFood.Util.Table
{
    public static class TableUtil
    {
        public static List<restaurant> GetRestaurants(HttpRequestBase request)
        {
            user user = Util.User.UserUtil.getAuthenticatedUser(request);
            List<restaurant> restos = new List<restaurant>();
            if(user == null){
                return restos;
            }

            foreach (restaurant_user re in user.restaurant_user)
            {
                restos.Add(re.restaurant);
            }
            return restos;
        }
    }
}