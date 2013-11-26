using System.Collections.Generic;
using System.Data.Entity;
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
    ///This is a test class for BillControllerTest and is intended
    ///to contain all BillControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class BillControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static restaurant restaurant1;
        private static table table1;
        private static user user1;
        private static order order1;
        private static order_item orderItem1;
        private static bill bill1;
        private static menu_item menuItem1;
        private static item item1;
        private static category category1;
        private static menu menu1;
        private static menu_category menuCategory1;
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
            //Add test data (order specific)
            testDatabase = new TestDatabaseHelper();
            restaurant1 = testDatabase.AddRestaurant();
            table1 = testDatabase.AddTable(restaurant1);
            user1 = testDatabase.AddUser("billUnitTest@email.com", table1, (int)SiteRoles.Admin);
            order1 = testDatabase.AddOrder(table1);
            item1 = testDatabase.AddItem();
            category1 = testDatabase.AddCategory();
            menu1 = testDatabase.AddMenu(restaurant1);
            menuCategory1 = testDatabase.AddMenuCategory(category1, menu1);
            menuItem1 = testDatabase.AddMenuItem(item1, menuCategory1);

            //Session
            db = new touch_for_foodEntities();
            BillController target = new BillController();
            Session session = new Session(db, target);
            session.simulateLogin(user1.username, user1.password);
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
            testDatabase.RemoveUser(user1);
            testDatabase.RemoveTable(table1);
            testDatabase.RemoveRestaurant(restaurant1);
        }

        /// <summary>
        ///Use TestInitialize to run code before running each test 
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            bill1 = testDatabase.AddBill(order1);
            orderItem1 = testDatabase.AddOrderItem(order1, bill1, menuItem1);
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            testDatabase.RemoveOrderItem(orderItem1);
            foreach (bill b in db.orders.Find(order1.id).bills.ToList<bill>())
            {
                testDatabase.RemoveBill(b);
            }
        }
        #endregion

        #region Test Methods
        /// <summary>
        ///A test for CreateBillForOrder
        ///</summary>
        [TestMethod()]
        public void CreateBillForOrderTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            BillController target = new BillController();
            int numberOfTotalBillsBefore = db.orders.SelectMany(o => o.bills).ToList<bill>().Count();
            int expected = 1;

            //Act
            int actual = target.CreateBillForOrder(order1.id);

            //Assert
            db = new touch_for_foodEntities();
            int numberOfTotalBillsAfter = db.orders.SelectMany(o => o.bills).ToList<bill>().Count();
            Assert.IsTrue(actual == expected);
            Assert.IsTrue((numberOfTotalBillsBefore + 1) == numberOfTotalBillsAfter);
        }

        /// <summary>
        /// test create view
        /// </summary>
        [TestMethod()]
        public void CreateViewTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            BillController target = new BillController();
            SelectList orderList = new SelectList(db.orders, "id", "id");

            //Act
            var actual = (ViewResult)target.Create();

            //Assert         
            Assert.AreEqual(orderList.GetType(), actual.ViewBag.order_id.GetType());
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            BillController target = new BillController();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            SelectList expectedOrderList = new SelectList(db.orders, "id", "id", bill1.order_id);
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Create(bill1);

            // Assert
            SelectList actualOrderList = actual.ViewBag.order_id;
            Assert.AreEqual(expectedOrderList.GetType(), actualOrderList.GetType());
            Assert.AreEqual(expectedOrderList.Count(), actualOrderList.Count());

            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for DeleteBillFromOrder
        ///</summary>
        [TestMethod()]
        public void DeleteBillFromOrderTest()
        {
            //Check Setup
            db = new touch_for_foodEntities();
            Assert.IsFalse(db.bills.Find(bill1.id) == null);

            //Arrange
            BillController target = new BillController();
            int numberOfTotalBillsBefore = db.orders.SelectMany(o => o.bills).ToList<bill>().Count();
            int expected = 1;

            //Act
            int actual = target.DeleteBillFromOrder(bill1.id);

            //Assert
            db = new touch_for_foodEntities();
            int numberOfTotalBillsAfter = db.orders.SelectMany(o => o.bills).ToList<bill>().Count();
            Assert.AreEqual(expected, actual);
            Assert.IsTrue((numberOfTotalBillsBefore - 1) == numberOfTotalBillsAfter);
            Assert.IsTrue(db.bills.Find(bill1.id) == null);
        }

        /// <summary>
        ///A test for RemoveOrderItem
        ///</summary>
        [TestMethod()]
        public void RemoveOrderItemTest()
        {
            //Check Setup
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.order_item.Find(orderItem1.id).bill_id == bill1.id);

            //Arrange
            BillController target = new BillController();

            //Act
            RedirectToRouteResult actual = (RedirectToRouteResult)target.RemoveOrderItem(orderItem1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.AreEqual("Bill", actual.RouteValues["controller"]);
            Assert.AreEqual("ManageBills", actual.RouteValues["action"]);
            Assert.AreEqual(order1.id, actual.RouteValues["id"]);
            Assert.IsTrue(db.order_item.Find(orderItem1.id).bill_id == null);
        }

        /// <summary>
        ///A test for ManageBills
        ///</summary>
        [TestMethod()]
        public void ManageBillsTest()
        {
            //Arrange
            BillController target = new BillController();

            //Act
            ViewResult actual = (ViewResult)target.ManageBills(order1.id);

            //Assert
            Assert.AreEqual(actual.ViewName, "ManageBills");
            Assert.AreEqual(((order)actual.Model).id, order1.id);
        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Arrange
            BillController target = new BillController();

            //Act
            ViewResult actual = (ViewResult)target.Delete(bill1.id);

            //Assert
            Assert.AreEqual(((bill)actual.Model).id, bill1.id);
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTest()
        {
            //Arrange
            BillController target = new BillController();

            //Act
            ViewResult actual = (ViewResult)target.Edit(bill1.id);

            //Assert
            Assert.AreEqual(((bill)actual.Model).id, bill1.id);
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditBillTest()
        {
            //Check Setup
            db = new touch_for_foodEntities();
            Assert.IsFalse(db.bills.Find(bill1.id).total == 9M);
            Assert.IsFalse(db.bills.Find(bill1.id).tps == 3M);

            //Arrange
            BillController target = new BillController();
            bill1.total = 9M;
            bill1.tps = 3M;

            //Act
            RedirectToRouteResult actual = (RedirectToRouteResult)target.Edit(bill1);

            //Assert
            db = new touch_for_foodEntities();
            bill newBill = db.bills.Find(bill1.id);
            Assert.IsTrue(db.bills.Find(bill1.id).total == 9M);
            Assert.IsTrue(db.bills.Find(bill1.id).tps == 3M);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void EditBillTestLockError()
        {
            // Arrange
            db = new touch_for_foodEntities();
            BillController target = new BillController();
            SelectList expectedOrderList = new SelectList(db.orders, "id", "id", bill1.order_id);
            ViewResult actual;
            bill1.version += 5;

            // Act
            actual = (ViewResult)target.Edit(bill1);

            // Assert
            SelectList actualOrderList = actual.ViewBag.order_id;
            Assert.AreEqual(expectedOrderList.GetType(), actualOrderList.GetType());
            Assert.AreEqual(expectedOrderList.Count(), actualOrderList.Count());

            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void EditBillTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            BillController target = new BillController();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            SelectList expectedOrderList = new SelectList(db.orders, "id", "id", bill1.order_id);
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Edit(bill1);

            // Assert
            SelectList actualOrderList = actual.ViewBag.order_id;
            Assert.AreEqual(expectedOrderList.GetType(), actualOrderList.GetType());
            Assert.AreEqual(expectedOrderList.Count(), actualOrderList.Count());

            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        /// A test for Add Order Item To Bill
        /// </summary>
        [TestMethod()]
        public void AddOrderItemToBillTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            BillController target = new BillController();
            order_item orderItem2 = testDatabase.AddOrderItem(order1, menuItem1);
            int numberOfOrderItemsBefore = db.bills.Find(bill1.id).order_item.Count();
            int expected = 1;

            //Act
            int actual = target.AddOrderItemToBill(bill1.id, orderItem2.id);

            //Assert
            db = new touch_for_foodEntities();
            int numberOfOrderItemsAfter = db.bills.Find(bill1.id).order_item.Count();
            Assert.AreEqual(expected, actual);
            Assert.IsTrue((numberOfOrderItemsBefore + 1) == numberOfOrderItemsAfter);

            //Cleanup
            testDatabase.RemoveOrderItem(orderItem2);
        }

        /// <summary>
        ///A test for Details
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            BillController target = new BillController();
            int expected = bill1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Details(bill1.id);

            //Assert
            Assert.AreEqual(expected, ((bill)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for Index
        ///</summary>
        [TestMethod()]
        public void IndexTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            BillController target = new BillController();
            List<bill> expectedOrders = db.bills.Include(b => b.order).ToList();
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Index();

            //Assert
            List<bill> actualOrders = actual.Model as List<bill>;
            Assert.AreEqual(expectedOrders.Count(), actualOrders.Count());
        }
        #endregion
    }
}
