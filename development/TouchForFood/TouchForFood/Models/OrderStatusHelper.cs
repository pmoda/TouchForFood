using System.Collections.Generic;
using System.Web.Mvc;

namespace TouchForFood.Models
{
    public static class OrderStatusHelper
    {
        private static List<SelectListItem> states;

        public enum OrderStatusEnum
        {
            OPEN = 0,
            CLOSED,
            EDITING,
            PLACED,
            ERROR,
            COMPLETE,
            CANCELED,
            DELETED,
            PROCESSING,
            LAST_STATUS
        };

        public enum OrderItemStatusEnum
        {
            PENDING = 0,
            REJECTED,
            ERROR,
            PLACED,
            PROCESSING,            
            DELIVERED,
            EDITING,
            DELETED,
            CANCELED,
            PAID,
            LAST_STATUS
        };

        public static List<SelectListItem> GetAvailableItemStatuses()
        {
            if (states == null) return states;

            states = new List<SelectListItem>();

            const int max = (int)OrderItemStatusEnum.LAST_STATUS;
            for (int i = 0; i < max; i++)
            {
                SelectListItem temp = new SelectListItem();
                temp.Text = System.Enum.GetName(typeof(OrderItemStatusEnum), (OrderItemStatusEnum)i);
                states.Add(temp);
            }
            states[0].Selected = true;
            return states;
        }

        public static string GetOrderStatusDescription(OrderStatusEnum ose)
        {
            string message="";
            switch (ose)
            {
                case OrderStatusEnum.OPEN:
                    message= "The order is open.";
                    break;
                case OrderStatusEnum.CLOSED:
                    message= "The order is closed.";
                    break;
                case OrderStatusEnum.EDITING:
                    message = "The order is being edited.";
                    break;
                case OrderStatusEnum.PLACED:
                    message = "The order has been placed.";
                    break;
                case OrderStatusEnum.ERROR:
                    message= "There was an error with your order, see error message.";
                    break;
                case OrderStatusEnum.COMPLETE:
                    message= "The order is complete.";
                    break;
                case OrderStatusEnum.CANCELED:
                    message = "The order is canceled.";
                    break;
                case OrderStatusEnum.PROCESSING:
                    message = "The order is processing.";
                    break;
            }
            return message;
        }

        public static string GetOrderStatusName(int? ose)
        {
            if(ose == null) return "";
            return System.Enum.GetName(typeof(OrderStatusEnum), ose);
            /*
            string message = "";
            switch (ose)
            {
                case 1:
                    message = "Open";
                    break;
                case 2:
                    message = "Closed";
                    break;
                case 3:
                    message = "Editing";
                    break;
                case 4:
                    message = "Placed";
                    break;
                case 5:
                    message = "Error";
                    break;
                case 6:
                    message = "Complete";
                    break;
                case 7:
                    message = "Canceled";
                    break;
            }
            return message;
            */
        }

        public static string GetOrderItemStatusDescription(OrderItemStatusEnum ose)
        {            
            switch (ose)
            {
                case OrderItemStatusEnum.PENDING:
                    return "The Order is in progress and not placed yet.";
                case OrderItemStatusEnum.REJECTED:
                    // TODO: What errors?
                    return "The Order was rejected, see error message.";
                case OrderItemStatusEnum.ERROR:
                    // TODO: What errors?
                    return "There was an error with your order, see error message.";
                case OrderItemStatusEnum.PLACED:
                    return "The order has been placed.";
                case OrderItemStatusEnum.PROCESSING:
                    return "The order is being processed.";
                case OrderItemStatusEnum.EDITING:
                    return "The order is being edited.";
                case OrderItemStatusEnum.DELIVERED:
                    return "The order was successfully delivered.";
                case OrderItemStatusEnum.CANCELED:
                    return "The order is canceled.";
                case OrderItemStatusEnum.PAID:
                    return "The order item is paid.";
            }
            return "";
        }
        public static string GetOrderItemStatusName(int? ose)
        {
            if (ose == null) return "";
            return System.Enum.GetName(typeof(OrderItemStatusEnum), ose);
            /*
            switch (ose)
            {
                case 0:
                    return "Pending";
                case 1:
                    // TODO: What errors?
                    return "Rejected";
                case 2:
                    // TODO: What errors?
                    return "Error";
                case 3:
                    return "Placed";
                case 4:
                    return "Processing";
                case 5:
                    return "Delivered";
                case 6:
                    return "Edited.";
                case 7:
                    return "Canceled";
            }
            return "";
            */
        }
    };
};