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
    /// <summary>
    /// NOTE: The friendship controller class does not use the locking 
    /// mechanism or mappers. If the friendship functionality is used
    /// these tests must be updated. In addition, multiple friendships
    /// between two users should be prohibited.
    /// 
    ///This is a test class for FriendshipControllerTest and is intended
    ///to contain all FriendshipControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class FriendshipControllerTest
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
        private static friendship friendship2;
        #endregion

        #region Properties
        /// <summary>
        ///Gets or sets the test context which provides information 
        ///about and functionality for the current test run.
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

        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup() 
        {
            //Remove test data (order specific)
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
            user1 = testDatabase.AddUser("friendship1UnitTest@email.com", table1, (int)SiteRoles.Admin);
            user2 = testDatabase.AddUser("friendship2UnitTest@email.com", table1, (int)SiteRoles.Admin);
            user3 = testDatabase.AddUser("friendship3UnitTest@email.com", table1, (int)SiteRoles.Admin);
            friendship1 = testDatabase.AddFriendship(user1, user2);
            friendship2 = new friendship();
            friendship2.first_user = user1.id;
            friendship2.second_user = user3.id;
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveFriendship(friendship1);
            testDatabase.RemoveFriendship(friendship2);
            testDatabase.RemoveUser(user3);
            testDatabase.RemoveUser(user2);
            testDatabase.RemoveUser(user1);
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
            FriendshipController target = new FriendshipController();
            SelectList expectedUserList = new SelectList(db.users, "id", "username");

            //Act
            ViewResult actual = (ViewResult)target.Create();

            //Assert
            SelectList actualUser1List = actual.ViewBag.first_user;
            SelectList actualUser2List = actual.ViewBag.second_user;
            Assert.AreEqual(expectedUserList.GetType(), actualUser1List.GetType());
            Assert.AreEqual(expectedUserList.GetType(), actualUser2List.GetType());
            Assert.AreEqual(expectedUserList.Count(), actualUser1List.Count());
            Assert.AreEqual(expectedUserList.Count(), actualUser2List.Count());
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            FriendshipController target = new FriendshipController();
            int expectedFriendships = db.friendships.ToList<friendship>().Count()+1;

            // Act
            var actualResult = target.Create(friendship2) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            int actualFriendships = db.friendships.ToList<friendship>().Count();
            Assert.AreEqual(expectedFriendships, actualFriendships);
            Assert.AreEqual("Index", actualResultURI);
        }

        /// <summary>
        ///A test for Create when the model state is invalid.
        ///</summary>
        [TestMethod()]
        public void CreateTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            FriendshipController target = new FriendshipController();
            long expected = friendship1.id;
            SelectList expectedUserList = new SelectList(db.users, "id", "username");
            target.ModelState.AddModelError("error", "ModelState is invalid");

            // Act
            ViewResult actual = (ViewResult)target.Create(friendship2);

            // Assert
            SelectList actualUser1List = actual.ViewBag.first_user;
            SelectList actualUser2List = actual.ViewBag.second_user;
            string errorMsg = actual.ViewBag.Error;
            Assert.AreEqual("Create", actual.ViewName);
            Assert.AreEqual(expectedUserList.GetType(), actualUser1List.GetType());
            Assert.AreEqual(expectedUserList.GetType(), actualUser2List.GetType());
            Assert.AreEqual(expectedUserList.Count(), actualUser1List.Count());
            Assert.AreEqual(expectedUserList.Count(), actualUser2List.Count());
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Arrange
            FriendshipController target = new FriendshipController();
            long expected = friendship1.id;

            //Act
            ViewResult actual = (ViewResult)target.Delete(friendship1.id);

            //Assert
            Assert.AreEqual(expected, ((friendship)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for DeleteConfirmed
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            FriendshipController target = new FriendshipController();
            int expectedFriendships = db.friendships.ToList<friendship>().Count() - 1;

            //Check Setup
            Assert.IsNotNull(db.friendships.Find(friendship1.id));

            //Act
            var actualResult = target.DeleteConfirmed(friendship1.id) as RedirectToRouteResult;

            //Assert
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            int actualFriendships = db.friendships.ToList<friendship>().Count();
            Assert.AreEqual(expectedFriendships, actualFriendships);
            Assert.AreEqual("Index", actualResultURI);
            Assert.IsNull(db.friendships.Find(friendship1.id));
        }

        /// <summary>
        ///A test for Details
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            FriendshipController target = new FriendshipController();
            long expected = friendship1.id;

            //Act
            ViewResult actual = (ViewResult)target.Details(friendship1.id);

            //Assert
            Assert.AreEqual(expected, ((friendship)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditViewTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            FriendshipController target = new FriendshipController();
            long expected = friendship1.id;
            SelectList expectedUser1List = new SelectList(db.users, "id", "username", user1);
            SelectList expectedUser2List = new SelectList(db.users, "id", "username", user2);

            //Act
            ViewResult actual = (ViewResult)target.Edit(friendship1.id);

            //Assert
            SelectList actualUser1List = actual.ViewBag.first_user;
            SelectList actualUser2List = actual.ViewBag.second_user;
            Assert.AreEqual(((friendship)actual.Model).id, expected);
            Assert.AreEqual(expectedUser1List.GetType(), actualUser1List.GetType());
            Assert.AreEqual(expectedUser2List.GetType(), actualUser2List.GetType());
            Assert.AreEqual(expectedUser1List.Count(), actualUser1List.Count());
            Assert.AreEqual(expectedUser2List.Count(), actualUser2List.Count());
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTest()
        {
            //Arrange
            FriendshipController target = new FriendshipController();
            friendship1.second_user = user3.id;

            //Act
            var actualResult = target.Edit(friendship1) as RedirectToRouteResult;

            //Assert
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            Assert.AreEqual("Index", actualResultURI);
            Assert.AreEqual(db.friendships.Find(friendship1.id).second_user, user3.id);
        }

        /// <summary>
        ///A test for Edit when the model state is invalid.
        ///</summary>
        [TestMethod()]
        public void EditTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            FriendshipController target = new FriendshipController();
            long expected = friendship1.id;
            SelectList expectedUser1List = new SelectList(db.users, "id", "username", user1);
            SelectList expectedUser2List = new SelectList(db.users, "id", "username", user2);
            friendship1.second_user = user3.id;
            target.ModelState.AddModelError("error", "ModelState is invalid");

            // Act
            ViewResult actual = (ViewResult)target.Edit(friendship1);

            // Assert
            SelectList actualUser1List = actual.ViewBag.first_user;
            SelectList actualUser2List = actual.ViewBag.second_user;
            string errorMsg = actual.ViewBag.Error;
            Assert.AreEqual(((friendship)actual.Model).id, expected);
            Assert.AreEqual(expectedUser1List.GetType(), actualUser1List.GetType());
            Assert.AreEqual(expectedUser2List.GetType(), actualUser2List.GetType());
            Assert.AreEqual(expectedUser1List.Count(), actualUser1List.Count());
            Assert.AreEqual(expectedUser2List.Count(), actualUser2List.Count());
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for Index
        ///</summary>
        [TestMethod()]
        public void IndexTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            FriendshipController target = new FriendshipController();
            List<friendship> expectedFriendships = db.friendships.ToList<friendship>();

            // Act
            ViewResult actual = (ViewResult)target.Index();

            // Assert
            List<friendship> actualFriendships = actual.Model as List<friendship>;
            Assert.AreEqual(actualFriendships.Count(), expectedFriendships.Count());
        }
        #endregion
    }
}
