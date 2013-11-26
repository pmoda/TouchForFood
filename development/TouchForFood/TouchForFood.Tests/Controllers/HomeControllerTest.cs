using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Controllers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;
using TouchForFood.Util.Security;

namespace TouchForFood.Tests.Controllers
{
    [TestClass]
    public class HomeControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static restaurant restaurant1;
        private static table table1;
        private static user user1;
        private static HomeController target;
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
            user1 = testDatabase.AddUser("homeUnitTest@email.com", table1, (int)SiteRoles.Admin);

            //Session
            db = new touch_for_foodEntities();
            target = new HomeController();
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
            testDatabase.RemoveUser(user1);
            testDatabase.RemoveTable(table1);
            testDatabase.RemoveRestaurant(restaurant1);
        }
        #endregion

        #region Test Methods
        /// <summary>
        /// A test for index.
        /// </summary>
        [TestMethod]
        public void Index()
        {
            // Act
            ViewResult result = (ViewResult)target.Index();

            // Assert
            Assert.AreEqual("Welcome to Touch For Food", result.ViewBag.Message);
        }
        #endregion
    }
}
