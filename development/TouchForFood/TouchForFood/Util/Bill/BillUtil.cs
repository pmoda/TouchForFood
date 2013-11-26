using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using TouchForFood.Models;

namespace TouchForFood.Util.Bill
{
    public static class BillUtil
    {
        public static decimal GetTPS()
        {
            // TODO: Should this be in the db?
            const decimal TPS_VALUE = 0.05M;
            return TPS_VALUE;
        }

        public static decimal GetTVQ()
        {
            // TODO: Should this be in the db?
            const decimal TVQ_VALUE = 0.09975M;
            return TVQ_VALUE;
        }

        public static void Update(ref bill theBill)
        {
            theBill.total = 0;
            theBill.tps = 0;
            theBill.tvq = 0;
            foreach (order_item oi in theBill.order_item)
            {
                theBill.total += oi.menu_item.price;
            }
            theBill.tps = theBill.total * GetTPS();
            theBill.tvq = (theBill.total + theBill.tps) * GetTVQ();
        }

        public static decimal GetTotalBeforeTax(bill theBill)
        {
            return (decimal)theBill.total;
        }

        public static decimal GetTotalAfterTax(bill theBill)
        {
            return (decimal)(theBill.total + theBill.tvq + theBill.tps);
        }
        public static bool CheckFullyPaid(bill theBill)
        {
            bool fullyPaid = true;
            foreach (order_item oi in theBill.order.order_item)
            {
                if (oi.order_item_status != (int)OrderStatusHelper.OrderItemStatusEnum.PAID)
                {
                    fullyPaid = false;
                    break;
                }
            }
            return fullyPaid;
        }

        public static bool CheckProcessing(order order)
        {
            bool processing = true;
            foreach (order_item oi in order.order_item)
            {
                if (oi.order_item_status == (int)OrderStatusHelper.OrderItemStatusEnum.PLACED ||
                    oi.order_item_status == (int)OrderStatusHelper.OrderItemStatusEnum.PENDING)
                {
                    processing = false;
                    break;
                }
            }
            return processing;
        }

        public static bool CheckItemsRemaining(order order)
        {
            foreach (order_item oi in order.order_item)
            {
                if ((oi.bill_id == null || oi.bill_id <= 0) && oi.bill == null)
                {
                    return true;
                }
            }
            return false;
        }
    }
}