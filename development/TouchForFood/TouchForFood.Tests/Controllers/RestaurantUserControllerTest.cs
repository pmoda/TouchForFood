using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Controllers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;
using TouchForFood.Util.Security;
using System.Collections.Generic;

namespace TouchForFood.Tests
{
    
    
    /// <summary>
    ///This is a test class for RestaurantUserControllerTest and is intended
    ///to contain all RestaurantUserControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class RestaurantUserControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static restaurant restaurant1;
        private static table table1;
        private static user user1;
        private static restaurant_user restoUser1;
        private static restaurant_user restoUser2;
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
            user1 = testDatabase.AddUser("restoUser1UnitTest@email.com", table1, (int)SiteRoles.Admin);
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
            restoUser1 = testDatabase.AddRestaurantUser(user1, restaurant1);

            restoUser2 = new restaurant_user();
            restoUser2.restaurant_id = restaurant1.id;
            restoUser2.user_id = user1.id;
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveRestaurantUser(restoUser1);
            testDatabase.RemoveRestaurantUser(restoUser2);
        }
        #endregion

        #region Test Methods
        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateViewTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            RestaurantUserController target = new RestaurantUserController();
            SelectList expectedRestoList = new SelectList(db.restaurants, "id", "name");
            SelectList expectedUsersList = new SelectList(db.users.Where(u => u.user_role == (int)SiteRoles.Restaurant
                || u.user_role == (int)SiteRoles.Admin), "id", "username");
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Create();

            //Assert
            SelectList actualRestoList = actual.ViewBag.restaurant_id;
            Assert.AreEqual(expectedRestoList.GetType(), actualRestoList.GetType());
            Assert.AreEqual(expectedRestoList.Count(), actualRestoList.Count());

            SelectList actualUserList = actual.ViewBag.user_id;
            Assert.AreEqual(expectedUsersList.GetType(), actualUserList.GetType());
            Assert.AreEqual(expectedUsersList.Count(), actualUserList.Count());
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            RestaurantUserController target = new RestaurantUserController();
            int numExpectedAssociations = db.restaurant_user.ToList<restaurant_user>().Count() + 1;

            // Act
            var actualResult = target.Create(restoUser2) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            int actualRestoUsers = db.restaurant_user.ToList<restaurant_user>().Count();
            Assert.AreEqual(numExpectedAssociations, actualRestoUsers);
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
            RestaurantUserController target = new RestaurantUserController();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            SelectList expectedRestoList = new SelectList(db.restaurants, "id", "name", restoUser2.restaurant_id);
            SelectList expectedUserList = new SelectList(db.users.Where(u => u.user_role == (int)SiteRoles.Restaurant
                || u.user_role == (int)SiteRoles.Admin), "id", "username");
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Create(restoUser2);

            // Assert
            SelectList actualRestoList = actual.ViewBag.restaurant_id;
            Assert.AreEqual(expectedRestoList.GetType(), actualRestoList.GetType());
            Assert.AreEqual(expectedRestoList.Count(), actualRestoList.Count());

            SelectList actualUserList = actual.ViewBag.user_id;
            Assert.AreEqual(expectedUserList.GetType(), actualUserList.GetType());
            Assert.AreEqual(expectedUserList.Count(), actualUserList.Count());
        }


        /// <summary>
        ///A test for Delete
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Arrange
            RestaurantUserController target = new RestaurantUserController();
            int expected = restoUser1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Delete(restoUser1.id);

            //Assert
            Assert.AreEqual(expected, ((restaurant_user)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for DeleteConfirmed
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            RestaurantUserController target = new RestaurantUserController();

            //CheckSetup
            Assert.AreEqual(db.restaurant_user.Find(restoUser1.id).id, restoUser1.id);

            // Act
            var actualResult = target.DeleteConfirmed(restoUser1.id) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];

            Assert.IsNull(db.restaurant_user.Find(restoUser1.id));
            Assert.AreEqual("Index", actualResultURI);
        }

        /// <summary>
        ///A test for Index
        ///</summary>
        [TestMethod()]
        public void IndexTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            RestaurantUserController target = new RestaurantUserController();
            List<restaurant_user> expectedRestoUsers = db.restaurant_user.ToList();
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Index();

            //Assert
            List<restaurant_user> actualRestoUsers = actual.Model as List<restaurant_user>;
            Assert.AreEqual(expectedRestoUsers.Count(), actualRestoUsers.Count());
        }
        #endregion
    }
}
