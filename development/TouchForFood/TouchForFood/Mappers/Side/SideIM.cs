using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Models;

namespace TouchForFood.Mappers
{
    public class SideIM : GenericIM
    {
        public SideIM(): base() {}
        public SideIM(touch_for_foodEntities db) : base(db) { }

        public side find(int id)
        {
            try
            {
                return db.sides.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        public ICollection<side> find(bool is_active, bool is_deleted)
        {
            try
            {
                return db.sides.Where(s => s.is_active == is_active && s.is_deleted == is_deleted).ToList();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }
    }
}