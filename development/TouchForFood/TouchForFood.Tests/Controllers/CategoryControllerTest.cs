using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Controllers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;

namespace TouchForFood.Tests
{
    /// <summary>
    ///This is a test class for CategoryControllerTest and is intended
    ///to contain all CategoryControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CategoryControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static category category1;
        private static category category2;
        private static category category3;
        private static menu_category menuCategory1;
        private static restaurant restaurant1;
        private static menu menu1;
        private static CategoryController target;
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
            menu1 = testDatabase.AddMenu(restaurant1);
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveMenu(menu1);
            testDatabase.RemoveRestaurant(restaurant1);
        }

        /// <summary>
        /// Use TestInitialize to run code before running each test
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //Add test data (order specific)
            category1 = testDatabase.AddCategory();
            category2 = testDatabase.AddCategory();
            category3 = new category();
            category3.name = "CategoryUnitTest";
            category3.version = 1;
            menuCategory1 = testDatabase.AddMenuCategory(category2, menu1);

            //Session
            target = new CategoryController();
            Session session = new Session(db, target);
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveMenuCategory(menuCategory1);
            testDatabase.RemoveCategory(category3);
            testDatabase.RemoveCategory(category2);
            testDatabase.RemoveCategory(category1);
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
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Create();

            //Assert
            Assert.IsTrue((category)actual.ViewData.Model == null);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            RedirectToRouteResult actual;

            // Act
            actual = (RedirectToRouteResult)target.Create(category3);

            // Assert
            db = new touch_for_foodEntities();
            var actualResultURI = actual.RouteValues["action"];
            Assert.AreEqual("Index", actualResultURI);
            Assert.IsNotNull(db.categories.Find(category3.id));

        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Create(category3);

            // Assert
            Assert.AreEqual(category3.name, ((category)actual.ViewData.Model).name);
            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTestInvalidCategory()
        {
            // Arrange
            db = new touch_for_foodEntities();
            ViewResult actual;
            category3.name = "";

            // Act
            actual = (ViewResult)target.Create(category3);

            // Assert
            Assert.AreEqual(category3.version, ((category)actual.ViewData.Model).version);
            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for CreatePartial
        ///</summary>
        [TestMethod()]
        public void CreatePartialTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            PartialViewResult actual;

            //Act
            actual = (PartialViewResult)target.CreatePartial(menu1.id);

            //Assert
            Assert.AreEqual("_CategoryCreate", actual.ViewName);
        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Arrange
            int expected = category1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Delete(category1.id);

            //Assert
            Assert.AreEqual(expected, ((category)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for DeleteConfirmed
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            RedirectToRouteResult actual;

            //Check Setup
            Assert.IsNotNull(db.categories.Find(category1.id));

            // Act
            actual = (RedirectToRouteResult)target.DeleteConfirmed(category1.id);

            // Assert
            db = new touch_for_foodEntities();
            var actualResultURI = actual.RouteValues["action"];
            Assert.AreEqual("Index", actualResultURI);
            Assert.IsNull(db.categories.Find(category1.id));
        }

        ///A test for DeleteConfirmed
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedTestAssociationExistsException()
        {
            // Arrange
            db = new touch_for_foodEntities();
            RedirectToRouteResult actual;

            // Act
            actual = (RedirectToRouteResult)target.DeleteConfirmed(category2.id);

            // Assert
            db = new touch_for_foodEntities();
            var actualResultURI = actual.RouteValues["action"];
            Assert.AreEqual("Index", actualResultURI);
            Assert.IsNotNull(db.categories.Find(category2.id));
        }

        /// <summary>
        ///A test for Details
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            int expected = category1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Details(category1.id);

            //Assert
            Assert.AreEqual(expected, ((category)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditViewTest()
        {
            //Arrange
            int expected = category1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Edit(category1.id);

            //Assert
            Assert.AreEqual(expected, ((category)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            int expectedVersion = category1.version + 1;
            string changeString ="Test";
            category1.name = changeString;

            //Check Setup
            Assert.IsFalse(db.categories.Find(category1.id).name == changeString);

            // Act
            var actualResult = target.Edit(category1) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            Assert.AreEqual(db.categories.Find(category1.id).version, expectedVersion); //version was incremented
            Assert.IsTrue(db.categories.Find(category1.id).name == changeString); //price was changed
            Assert.AreEqual("Details", actualResultURI); //directed to correct location
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTestLockError()
        {
            // Arrange
            db = new touch_for_foodEntities();
            ViewResult actual;
            category1.version += 5;

            // Act
            actual = (ViewResult)target.Edit(category1);

            // Assert
            Assert.AreEqual(category1.id, ((category)actual.ViewData.Model).id);
            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTestInvalidCategory()
        {
            // Arrange
            db = new touch_for_foodEntities();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Edit(category1);

            // Assert
            Assert.AreEqual(category1.id, ((category)actual.ViewData.Model).id);
            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for FilterCategories
        ///</summary>
        [TestMethod()]
        public void FilterCategoriesTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            PartialViewResult actual;

            //Act
            actual = (PartialViewResult)target.FilterCategories(menu1);

            //Assert
            Assert.AreEqual("_CategoryTable", actual.ViewName);
        }

        /// <summary>
        ///A test for Index
        ///</summary>
        [TestMethod()]
        public void IndexTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            List<category> expectedCategories = db.categories.ToList();
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Index();

            //Assert
            List<category> actualCategories = actual.Model as List<category>;
            Assert.AreEqual(expectedCategories.Count(), actualCategories.Count());
        }
        #endregion
    }
}
