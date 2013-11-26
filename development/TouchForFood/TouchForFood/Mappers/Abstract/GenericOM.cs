using TouchForFood.Models;
using System.Linq;
using System;
using TouchForFood.Exceptions;
using System.Data;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;


namespace TouchForFood.Mappers.Abstract
{
    public abstract class GenericOM
    {
        protected touch_for_foodEntities db;
        protected GenericOM() 
        { 
            db = new touch_for_foodEntities();
        }

        protected GenericOM(touch_for_foodEntities db)
        {
            this.db = db;
        }
        
        public abstract int delete(int id);
    }
}