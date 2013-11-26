using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Controllers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;

namespace TouchForFood.Tests
{
    /// <summary>
    ///This is a test class for Order_ItemControllerTest and is intended
    ///to contain all Order_ItemControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Order_ItemControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static item item1;
        private static category category1;
        private static restaurant restaurant1;
        private static menu menu1;
        private static menu_category menuCategory1;
        private static menu_item menuItem1;
        private static table table1;
        private static order order1;
        private static order_item orderItem1;
        private static order_item orderItem2;
        private static order_item orderItem3;
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

            //Add test data (order specific)
            restaurant1 = testDatabase.AddRestaurant();
            table1 = testDatabase.AddTable(restaurant1);
            order1 = testDatabase.AddOrder(table1);
            item1 = testDatabase.AddItem();
            category1 = testDatabase.AddCategory();
            menu1 = testDatabase.AddMenu(restaurant1);
            menuCategory1 = testDatabase.AddMenuCategory(category1, menu1);
            menuItem1 = testDatabase.AddMenuItem(item1, menuCategory1);
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveMenuItem(menuItem1);
            testDatabase.RemoveMenuCategory(menuCategory1);
            testDatabase.RemoveMenu(menu1);
            testDatabase.RemoveCategory(category1);
            testDatabase.RemoveItem(item1);
            testDatabase.RemoveOrder(order1);
            testDatabase.RemoveTable(table1);
            testDatabase.RemoveRestaurant(restaurant1);
        }

        /// <summary>
        /// Use TestInitialize to run code before running each test
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //Add test data (order specific)
            orderItem1 = testDatabase.AddOrderItem(order1, menuItem1);
            orderItem2 = testDatabase.AddOrderItem(order1, menuItem1);
            orderItem3 = new order_item();
            orderItem3.menu_item_id = menuItem1.id;
            orderItem3.order_id = order1.id;
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveOrderItem(orderItem1);
            testDatabase.RemoveOrderItem(orderItem2);
            testDatabase.RemoveOrderItem(orderItem3);
        }
        #endregion

        #region Test Methods
        /// <summary>
        ///A test for Details
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            Order_ItemController target = new Order_ItemController(); ;
            int expected = orderItem1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Details(orderItem1.id);

            //Assert
            Assert.AreEqual(expected, ((order_item)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateViewTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Order_ItemController target = new Order_ItemController();
            SelectList expectedMenuList = new SelectList(db.menu_item, "id", "id");
            SelectList expectedOrderList = new SelectList(db.orders, "id", "id");
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Create();

            //Assert
            SelectList actualMenuList = actual.ViewBag.menu_item_id;
            Assert.AreEqual(expectedMenuList.GetType(), actualMenuList.GetType());
            Assert.AreEqual(expectedMenuList.Count(), actualMenuList.Count());

            SelectList actualOrderList = actual.ViewBag.order_id;
            Assert.AreEqual(expectedOrderList.GetType(), actualOrderList.GetType());
            Assert.AreEqual(expectedOrderList.Count(), actualOrderList.Count());
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Order_ItemController target = new Order_ItemController();
            int expectedOrderItems = db.order_item.ToList<order_item>().Count() + 1;

            // Act
            var actualResult = target.Create(orderItem3) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            int actualOrderItems = db.order_item.ToList<order_item>().Count();
            Assert.AreEqual(expectedOrderItems, actualOrderItems);
            Assert.IsNotNull(db.order_item.Find(orderItem3.id));
            Assert.AreEqual("Index", actualResultURI);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Order_ItemController target = new Order_ItemController();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            SelectList expectedMenuList = new SelectList(db.menu_item, "id", "id", orderItem3.menu_item_id);
            SelectList expectedOrderList = new SelectList(db.orders, "id", "id", orderItem3.order_id);
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Create(orderItem3);

            // Assert
            SelectList actualMenuList = actual.ViewBag.menu_item_id;
            Assert.AreEqual(expectedMenuList.GetType(), actualMenuList.GetType());
            Assert.AreEqual(expectedMenuList.Count(), actualMenuList.Count());

            SelectList actualOrderList = actual.ViewBag.order_id;
            Assert.AreEqual(expectedOrderList.GetType(), actualOrderList.GetType());
            Assert.AreEqual(expectedOrderList.Count(), actualOrderList.Count());

            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditViewTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Order_ItemController target = new Order_ItemController();
            int expected = orderItem1.id;
            SelectList expectedMenuList = new SelectList(db.menu_item, "id", "id", orderItem1.menu_item_id);
            SelectList expectedOrderList = new SelectList(db.orders, "id", "id", orderItem1.order_id);
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Edit(orderItem1.id);

            // Assert
            Assert.AreEqual(expected, ((order_item)actual.ViewData.Model).id);

            SelectList actualMenuList = actual.ViewBag.menu_item_id;
            Assert.AreEqual(expectedMenuList.GetType(), actualMenuList.GetType());
            Assert.AreEqual(expectedMenuList.Count(), actualMenuList.Count());

            SelectList actualOrderList = actual.ViewBag.order_id;
            Assert.AreEqual(expectedOrderList.GetType(), actualOrderList.GetType());
            Assert.AreEqual(expectedOrderList.Count(), actualOrderList.Count());
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Order_ItemController target = new Order_ItemController();
            int expectedVersion = orderItem1.version + 1;
            string changeString= "OrderItemUnitTest";
            orderItem1.note = changeString;

            //Check Setup
            Assert.IsFalse(db.order_item.Find(orderItem1.id).note == changeString);

            // Act
            var actualResult = target.Edit(orderItem1) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            Assert.AreEqual(db.order_item.Find(orderItem1.id).version, expectedVersion); //version was incremented
            Assert.IsTrue(db.order_item.Find(orderItem1.id).note == changeString); //note was changed
            Assert.AreEqual("Index", actualResultURI); //directed to correct location
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTestLockError()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Order_ItemController target = new Order_ItemController();
            int expected = orderItem1.id;
            orderItem1.version += 5;
            SelectList expectedMenuList = new SelectList(db.menu_item, "id", "id", orderItem1.menu_item_id);
            SelectList expectedOrderList = new SelectList(db.orders, "id", "id", orderItem1.order_id);
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Edit(orderItem1);

            // Assert
            Assert.AreEqual(expected, ((order_item)actual.ViewData.Model).id);

            SelectList actualMenuList = actual.ViewBag.menu_item_id;
            Assert.AreEqual(expectedMenuList.GetType(), actualMenuList.GetType());
            Assert.AreEqual(expectedMenuList.Count(), actualMenuList.Count());

            SelectList actualOrderList = actual.ViewBag.order_id;
            Assert.AreEqual(expectedOrderList.GetType(), actualOrderList.GetType());
            Assert.AreEqual(expectedOrderList.Count(), actualOrderList.Count());

            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Order_ItemController target = new Order_ItemController();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            int expected = orderItem1.id;
            SelectList expectedMenuList = new SelectList(db.menu_item, "id", "id", orderItem1.menu_item_id);
            SelectList expectedOrderList = new SelectList(db.orders, "id", "id", orderItem1.order_id);
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Edit(orderItem1);

            // Assert
            Assert.AreEqual(expected, ((order_item)actual.ViewData.Model).id);

            SelectList actualMenuList = actual.ViewBag.menu_item_id;
            Assert.AreEqual(expectedMenuList.GetType(), actualMenuList.GetType());
            Assert.AreEqual(expectedMenuList.Count(), actualMenuList.Count());

            SelectList actualOrderList = actual.ViewBag.order_id;
            Assert.AreEqual(expectedOrderList.GetType(), actualOrderList.GetType());
            Assert.AreEqual(expectedOrderList.Count(), actualOrderList.Count());

            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Arrange
            Order_ItemController target = new Order_ItemController(); ;
            int expected = orderItem1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Delete(orderItem1.id);

            //Assert
            Assert.AreEqual(expected, ((order_item)actual.ViewData.Model).id);
        }
        #endregion
    }
}
