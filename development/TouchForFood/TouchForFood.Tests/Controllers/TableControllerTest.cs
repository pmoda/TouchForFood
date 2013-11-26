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
    ///This is a test class for TableControllerTest and is intended
    ///to contain all TableControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TableControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static TableController target;
        private static restaurant restaurant1;
        private static table table1;
        private static table table2;
        private static user user1;
        private static order order1;
        private static service_request request1;
        private static restaurant_user restaurantUser1;
        #endregion

        #region Properties
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
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

        //Use ClassCleanup to run code after all tests in a class have run
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
            table1 = testDatabase.AddTable(restaurant1);
            table2 = new table();
            user1 = testDatabase.AddUser("tableUnitTest@email.com", table1, (int)SiteRoles.Restaurant);
            order1 = testDatabase.AddOrder(table1);
            request1 = testDatabase.AddServiceRequest(table1);
            restaurantUser1 = testDatabase.AddRestaurantUser(user1, restaurant1);

            //Session
            db = new touch_for_foodEntities();
            target = new TableController();
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
            testDatabase.RemoveRestaurantUser(restaurantUser1);
            testDatabase.RemoveServiceRequest(request1);
            testDatabase.RemoveOrder(order1);
            testDatabase.RemoveUser(user1);
            testDatabase.RemoveTable(table1);
            testDatabase.RemoveTable(table2);
            testDatabase.RemoveRestaurant(restaurant1);
            
        }
        #endregion

        #region Test Methods
        /// <summary>
        /// Must return the details of a table
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            int expected = table1.id;

            //Act
            ViewResult actual = (ViewResult)target.Details(table1.id);

            //Assert
            Assert.AreEqual(expected, ((table)actual.ViewData.Model).id);
        }

        /// <summary>
        /// Test create view
        /// </summary>
        [TestMethod()]
        public void CreateViewTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            int expectedCount = 1;

            //Act
            ViewResult actual = (ViewResult)target.Create();

            //Assert
            SelectList actualRestoList = actual.ViewBag.restaurant_id;
            Assert.AreEqual(expectedCount, actualRestoList.Count());
        }

        /// <summary>
        /// Must create a restaurant and restaurant_user
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            table2.restaurant_id = restaurant1.id;
            table2.name = "TableControllerTEST";
            int numberOfTablesBefore = db.restaurants.Find(restaurant1.id).tables.ToList<table>().Count();

            //Act
            var actual = target.Create(table2);

            //Assert
            db = new touch_for_foodEntities();
            int numberOfTablesAfter = db.restaurants.Find(restaurant1.id).tables.ToList<table>().Count();
            Assert.AreEqual((numberOfTablesBefore + 1), numberOfTablesAfter); //Only one table gets created for this restaurant
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
            int numberOfTablesBefore = db.restaurants.Find(restaurant1.id).tables.ToList<table>().Count();

            //Act
            ViewResult actual = target.Create(table2) as ViewResult;

            //Assert
            db = new touch_for_foodEntities();
            int numberOfTablesAfter = db.restaurants.Find(restaurant1.id).tables.ToList<table>().Count();
            string errorMsg = actual.ViewBag.Error;
            Assert.AreEqual(numberOfTablesBefore, numberOfTablesAfter); //no restaurants are created
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        /// A test for Edit(int)
        ///</summary>
        [TestMethod()]
        public void EditTest()
        {
            //Setup
            int expected = table1.id;
            SelectList expectedRestoList = new SelectList(db.restaurants, "id", "name", table1.restaurant_id);

            //Act
            ViewResult actual = (ViewResult)target.Edit(table1.id);

            //Assert
            Assert.AreEqual(((table)actual.Model).id, expected);
            Assert.AreEqual(expectedRestoList.GetType(), actual.ViewBag.restaurant_id.GetType());
            SelectList actualRestoList = actual.ViewBag.restaurant_id;
            Assert.AreEqual(expectedRestoList.Count(), actualRestoList.Count());
        }

        /// <summary>
        /// A test for Edit(table)
        ///</summary>
        [TestMethod()]
        public void EditTableTest()
        {
            //Setup
            db = new touch_for_foodEntities();
            string changeString = "testingEditMethod";
            table1.name = changeString;

            //Check Setup
            Assert.IsFalse(db.tables.Find(table1.id).name.Equals(changeString, StringComparison.OrdinalIgnoreCase));

            //Act
            var actual = target.Edit(table1);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.tables.Find(table1.id).name.Equals(changeString, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// A test for Edit(table) if exception occurs the table is uneffected and 
        /// a error message is sent to the view.
        ///</summary>
        [TestMethod()]
        public void EditTableExceptionTest()
        {
            //Setup
            db = new touch_for_foodEntities();
            string changeString = "testingEditMethod";
            table1.name = changeString;
            table1.restaurant_id = restaurant1.id+1;

            //Check Setup
            Assert.IsFalse(db.restaurants.Find(restaurant1.id).name.Equals(changeString, StringComparison.OrdinalIgnoreCase));

            //Act
            ViewResult actual = target.Edit(table1) as ViewResult;

            //Assert
            db = new touch_for_foodEntities();
            string errorMsg = actual.ViewBag.Error;
            Assert.IsFalse(db.tables.Find(table1.id).name.Equals(changeString, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTableTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            int expected = table1.id;
            SelectList expectedRestos = new SelectList(db.restaurants, "id", "name", table1.restaurant_id);
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Edit(table1);

            // Assert
            Assert.AreEqual(expected, ((table)actual.ViewData.Model).id);

            SelectList actualRestos = actual.ViewBag.restaurant_id;
            Assert.AreEqual(expectedRestos.GetType(), actualRestos.GetType());
            Assert.AreEqual(expectedRestos.Count(), actualRestos.Count());

            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }
        /// <summary>
        /// A test for Delete(int)
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Setup
            int expected = table1.id;

            //Act
            ViewResult actual = (ViewResult)target.Delete(table1.id);

            //Assert
            Assert.AreEqual(((table)actual.Model).id, expected);
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
            Assert.IsNotNull(db.tables.Find(table1.id));
            Assert.IsNotNull(db.orders.Find(order1.id).table_id);
            Assert.IsNotNull(db.service_request.Find(request1.id).table_id);

            //Act
            var actual = target.DeleteConfirmed(table1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsNull(db.tables.Find(table1.id));
            Assert.IsNull(db.service_request.Find(request1.id).table_id);
            Assert.AreEqual((request1.version + 1), db.service_request.Find(request1.id).version);
            Assert.IsNull(db.orders.Find(order1.id).table_id);
            Assert.AreEqual((order1.version + 1), db.orders.Find(order1.id).version);
        }

        /// <summary>
        /// A test for DeleteConfirmed(int) if exception is thrown no
        /// changes should be saved to db regarding table and any of it's
        /// relations.
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedExceptionTest()
        {
            //Setup
            db = new touch_for_foodEntities();

            //CheckSetup
            Assert.IsNotNull(db.tables.Find(table1.id));
            Assert.IsNotNull(db.orders.Find(order1.id).table_id);
            Assert.IsNotNull(db.service_request.Find(request1.id).table_id);

            //Act
            ViewResult actual = target.DeleteConfirmed(table1.id) as ViewResult;

            //Assert
            db = new touch_for_foodEntities();
            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(db.tables.Find(table1.id));
            Assert.IsNotNull(db.service_request.Find(request1.id).table_id);
            Assert.AreEqual(request1.version, db.service_request.Find(request1.id).version);
            Assert.IsNotNull(db.orders.Find(order1.id).table_id);
            Assert.AreEqual(order1.version, db.orders.Find(order1.id).version);
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        /// Test create view
        /// </summary>
        [TestMethod()]
        public void IndexTest()
        {
            //Setup
            db = new touch_for_foodEntities();
           // List<table> expectedTables = db.tables.ToList<table>();
            List<table> expectedTables = db.tables.Where(t => t.restaurant_id == restaurant1.id).ToList<table>();
            //Act
            ViewResult actual = (ViewResult)target.Index();

            //Assert
            List<table> actualTables = actual.Model as List<table>;
            Assert.AreEqual(actualTables.Count(), expectedTables.Count());
        }

        [TestMethod()]
        public void ManageIndexTest()
        {
            //Act
            ViewResult actual = (ViewResult) target.ManageIndex(table1.id);

            //Assert
            Assert.AreEqual(actual.ViewBag.table_id, table1.id);
            Assert.AreEqual("Manage", actual.ViewName);
        }

        [TestMethod()]
        public void AssignTest()
        {
            //Arrange
            Util.Security.AES aes = new Util.Security.AES();
            string encryption = aes.EncryptToString(table1.id.ToString());

            //Act
            RedirectToRouteResult actual = (RedirectToRouteResult)target.Assign(encryption);

            //Assert
            db = new touch_for_foodEntities();
            Assert.AreEqual("Menu", actual.RouteValues["controller"]);
            Assert.AreEqual("UserMenu", actual.RouteValues["action"]);
            Assert.AreEqual(table1.id , db.users.Find(user1.id).current_table_id);
        }
        #endregion
    }
}
