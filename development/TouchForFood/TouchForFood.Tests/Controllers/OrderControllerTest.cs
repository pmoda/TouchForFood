using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Controllers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;
using TouchForFood.Util.Security;

namespace TouchForFood.Tests
{
    /// <summary>
    ///This is a test class for OrderControllerTest and is intended
    ///to contain all OrderControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class OrderControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static OrderController target;
        private static item item1;
        private static category category1;
        private static restaurant restaurant1;
        private static menu menu1;
        private static menu_category menuCategory1;
        private static menu_item menuItem1;
        private static table table1;
        private static order order1;
        private static order_item orderItem1;
        private static user user1;
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
            user1 = testDatabase.AddUser("orderUnitTest@email.com", table1, (int)SiteRoles.Admin);
           
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveUser(user1);
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
            order1 = testDatabase.AddOrder(table1);

            //Session
            db = new touch_for_foodEntities();
            target = new OrderController();
            Session session = new Session(db, target);
            session.simulateLogin(user1.username, user1.password);
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveOrder(order1);
        }
        #endregion

        #region Test Methods
        /// <summary>
        ///A test for Cancel
        ///</summary>
        [TestMethod()]
        public void CancelTest()
        {
            //Arrange
            int expected = order1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Cancel(order1.id);

            //Assert
            Assert.AreEqual(expected, ((order)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Arrange
            int expected = order1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Delete(order1.id);

            //Assert
            Assert.AreEqual(expected, ((order)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for Details
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            int expected = order1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Details(order1.id);

            //Assert
            Assert.AreEqual(expected, ((order)actual.ViewData.Model).id);
        }
        #endregion
    }
}
