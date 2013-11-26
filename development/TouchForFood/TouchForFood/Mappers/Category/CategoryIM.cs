using TouchForFood.Models;
using System.Linq;
using System;
using TouchForFood.Exceptions;
using System.Data;
using System.Collections.Generic;
using TouchForFood.Mappers.Abstract;
namespace TouchForFood.Mappers
{
    public class CategoryIM : GenericIM
    {

        public CategoryIM() : base(){}

        public CategoryIM(touch_for_foodEntities db): base(db){}

        public category find(int id)
        {
            try
            {
                return db.categories.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<category> find()
        {
            try
            {
                return db.categories.ToList();
            }
            catch (System.ArgumentNullException)
            {
                return null;
            }
        }
    }
}