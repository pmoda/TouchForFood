using System.Data;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Exceptions;
using TouchForFood.Mappers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;

namespace TouchForFood.Tests
{    
    /// <summary>
    ///This is a test class for OrderOMTest and is intended
    ///to contain all OrderOMTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OrderOMTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static order order1;
        private static category category1;
        private static restaurant restaurant1;
        private static menu menu1;
        private static menu_category menuCategory1;
        private static table table1;
        private static item item1;
        private static menu_item menuItem1;
        private static order_item orderItem1;
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

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup(){ }

        /// <summary>
        /// Use TestInitialize to run code before running each test 
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //Add test data (order specific)
            category1 = testDatabase.AddCategory();
            restaurant1 = testDatabase.AddRestaurant();
            menu1 = testDatabase.AddMenu(restaurant1);
            menuCategory1 = testDatabase.AddMenuCategory(category1, menu1);
            table1 = testDatabase.AddTable(restaurant1);
            order1 = testDatabase.AddOrder(table1);
            item1 = testDatabase.AddItem(category1);
            menuItem1 = testDatabase.AddMenuItem(item1, menuCategory1);
            orderItem1 = testDatabase.AddOrderItem(order1, menuItem1);
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveOrderItem(orderItem1);
            testDatabase.RemoveMenuItem(menuItem1);
            testDatabase.RemoveItem(item1);
            testDatabase.RemoveOrder(order1);
            testDatabase.RemoveTable(table1);
            testDatabase.RemoveMenuCategory(menuCategory1);
            testDatabase.RemoveMenu(menu1);
            testDatabase.RemoveRestaurant(restaurant1);
            testDatabase.RemoveCategory(category1);
        }
        #endregion

        #region Test Methods
        /// <summary>
        /// A Test For Delete 
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            OrderOM target = new OrderOM(db);
            int expected = 2;
            int version = order1.version;
            //Check-Setup
            Assert.IsFalse(db.orders.Find(order1.id).order_status ==
                (int)OrderStatusHelper.OrderStatusEnum.DELETED);
            Assert.IsFalse(db.order_item.Find(orderItem1.id).order_item_status ==
                (int)OrderStatusHelper.OrderItemStatusEnum.DELETED);

            //Act
            int actual = target.delete(order1.id);

            //Assert
            db = new touch_for_foodEntities();
            order o = db.orders.Find(order1.id);
            Assert.IsTrue(o.order_status == 
                (int)OrderStatusHelper.OrderStatusEnum.DELETED);
            Assert.AreEqual(o.version, ++version);
            Assert.IsTrue(db.order_item.Find(orderItem1.id).order_item_status == 
                (int)OrderStatusHelper.OrderItemStatusEnum.DELETED);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for delete
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(ItemActiveException))]
        public void DeleteItemActiveExceptionTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            OrderOM target = new OrderOM(db);
            orderItem1.order_item_status = (int)OrderStatusHelper.OrderItemStatusEnum.PROCESSING;
            db.Entry(orderItem1).State = EntityState.Modified;
            db.SaveChanges();

            //Act
            int actual = target.delete(order1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsFalse(db.orders.Find(order1.id).order_status ==
                (int)OrderStatusHelper.OrderStatusEnum.DELETED);
            Assert.IsFalse(db.order_item.Find(orderItem1.id).order_item_status ==
                (int)OrderStatusHelper.OrderItemStatusEnum.DELETED);
        }
        #endregion
    }
}
