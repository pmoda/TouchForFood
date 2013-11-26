using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Controllers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;
using TouchForFood.Util.Security;

namespace TouchForFood.Tests
{
    [TestClass]
    public class UserControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static restaurant restaurant1;
        private static table table1;
        private static user user1;
        private static user user2;
        private static user user3;
        private static friendship friendship1;
        private static restaurant_user restaurantUser1;
        private static order order1;
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
        public static void MyClassCleanup(){}

        /// <summary>
        /// Use TestInitialize to run code before running each test
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //Add test data (order specific)
            restaurant1 = testDatabase.AddRestaurant();
            table1 = testDatabase.AddTable(restaurant1);
            user1 = testDatabase.AddUser("user1UnitTest@email.com", table1, (int)SiteRoles.Admin);
            user3 = testDatabase.AddUser("user3UnitTest@email.com", table1, (int)SiteRoles.Customer);
            friendship1 = testDatabase.AddFriendship(user1, user3);
            restaurantUser1 = testDatabase.AddRestaurantUser(user1, restaurant1);
            order1 = testDatabase.AddOrder(table1);

            // Create a valid user object with test values
            user2 = new user();
            user2.username = "user2UnitTest@email.com";
            user2.password = "user2UnitTest@email.com";
            user2.ConfirmPassword = "user2UnitTest@email.com";
            user2.first_name = "user2UnitTest@email.com";
            user2.last_name = "user2UnitTest@email.com";
            user2.email = "user2UnitTest@email.com";
            user2.image_url = null;
            user2.current_table_id = table1.id;
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            db = new touch_for_foodEntities();
            //Remove test data (order specific)
            testDatabase.RemoveOrder(order1);
            testDatabase.RemoveRestaurantUser(restaurantUser1);
            testDatabase.RemoveFriendship(friendship1);
            foreach (user u in db.tables.Find(table1.id).users)
            {
                testDatabase.RemoveUser(u);
            }
            testDatabase.RemoveTable(table1);
            testDatabase.RemoveRestaurant(restaurant1);
        }
        #endregion

        #region Test Methods
        /// <summary>
        /// A test for index
        /// </summary>
        [TestMethod]
        public void IndexTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            UserController target = new UserController();
            List<user> expectedUsers = db.users.ToList<user>();

            // Act
            ViewResult actual = (ViewResult)target.Index();

            // Assert
            List<user> actualUsers = actual.Model as List<user>;
            Assert.AreEqual(actualUsers.Count(), expectedUsers.Count());
        }

        /// <summary>
        /// A test for details
        /// </summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            UserController target = new UserController();
            int expected = user1.id;

            //Act
            ViewResult actual = (ViewResult)target.Details(user1.id);

            //Assert
            Assert.AreEqual(expected, ((user)actual.ViewData.Model).id);
        }

        /// <summary>
        /// A test for create view.
        /// </summary>
        [TestMethod]
        public void CreateViewTest()
        {
            // Arrange
            UserController target = new UserController();

            // Act
            ViewResult actual = (ViewResult)target.Create();

            // Assert
            Assert.AreEqual("Create a new user profile", actual.ViewBag.Message);
        }

        /// <summary>
        /// Creating a valid user.
        /// </summary>
        [TestMethod]
        public void CreateUserTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            UserController target = new UserController();
            var mock = new ContextMocks(target);
            int expectedUsers = db.users.ToList<user>().Count();

            // Act
            var actualResult = target.Create(user2) as RedirectToRouteResult;
            var actualResultURI = actualResult.RouteValues["controller"] + "/" + actualResult.RouteValues["action"];

            // Assertions
            db = new touch_for_foodEntities();
            int actualUsers = db.users.ToList<user>().Count();
            Assert.AreEqual((expectedUsers + 1), actualUsers);
            Assert.IsNotNull(actualResult, "Result obtained from actual result is null");
            Assert.AreEqual("Home/Index", actualResultURI);
        }

        /// <summary>
        /// Test create duplicate username.
        /// </summary>
        [TestMethod]
        public void CreateUserDbUpdateExceptionUsernameTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            UserController target = new UserController();
            user2.username = user1.username;
            int expectedUsers = db.users.ToList<user>().Count();

            var actual = target.Create(user2) as ViewResult;

            // Assert
            db = new touch_for_foodEntities();
            int actualUsers = db.users.ToList<user>().Count();
            Assert.AreEqual(expectedUsers, actualUsers);
            Assert.AreEqual("Create", actual.ViewName); //Directed to correct location
        }

        /// <summary>
        /// Test create duplicate email.
        /// </summary>
        [TestMethod]
        public void CreateUserDbUpdateExceptionEmailTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            UserController target = new UserController();
            user2.email = user1.email;
            int expectedUsers = db.users.ToList<user>().Count();

            var actual = target.Create(user2) as ViewResult;

            // Assert
            db = new touch_for_foodEntities();
            int actualUsers = db.users.ToList<user>().Count();
            Assert.AreEqual(expectedUsers, actualUsers);
            Assert.AreEqual("Create", actual.ViewName); //Directed to correct location
        }

        /// <summary>
        /// Test create with invalid user attributes.
        /// </summary>
        [TestMethod]
        public void CreateUserDBEntityValidationExceptionTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            UserController target = new UserController();
            user2.email = null;
            user2.username = null;
            int expectedUsers = db.users.ToList<user>().Count();
            Session session = new Session(db, target);

            var actual = target.Create(user2) as ViewResult;

            // Assert
            db = new touch_for_foodEntities();
            int actualUsers = db.users.ToList<user>().Count();
            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
            Assert.AreEqual(expectedUsers, actualUsers);
            Assert.AreEqual("Create", actual.ViewName); //Directed to correct location
        }

        /// <summary>
        /// Create method with invalid state.
        /// </summary>
        [TestMethod]
        public void CreateUserWithInvalidStateModelTest()
        {
            // Arrange
            UserController target = new UserController();
            target.ModelState.AddModelError("error", "ModelState is invalid");

            // Act
            var actual = target.Create(user2) as ViewResult;

            // Assert
            string errorMsg = actual.ViewBag.Error;
            Assert.AreEqual("Create", actual.ViewName);
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        /// A test for the edit redirect.
        /// </summary>
        [TestMethod]
        public void EditViewTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            UserController target = new UserController();
            Session session = new Session(db, target);
            session.simulateLogin(user1.username, user1.password);
            int expected = user1.id;

            //Act
            ViewResult actual = (ViewResult)target.Edit(user1.id);

            //Assert
            Assert.AreEqual(((user)actual.Model).id, expected);
        }

        /// <summary>
        /// A test for the edit redirect.
        /// </summary>
        [TestMethod]
        public void EditViewInvalidUserTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            UserController target = new UserController();
            Session session = new Session(db, target);
            session.simulateLogin(user1.username, user1.password);

            //Act
            var actualResult = target.Edit(user3.id) as RedirectToRouteResult;
            var actualResultURI = actualResult.RouteValues["controller"] + "/" + actualResult.RouteValues["action"];

            //Assertions
            Assert.AreEqual("Home/Index", actualResultURI);
        }

        /// <summary>
        /// Edit with valid criteria.
        /// </summary>
        [TestMethod]
        public void EditUserTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            UserController target = new UserController();
            string changeString = "userUnitTest@email.com";
            user1.first_name = changeString;

            //CheckSetup
            Assert.IsFalse(db.users.Find(user1.id).first_name.Equals(changeString, StringComparison.OrdinalIgnoreCase));

            // The edit needs the password to be un-hashed, so we're force it to be un-hashed
            user1.password = "user1UnitTest@email.com";
            user1.ConfirmPassword = "user1UnitTest@email.com";

            //Act
            var actual = target.Edit(user1, null) as RedirectToRouteResult;

            //Assertions
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.users.Find(user1.id).first_name.Equals(changeString, StringComparison.OrdinalIgnoreCase));
            Assert.AreEqual("Index", actual.RouteValues["action"]);
        }

        /// <summary>
        /// Edit with valid criteria.
        /// </summary>
        [TestMethod]
        public void EditUserLockTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            UserController target = new UserController();
            string changeString = "userUnitTest@email.com";
            user1.first_name = changeString;
            user1.version -= 1;

            //Check setup
            Assert.IsFalse(db.users.Find(user1.id).first_name.Equals(changeString, StringComparison.OrdinalIgnoreCase));

            //Act
            var actual = target.Edit(user1, null) as ViewResult;

            //Assertions
            db = new touch_for_foodEntities();
            string errorMsg = actual.ViewBag.Error;
            Assert.IsFalse(db.users.Find(user1.id).first_name.Equals(changeString, StringComparison.OrdinalIgnoreCase));
            Assert.IsNotNull(errorMsg); //error message is sent to view
            
        }

        /// <summary>
        /// A test for the delete redirect.
        /// </summary>
        [TestMethod]
        public void DeleteTest()
        {
            //Arrange
            UserController target = new UserController();
            int expected = user1.id;

            //Act
            ViewResult actual = (ViewResult)target.Delete(user1.id);

            //Assert
            Assert.AreEqual(expected, ((user)actual.ViewData.Model).id);
        }

        /// <summary>
        /// A test for the delete confirmed success
        /// </summary>
        [TestMethod]
        public void DeleteConfirmedTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            UserController target = new UserController();
            int expectedNumberOfUsers = db.users.ToList<user>().Count()-1;

            //Act
            var actual = target.DeleteConfirmed(user1.id) as RedirectToRouteResult;
            var actualResultURI = actual.RouteValues["action"];

            //Assert
            db = new touch_for_foodEntities();
            int actualNumberOfUsers = db.users.ToList<user>().Count();
            Assert.AreEqual(expectedNumberOfUsers, actualNumberOfUsers);
            Assert.AreEqual("Index", actualResultURI);
            Assert.IsNull(db.users.Find(user1.id));
            Assert.IsNull(db.friendships.Find(friendship1.id));
            Assert.IsNull(db.restaurant_user.Find(restaurantUser1.id));
            Assert.IsNull(db.orders.Find(order1.id));
        }

        /// <summary>
        /// A test for logon
        /// </summary>
        [TestMethod]
        public void LogOnViewTest()
        {
            // Arrange
            UserController controller = new UserController();

            // Act
            var result = controller.LogOn() as ViewResult;

            // Assert
            Assert.AreEqual("LogOn", result.ViewName);
        }

        /// <summary>
        /// Invalid model.
        /// </summary>
        [TestMethod]
        public void LogOnWithInvalidStateModelTest()
        {
            // Arrange
            UserController target = new UserController();
            target.ModelState.AddModelError("error", "ModelState is invalid");

            // Act
            var actual = target.LogOn(user2.username, user2.password) as RedirectToRouteResult;
            var resultURI = actual.RouteValues["action"];

            // Assert
            Assert.AreEqual("LogOn", resultURI);

        }

        /// <summary>
        /// A test for logon invalid user
        /// </summary>
        [TestMethod]
        public void LogOnWithInvalidUserTest()
        {
            // Arrange
            UserController target = new UserController();

            // Act
            var actual = target.LogOn(user2.username, "wrongpassword") as ViewResult;

            // Assert
            Assert.AreEqual("LogOn", actual.ViewName);

        }

        /// <summary>
        /// A test for logon
        /// </summary>
        [TestMethod]
        public void LogOnWithValidUserTest()
        {
            // Arrange
            UserController target = new UserController();
            var mock = new ContextMocks(target);

            // Act
            // We use the password in string format since our TestDatabaseHelper uses the encrypted value
            var actualResult = target.LogOn(user1.username, "user1UnitTest@email.com") as RedirectToRouteResult;
            var actualResultURI = actualResult.RouteValues["controller"] + "/" + actualResult.RouteValues["action"];

            // Assertions
            Assert.AreEqual("Home/Index", actualResultURI);
        }
        #endregion
    }
}
