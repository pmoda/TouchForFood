using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Controllers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;
using TouchForFood.Util.Security;

namespace TouchForFood.Tests
{    
    /// <summary>
    ///This is a test class for RestaurantControllerTest and is intended
    ///to contain all RestaurantControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RestaurantControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static RestaurantController target;
        private static restaurant restaurant1;
        private static restaurant restaurant2;
        private static table table1;
        private static user user1;
        private static restaurant_user restaurantUser1;
        private static menu menu1;
        private static order order1;
        private static review review1;
        #endregion

        #region Properties
        /// <summary>
        ///Gets or sets the test context which provides information about and functionality for the current test run.
        ///</summary>
        private TestContext testContextInstance;
        public TestContext TestContext
        {
            get { return testContextInstance; }
            set { testContextInstance = value; }
        }
        #endregion

        #region Test Attributes
        /// <summary>
        /// Use ClassInitialize to run code before running the first test in the class
        /// </summary>
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            testDatabase = new TestDatabaseHelper();
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup(){}

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //Add test data (order specific)
            restaurant1 = testDatabase.AddRestaurant();
            restaurant2 = new restaurant();
            table1 = testDatabase.AddTable(restaurant1);
            user1 = testDatabase.AddUser("restaurantUnitTest@email.com", table1, (int)SiteRoles.Admin);
            restaurantUser1 = testDatabase.AddRestaurantUser(user1, restaurant1);
            menu1 = testDatabase.AddMenu(restaurant1);
            order1 = testDatabase.AddOrder(table1);
            review1 = testDatabase.AddReview(restaurant1, order1, user1);

