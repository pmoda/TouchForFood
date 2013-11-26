using System;
using TouchForFood.Models;
using TouchForFood.Util.ServiceRequest;
using TouchForFood.Util.Security;

namespace TouchForFood.Tests.Classes
{
    public class TestDatabaseHelper
    {
        public touch_for_foodEntities db;
        public TestDatabaseHelper() { }

        #region Bill
        /// <summary>
        /// Adds a bill entry to the database
        /// </summary>
        /// <param name="orderEntity">The associated order</param>
        /// <returns>The created bill entity.</returns>
        internal bill AddBill(order orderEntity)
        {
            //Initialise
            db = new touch_for_foodEntities();
            bill testBill = new bill();

            //Set attributes          
            testBill.order_id = orderEntity.id;
            testBill.is_deleted = false;

            //Save
            db.bills.Add(testBill);
            db.SaveChanges();
            db.Dispose();

            return testBill;
        }

        ///<summary>
        /// Removes a bill from the database.
        /// </summary>
        /// <param name="billEntity">Bill to be removed.</param>
        public void RemoveBill(bill billEntity)
        {
            db = new touch_for_foodEntities();
            if (db.bills.Find(billEntity.id) != null)
            {
                db.bills.Remove(db.bills.Find(billEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Category
        /// <summary>
        /// Adds a category entry to the database
        /// </summary>
        /// <returns>The created category entity.</returns>
        public category AddCategory()
        {
            //Initialise
            db = new touch_for_foodEntities();
            category testCategory = new category();

            //Set attributes          
            testCategory.name = "UnitTest";
            testCategory.version = 0;

            //Save
            db.categories.Add(testCategory);
            db.SaveChanges();
            db.Dispose();

            return testCategory;
        }

        ///<summary>
        /// Removes a bill from the database.
        /// </summary>
        /// <param name="billEntity">Bill to be removed.</param>
        public void RemoveCategory(category categoryEntity)
        {
            db = new touch_for_foodEntities();
            if (db.categories.Find(categoryEntity.id) != null)
            {
                db.categories.Remove(db.categories.Find(categoryEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Friendship
        /// <summary>
        /// Creates a friendship entity
        /// </summary>
        /// <param name="user1">The first user in the friendship</param>
        /// <param name="user2">The second user in the friendship</param>
        /// <returns>the created friendship entity</returns>
        public friendship AddFriendship(user user1, user user2)
        {
            //Initialise
            db = new touch_for_foodEntities();
            friendship testFriendship = new friendship();

            //Set Attributes
            testFriendship.first_user = user1.id;
            testFriendship.second_user = user2.id;

            //Save
            db.friendships.Add(testFriendship);
            db.SaveChanges();
            db.Dispose();

            return testFriendship;
        }

        /// <summary>
        /// Remove friendship item from database.
        /// </summary>
        /// <param name="menuEntity">Friendships item to be removed.</param>
        public void RemoveFriendship(friendship friendshipEntity)
        {
            db = new touch_for_foodEntities();
            if (db.friendships.Find(friendshipEntity.id) != null)
            {
                db.friendships.Remove(db.friendships.Find(friendshipEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Item
        /// <summary>
        /// Creates an entry of type item in the database.
        /// </summary>
        /// <returns>The created item entry.</returns>
        public item AddItem()
        {
            //Initialise
            db = new touch_for_foodEntities();
            item testItem = new item();

            //Set Attributes
            testItem.name = "UnitTest";

            //Save
            db.items.Add(testItem);
            db.SaveChanges();
            db.Dispose();

            return testItem;
        }

        /// <summary>
        /// Creates an entry of type item in the database.
        /// </summary>
        /// <param name="catagoryEntity">Category that item is related to.</param>
        /// <returns>The created item entry.</returns>
        public item AddItem(category catagoryEntity)
        {
            //Initialise
            db = new touch_for_foodEntities();
            item testItem = new item();

            //Set Attributes
            testItem.name = "UnitTest";
            testItem.category_id = catagoryEntity.id;

            //Save
            db.items.Add(testItem);
            db.SaveChanges();
            db.Dispose();

            return testItem;
        }

        /// <summary>
        /// Remove item item from database.
        /// </summary>
        /// <param name="menuEntity">Item item to be removed.</param>
        public void RemoveItem(item itemEntity)
        {
            db = new touch_for_foodEntities();
            if (db.items.Find(itemEntity.id) != null)
            {
                db.items.Remove(db.items.Find(itemEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Menu
        /// <summary>
        /// Creates an entry of type menu in the database.
        /// </summary>
        /// <param name="restaurantEntity">The restaurant the menu belongs to</param>
        /// <returns>The created menu entry.</returns>
        public menu AddMenu(restaurant restaurantEntity)
        {
            //Initialise
            db = new touch_for_foodEntities();
            menu testMenu = new menu();

            //Set Attributes
            testMenu.resto_id = restaurantEntity.id;
            testMenu.name = "UnitTest";
            testMenu.is_active = false;
            testMenu.is_deleted = false;

            //Save
            db.menus.Add(testMenu);
            db.SaveChanges();
            db.Dispose();

            return testMenu;
        }

        /// <summary>
        /// Remove menu item from database.
        /// </summary>
        /// <param name="menuEntity">Menu item to be removed.</param>
        public void RemoveMenu(menu menuEntity)
        {
            db = new touch_for_foodEntities();
            if (menuEntity != null)
            {
                db.menus.Remove(db.menus.Find(menuEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Menu Category
        /// <summary>
        /// Creates an entry of type menu category in the database.
        /// </summary>
        /// <param name="categoryEntity">The category the menu category is an extension of</param>
        /// <param name="menuEntity">The menu the category belongs to</param>
        /// <returns>The created menu category</returns>
        public menu_category AddMenuCategory(category categoryEntity, menu menuEntity)
        {
            //Initialise
            db = new touch_for_foodEntities();
            menu_category testMenuCategory = new menu_category();

            //Set Attributes
            testMenuCategory.category_id = categoryEntity.id;
            testMenuCategory.menu_id = menuEntity.id;
            testMenuCategory.is_active = false;

            //Save
            db.menu_category.Add(testMenuCategory);
            db.SaveChanges();
            db.Dispose();

            return testMenuCategory;
        }

        /// <summary>
        /// Remove menu item from database.
        /// </summary>
        /// <param name="menuCategoryEntity">Menu Category item to be removed.</param>
        public void RemoveMenuCategory(menu_category menuCategoryEntity)
        {
            db = new touch_for_foodEntities();
            if (db.menu_category.Find(menuCategoryEntity.id) != null)
            {
                db.menu_category.Remove(db.menu_category.Find(menuCategoryEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region MenuItem
        /// <summary>
        /// Creates an entry of type menu itemin the database.
        /// </summary>
        /// <param name="itemEntity">The item the menu item represents.</param>
        /// <param name="menuCategory">Category the menu item belongs to</param>
        /// <returns>The created menu item entry.</returns>
        public menu_item AddMenuItem(item itemEntity, menu_category menuCategory)
        {
            //Initialise
            db = new touch_for_foodEntities();
            menu_item testMenuItem = new menu_item();

            //Set Attributes
            testMenuItem.item_id = itemEntity.id;
            testMenuItem.menu_category_id = menuCategory.id;
            testMenuItem.price = new decimal(10.99);
            testMenuItem.is_active = false;

            //Save
            db.menu_item.Add(testMenuItem);
            db.SaveChanges();
            db.Dispose();

            return testMenuItem;
        }

        /// <summary>
        /// Remove menu item item from database.
        /// </summary>
        /// <param name="menuEntity">Menu item item to be removed.</param>
        public void RemoveMenuItem(menu_item menuItemEntity)
        {
            db = new touch_for_foodEntities();
            if (db.menu_item.Find(menuItemEntity.id) != null)
            {
                db.menu_item.Remove(db.menu_item.Find(menuItemEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Order
        /// <summary>
        /// Creates an entry of type order in the database.
        /// </summary>
        /// <param name="orderTable">The table where orders are made from.</param>
        /// <returns>The created order entry.</returns>
        public order AddOrder(table orderTable)
        {
            //Initialise
            db = new touch_for_foodEntities();
            order testOrder = new order();

            //Set Attributes
            testOrder.timestamp = DateTime.Now;
            testOrder.table_id = orderTable.id;
            testOrder.version = 1;

            //Save
            db.orders.Add(testOrder);
            db.SaveChanges();
            db.Dispose();

            return testOrder;
        }

        /// <summary>
        /// Creates an entry of type order in the database.
        /// </summary>
        /// <param name="orderTable">The table where orders are made from.</param>
        /// <param name="orderWaiter">The waiter in charge of order.</param>
        /// <returns>The created order entry.</returns>
        public order AddOrder(table orderTable, waiter orderWaiter)
        {
            //Initialise
            db = new touch_for_foodEntities();
            order testOrder = new order();

            //Set Attributes
            testOrder.timestamp = DateTime.Now;
            testOrder.table_id = orderTable.id;
            testOrder.waiter_id = orderWaiter.id;
            testOrder.version = 1;

            //Save
            db.orders.Add(testOrder);
            db.SaveChanges();
            db.Dispose();

            return testOrder;
        }

        /// <summary>
        /// Removes an order from the database.
        /// </summary>
        /// <param name="orderEntity">Order to be removed.</param>
        public void RemoveOrder(order orderEntity)
        {
            db = new touch_for_foodEntities();
            if (orderEntity != null)
            {
                db.orders.Remove(db.orders.Find(orderEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region OrderItem
        /// <summary>
        /// Creates an entry of type order item in the database.
        /// </summary>
        /// <param name="orderEntity">Order that order items belong to.</param>
        /// <returns>The created order item entity.</returns>
        public order_item AddOrderItem(order orderEntity, menu_item menuItem)
        {
            //Initialise
            db = new touch_for_foodEntities();
            order_item testOrderItem = new order_item();

            //Set Attributes
            testOrderItem.order_id = orderEntity.id;
            testOrderItem.version = 1;
            testOrderItem.menu_item_id = menuItem.id;

            //Save
            db.order_item.Add(testOrderItem);
            db.SaveChanges();
            db.Dispose();

            return testOrderItem;
        }

        /// <summary>
        /// Creates an entry of type order item in the database.
        /// </summary>
        /// <param name="orderEntity">Order that order items belongs to.</param>
        /// <param name="billEntity">Bill that order item belongs to.</param>
        /// <returns>The created order item entity.</returns>
        public order_item AddOrderItem(order orderEntity, bill billEntity, menu_item menuItem)
        {
            //Initialise
            db = new touch_for_foodEntities();
            order_item testOrderItem = new order_item();

            //Set Attributes
            testOrderItem.order_id = orderEntity.id;
            testOrderItem.bill_id = billEntity.id;
            testOrderItem.menu_item_id = menuItem.id;
            testOrderItem.version = 1;

            //Save
            db.order_item.Add(testOrderItem);
            db.SaveChanges();
            db.Dispose();

            return testOrderItem;
        }

        /// <summary>
        /// Removes an order item from the database.
        /// </summary>
        /// <param name="orderEntity">Order item to be removed.</param>
        public void RemoveOrderItem(order_item orderItem)
        {
            db = new touch_for_foodEntities();
            if(db.order_item.Find(orderItem.id) != null)
            {
                db.order_item.Remove(db.order_item.Find(orderItem.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Restaurant
        /// <summary>
        /// Creates an entry of type restaurant in the database.
        /// </summary>
        /// <returns>The created restaurant entity.</returns>
        public restaurant AddRestaurant()
        {
            //Initialise
            db = new touch_for_foodEntities();
            restaurant testRestaurant = new restaurant();

            //Set Attributes
            testRestaurant.name = "UnitTest";
            testRestaurant.address = "UnitTest";
            testRestaurant.city = "UnitTest";
            testRestaurant.version = 1;
            testRestaurant.is_deleted = false;

            //Save
            db.restaurants.Add(testRestaurant);
            db.SaveChanges();
            db.Dispose();

            return testRestaurant;
        }

        /// <summary>
        /// Removes a restaurant item from the database.
        /// </summary>
        /// <param name="restaurantEntity">Restaurant item to be removed.</param>
        public void RemoveRestaurant(restaurant restaurantEntity)
        {
            db = new touch_for_foodEntities();
            if (db.restaurants.Find(restaurantEntity.id) != null)
            {
                db.restaurants.Remove(db.restaurants.Find(restaurantEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Restaurant User
        /// <summary>
        /// Creates an entry of type restaurant_user in the database.
        /// </summary>
        /// <param name="testUser">User user_restaurant is associated to.</param>
        /// <param name="testRestaurant">Restaurant user_restaurant is associated to.</param>
        /// <returns>The created restaurant_user entity.</returns>
        public restaurant_user AddRestaurantUser(user testUser, restaurant testRestaurant)
        {
            //Initialise
            db = new touch_for_foodEntities();
            restaurant_user testRestaurantUser = new restaurant_user();

            //Set Attributes
            testRestaurantUser.user_id= testUser.id;
            testRestaurantUser.restaurant_id = testRestaurant.id;

            //Save
            db.restaurant_user.Add(testRestaurantUser);
            db.SaveChanges();
            db.Dispose();

            return testRestaurantUser;
        }

        /// <summary>
        /// Removes a restaurant_user item from the database.
        /// </summary>
        /// <param name="restaurantEntity">restaurant_user item to be removed.</param>
        public void RemoveRestaurantUser(restaurant_user restaurantUserEntity)
        {
            db = new touch_for_foodEntities();
            if (db.restaurant_user.Find(restaurantUserEntity.id) != null)
            {
                db.restaurant_user.Remove(db.restaurant_user.Find(restaurantUserEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Review
        /// <summary>
        /// Creates an entry of type review in the database.
        public review AddReview(restaurant r, order o, user u)
        {
            //Initialise
            db = new touch_for_foodEntities();
            review testReview = new review();

            //Set attributes
            testReview.is_anonymous = false;
            testReview.order_id = o.id;
            testReview.restaurant_id = r.id;
            testReview.user_id = u.id;
            testReview.rating = 0;

            //Save
            db.reviews.Add(testReview);
            db.SaveChanges();
            db.Dispose();

            return testReview;
        }

        /// <summary>
        /// Removes a review item from the database.
        /// </summary>
        /// <param name="userEntity">The review to be removed.</param>
        public void RemoveReview(review reviewEntity)
        {

            db = new touch_for_foodEntities();
            if (db.reviews.Find(reviewEntity.id) != null)
            {
                db.reviews.Remove(db.reviews.Find(reviewEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Review Order Item
        public review_order_item AddReviewOrderItem(review r, order_item oi, string text, int rating)
        {
            //Initialise
            db = new touch_for_foodEntities();
            review_order_item testReviewOI = new review_order_item();

            //Set attributes
            testReviewOI.rating = rating;
            testReviewOI.order_item_id = oi.id;
            testReviewOI.review_id = r.id;
            testReviewOI.submitted_on = System.DateTime.Now;
            testReviewOI.rating = 0;

            //Save
            db.review_order_item.Add(testReviewOI);
            db.SaveChanges();
            db.Dispose();

            return testReviewOI;

        }
        public void RemoveReviewOrderItem(review_order_item reviewEntityOI)
        {

            db = new touch_for_foodEntities();
            if (db.review_order_item.Find(reviewEntityOI.id) != null)
            {
                db.review_order_item.Remove(db.review_order_item.Find(reviewEntityOI.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Table
        /// <summary>
        /// Creates an entry of type table in the database.
        /// </summary>
        /// <param name="restaurantEntity">The restaurant where the table is located.</param>
        /// <returns>The created table entity.</returns>
        public table AddTable(restaurant restaurantEntity)
        {
            //Initialise
            db = new touch_for_foodEntities();
            table testTable = new table();

            //Set Attributes
            testTable.name = "Unit Test";
            testTable.restaurant_id = restaurantEntity.id;

            //Save
            db.tables.Add(testTable);
            db.SaveChanges();
            db.Dispose();

            return testTable;
        }

        /// <summary>
        /// Removes a table item from the database.
        /// </summary>
        /// <param name="tableEntity">The table to be removed.</param>
        public void RemoveTable(table tableEntity)
        {
            db = new touch_for_foodEntities();
            if (db.tables.Find(tableEntity.id) != null)
            {
                db.tables.Remove(db.tables.Find(tableEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Service Request
        /// <summary>
        /// Creates an entry of type service request in the database.
        /// </summary>
        /// <param name="tableEntity">the table the service request is associated to.</param>
        /// <returns>The created service request entity.</returns>
        public service_request AddServiceRequest(table tableEntity)
        {
            //Initialise
            db = new touch_for_foodEntities();
            service_request testServiceRequest = new service_request();

            //Set Attributes
            testServiceRequest = new service_request();
            testServiceRequest.note = "UnitTest";
            testServiceRequest.created = DateTime.Now;
            testServiceRequest.status = (int)ServiceRequestUtil.ServiceRequestStatus.OPEN;
            testServiceRequest.version = 0;
            testServiceRequest.table_id = tableEntity.id;

            //Save
            db.service_request.Add(testServiceRequest);
            db.SaveChanges();
            db.Dispose();

            return testServiceRequest;
        }

        /// <summary>
        /// Removes a service request item from the database.
        /// </summary>
        /// <param name="serviceRequest">The service request to be removed.</param>
        public void RemoveServiceRequest(service_request serviceRequest)
        {
            db = new touch_for_foodEntities();
            if (db.service_request.Find(serviceRequest.id) != null)
            {
                db.service_request.Remove(db.service_request.Find(serviceRequest.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Side
        /// <summary>
        /// Creates an entry of type side in the database.
        /// </summary>
        /// <param name="menuCategory">The menu category associated to the side</param>
        /// <returns>The created side entity.</returns>
        public side AddSide(menu_category menuCategory)
        {
            //Initialise
            db = new touch_for_foodEntities();
            side testSide = new side();

            //Set Attributes
            testSide.name = "UnitTest";
            testSide.price = new decimal(2.99);
            testSide.version = 1;
            testSide.menu_category_id = menuCategory.id;
            testSide.is_active = false;
            testSide.is_deleted = false;

            //Save
            db.sides.Add(testSide);
            db.SaveChanges();
            db.Dispose();

            return testSide;
        }

        /// <summary>
        /// Removes a side item from the database.
        /// </summary>
        /// <param name="testSide">The side to be removed.</param>
        public void RemoveSide(side testSide)
        {
            db = new touch_for_foodEntities();
            if (db.sides.Find(testSide.id) != null)
            {
                db.sides.Remove(db.sides.Find(testSide.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region User
        /// <summary>
        /// Creates an entry of type user in the database.
        /// </summary>
        /// <param name="username">A unique string to represent the user.</param>
        /// <param name="currentTable">A table in the restaurant.</param>
        /// <param name="userRole">The role of the user (ex: Administrator, Client)</param>
        /// <returns>The created user entity.</returns>
        public user AddUser(string email, table currentTable, int userRole)
        {
            //Initialise
            db = new touch_for_foodEntities();
            user testUser = new user();

            //Set attributes
            testUser.username = email;

            // Make sure the password is encrypted
            AES aes = new AES();
            testUser.password = aes.EncryptToString(email);
            testUser.ConfirmPassword = aes.EncryptToString(email);

            testUser.first_name = email;
            testUser.last_name = email;
            testUser.email = email;
            testUser.image_url = email;
            testUser.current_table_id = currentTable.id;
            testUser.version = 1;
            testUser.user_role = userRole;

            //Save
            db.users.Add(testUser);
            db.SaveChanges();
            db.Dispose();

            return testUser;
        }

        /// <summary>
        /// Removes a user item from the database.
        /// </summary>
        /// <param name="userEntity">The user to be removed.</param>
        public void RemoveUser(user userEntity)
        {
            db = new touch_for_foodEntities();
            if (db.users.Find(userEntity.id) != null)
            {
                db.users.Remove(db.users.Find(userEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion

        #region Waiter
        /// <summary>
        /// Creates a waiter entity
        /// </summary>
        /// <param name="restaurantEntity">Associated restaurant</param>
        /// <returns>Created restaurant entity</returns>
        public waiter AddWaiter(restaurant restaurantEntity)
        {
            //Initialise
            db = new touch_for_foodEntities();
            waiter testWaiter = new waiter();

            //Set attributes
            testWaiter.first_name = "UnitTest";
            testWaiter.last_name = "UnitTest";
            testWaiter.version = 1;
            testWaiter.resto_id = restaurantEntity.id;

            //Save
            db.waiters.Add(testWaiter);
            db.SaveChanges();
            db.Dispose();

            return testWaiter;
        }

        /// <summary>
        /// Removes a waiter item from the database.
        /// </summary>
        /// <param name="waiterEntity">The waiter to be removed</param>
        public void RemoveWaiter(waiter waiterEntity)
        {
            db = new touch_for_foodEntities();
            if (db.waiters.Find(waiterEntity.id) != null)
            {
                db.waiters.Remove(db.waiters.Find(waiterEntity.id));
                db.SaveChanges();
            }
            db.Dispose();
        }
        #endregion
    }
}
