using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects;
using TouchForFood.Models;
using TouchForFood.Mappers.Abstract;
using TouchForFood.ViewModels;

namespace TouchForFood.Mappers
{
    public class ReportsIM : GenericIM
    {
        public ReportsIM() : base() { }
        public ReportsIM(touch_for_foodEntities db):base(db){}

        public IEnumerable<MostPopularDishViewModel> findMostPopular(restaurant r)
        {
            var sql = "select r.name as restoName, i.name as menuItemName, COUNT(oi.id) AS timesOrdered, mi.id as menuItemId"
              + " from dbo.order_item oi"
              + " JOIN dbo.menu_item mi on oi.menu_item_id = mi.id"
              + " JOIN dbo.item i on mi.item_id = i.id"
              + " JOIN dbo.menu_category mc on mi.menu_category_id = mc.id"
              + " JOIN dbo.menu m on mc.menu_id = m.id"
              + " JOIN dbo.restaurant r on m.resto_id = r.id"
              + " WHERE r.id = " + r.id             
              + " Group by i.name, r.name, mi.id"
              + " Order by timesOrdered desc";
            
            IEnumerable<MostPopularDishViewModel> dishes = db.Database.SqlQuery<MostPopularDishViewModel>(sql);
            return dishes;
            
        }

        public IEnumerable<MostPopularDishViewModel> findMostPopularCustomer(restaurant r)
        {
            var sql = "select r.name as restoName, i.name as menuItemName, COUNT(oi.id) AS timesOrdered, mi.id as menuItemId"
              + " from dbo.order_item oi"
              + " JOIN dbo.menu_item mi on oi.menu_item_id = mi.id"
              + " JOIN dbo.item i on mi.item_id = i.id"
              + " JOIN dbo.menu_category mc on mi.menu_category_id = mc.id"
              + " JOIN dbo.menu m on mc.menu_id = m.id"
              + " JOIN dbo.restaurant r on m.resto_id = r.id"
              + " WHERE r.id = " + r.id
              + " AND mi.is_active = 1 "
              + " AND mi.is_deleted = 0 "
              + " Group by i.name, r.name, mi.id"
              + " Order by timesOrdered desc";

            IEnumerable<MostPopularDishViewModel> dishes = db.Database.SqlQuery<MostPopularDishViewModel>(sql);
            return dishes;

        }

        public IEnumerable<WaiterStatsViewModel> findWaiterStats(restaurant r, System.DateTime start, System.DateTime end)
        {
            String s = start.ToString("yyyy-MM-dd HH:mm:ss.fff");
            String e = end.ToString("yyyy-MM-dd HH:mm:ss.fff");
            
            var sql = " select r.id as restoId, r.name as restoName, w.id as waiterId, w.first_name as waiterFirstName, w.last_name as waiterLastName, COUNT(o.id) as completedOrders"
            + " from dbo.[order] o"
            + " Join dbo.waiter w on o.waiter_id = w.id"
            + " Join dbo.restaurant r on w.resto_id = r.id"
            + " WHERE o.order_status = " + (int)OrderStatusHelper.OrderItemStatusEnum.DELIVERED
            + " and r.id = " + r.id
            + " and o.timestamp >= '" + s + "'"
            + " and o.timestamp <= '" + e + "'"
            + " Group By w.first_name, w.last_name, r.id, r.name, w.id"
            + " order by completedOrders desc";

            System.Diagnostics.Debug.WriteLine(sql);
            IEnumerable<WaiterStatsViewModel> waiters = db.Database.SqlQuery<WaiterStatsViewModel>(sql);
            
            foreach(WaiterStatsViewModel waiter in waiters){
                System.Diagnostics.Debug.WriteLine(waiter.waiterFirstName + " made "+waiter.completedOrders);
            }
             
            return waiters;

        }
       
    }
}