            //Session
            db = new touch_for_foodEntities();
            target = new RestaurantController();
            Session session = new Session(db, target);
            session.simulateLogin(user1.username, user1.password);
        }

        /// <summary>
        ///Use TestInitialize to run code before running each test 
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveReview(review1);
            testDatabase.RemoveOrder(order1);
            testDatabase.RemoveMenu(menu1);
            foreach (restaurant_user ru in db.restaurant_user)
            {
                testDatabase.RemoveRestaurantUser(ru);
            }
            testDatabase.RemoveUser(user1);
            testDatabase.RemoveTable(table1);
            testDatabase.RemoveRestaurant(restaurant1);
            testDatabase.RemoveRestaurant(restaurant2);
        }
        #endregion

        #region Test Methods
        /// <summary>
        /// Must return the details of a restuarant
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            int expected = restaurant1.id;

            //Act
            ViewResult actual = target.Details(restaurant1.id);

            //Assert
            Assert.AreEqual(expected, ((restaurant)actual.ViewData.Model).id);
        }

        /// <summary>
        /// Must create a restaurant and restaurant_user
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            int numberOfRestaurantsBefore = db.restaurants.ToList<restaurant>().Count();
            
            //Act
            var actual = target.Create(restaurant2);

            //Assert
            db = new touch_for_foodEntities();
            int numberOfRestaurantsAfter = db.restaurants.ToList<restaurant>().Count();
            List<restaurant_user> restaurantUsers = db.restaurant_user
                .Where(ru => (ru.restaurant_id == restaurant2.id) && 
                (ru.user_id == user1.id)).ToList<restaurant_user>();
            Assert.AreEqual((numberOfRestaurantsBefore + 1), numberOfRestaurantsAfter); //Only one restaurant gets created
            Assert.IsTrue(restaurantUsers.Count() == 1); //Only one restaurant_user is created
        }

        /// <summary>
        /// Tests that if exception is thrown that a restaurant and restaurant user are not
        /// created and that a error message is shown.
        /// </summary>
        [TestMethod()]
        public void CreateExceptionTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            target = new RestaurantController(); //reset target creating exception. 
            int numberOfRestaurantsBefore = db.restaurants.ToList<restaurant>().Count();

            //Act
            ViewResult actual = target.Create(restaurant2) as ViewResult;

            //Assert
            db = new touch_for_foodEntities();
            int numberOfRestaurantsAfter = db.restaurants.ToList<restaurant>().Count();
            List<restaurant_user> restaurantUsers = db.restaurant_user
                .Where(ru => (ru.restaurant_id == restaurant2.id) && 
                (ru.user_id == user1.id)).ToList<restaurant_user>();
            string errorMsg = actual.ViewBag.Error;
            Assert.AreEqual(numberOfRestaurantsBefore, numberOfRestaurantsAfter); //no restaurants are created
            Assert.IsTrue(restaurantUsers.Count() == 0); //no restaurant_user are created
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        /// A test for Edit(int)
        ///</summary>
        [TestMethod()]
        public void EditTest()
        {
            //Setup
            int expected = restaurant1.id;

            //Act
            ViewResult actual = (ViewResult)target.Edit(restaurant1.id);

            //Assert
            Assert.AreEqual(((restaurant)actual.Model).id, expected);
        }

        /// <summary>
        /// A test for Edit(restaurant)
        ///</summary>
        [TestMethod()]
        public void EditRestaurantTest()
        {
            //Setup
            db = new touch_for_foodEntities();
            string changeString = "testingEditMethod";
            restaurant1.name = changeString;

            //Check Setup
            Assert.IsFalse(db.restaurants.Find(restaurant1.id).name.Equals(changeString, StringComparison.OrdinalIgnoreCase));

            //Act
            var actual = target.Edit(restaurant1);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.restaurants.Find(restaurant1.id).name.Equals(changeString, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// A test for Edit(restaurant) if exception occurs the restaurant is uneffected and 
        /// a warning message us sent to the view.
        ///</summary>
        [TestMethod()]
        public void EditRestaurantExceptionTest()
        {
            //Setup
            db = new touch_for_foodEntities();
            string changeString = "testingEditMethod";
            restaurant1.name = changeString;
            restaurant1.version += 5;

            //Check Setup
            Assert.IsFalse(db.restaurants.Find(restaurant1.id).name.Equals(changeString, StringComparison.OrdinalIgnoreCase));

            //Act
            ViewResult actual = target.Edit(restaurant1) as ViewResult;

            //Assert
            db = new touch_for_foodEntities();
            string warningMsg = actual.ViewBag.Warning;
            Assert.IsFalse(db.restaurants.Find(restaurant1.id).name.Equals(changeString, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(warningMsg); //error message is sent to view
        }

        /// <summary>
        /// A test for Delete(int)
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Setup
            int expected = restaurant1.id;

            //Act
            ViewResult actual = (ViewResult)target.Delete(restaurant1.id);

            //Assert
            Assert.AreEqual(((restaurant)actual.Model).id, expected);
        }

        /// <summary>
        /// A test for DeleteConfirmed(int)
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedTest()
        {
            //Setup
            db = new touch_for_foodEntities();
            user1.current_table_id = null;
            db.Entry(user1).State = EntityState.Modified;
            db.SaveChanges();

            //CheckSetup
            Assert.IsFalse(db.restaurants.Find(restaurant1.id).is_deleted);
            Assert.IsNotNull(db.tables.Find(table1.id));
            Assert.IsNotNull(db.restaurant_user.Find(restaurantUser1.id).restaurant_id);
            Assert.IsFalse(db.menus.Find(menu1.id).is_deleted);
            Assert.IsNotNull(db.reviews.Find(review1.id));

            //Act
            var actual = target.DeleteConfirmed(restaurant1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.restaurants.Find(restaurant1.id).is_deleted);
            Assert.IsNull(db.tables.Find(table1.id));
            Assert.IsNull(db.restaurant_user.Find(restaurantUser1.id).restaurant_id);
            Assert.IsTrue(db.menus.Find(menu1.id).is_deleted);
            Assert.IsNull(db.reviews.Find(review1.id));
        }

        /// <summary>
        /// A test for DeleteConfirmed(int) exception makes sure that nothing is changed if
        /// exception id thrown.
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedExceptionTest()
        {
            //Setup
            db = new touch_for_foodEntities();

            //CheckSetup
            Assert.IsFalse(db.restaurants.Find(restaurant1.id).is_deleted);
            Assert.IsNotNull(db.tables.Find(table1.id));
            Assert.IsNotNull(db.restaurant_user.Find(restaurantUser1.id).restaurant_id);
            Assert.IsFalse(db.menus.Find(menu1.id).is_deleted);
            Assert.IsNotNull(db.reviews.Find(review1.id));

            //Act
            ViewResult actual = target.DeleteConfirmed(restaurant1.id) as ViewResult;

            //Assert
            db = new touch_for_foodEntities();
            string errorMsg = actual.ViewBag.Error;
            Assert.IsFalse(db.restaurants.Find(restaurant1.id).is_deleted);
            Assert.IsNotNull(db.tables.Find(table1.id));
            Assert.IsNotNull(db.restaurant_user.Find(restaurantUser1.id).restaurant_id);
            Assert.IsFalse(db.menus.Find(menu1.id).is_deleted);
            Assert.IsNotNull(db.reviews.Find(review1.id));
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for Index
        ///</summary>
        [TestMethod()]
        public void IndexTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            List<restaurant> expectedRestaurants = db.restaurants.Where(r => r.is_deleted == false).ToList();
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Index();

            //Assert
            List<restaurant> actualRestaurants = actual.Model as List<restaurant>;
            Assert.AreEqual(expectedRestaurants.Count(), actualRestaurants.Count());
        }
        #endregion
    }
}
