using TouchForFood.Models;
using System.Linq;
using System;
using TouchForFood.Exceptions;
using System.Data;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;

namespace TouchForFood.Mappers.Abstract
{
    public abstract class GenericIM
    {
        protected touch_for_foodEntities db;

        protected GenericIM()
        {
            db = new touch_for_foodEntities();
        }

        protected GenericIM(touch_for_foodEntities db)
        {
            this.db = db;
        }
    }
}