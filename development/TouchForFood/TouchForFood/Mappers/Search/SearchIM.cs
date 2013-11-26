using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Models;

namespace TouchForFood.Mappers.Search
{
    public class SearchIM : GenericIM
    {
        public SearchIM() : base() { }
        public SearchIM(touch_for_foodEntities db) : base(db) { }

        public int findByUser(int userId)
        {
            try
            {
                var restoUserList = db.restaurant_user.Where(m => m.user_id == userId).ToList();

                foreach(restaurant_user ru in restoUserList)
                {
                    if (ru.restaurant_id.HasValue)
                    {
                        return (int)ru.restaurant_id;
                    }
                }

                return -1;
            }
            catch (InvalidOperationException)
            {
                return -1;
            }
        }
    }
}