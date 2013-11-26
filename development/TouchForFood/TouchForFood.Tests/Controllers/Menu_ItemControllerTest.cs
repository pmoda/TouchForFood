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
    ///This is a test class for Menu_ItemControllerTest and is intended
    ///to contain all Menu_ItemControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Menu_ItemControllerTest
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
        private static menu_item menuItem2;
        private static review review1;
        private static review_order_item reviewOrderItem1;
        private static review_order_item reviewOrderItem2;
        private static table table1;
        private static order order1;
        private static user user1;
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

            //Add test data (order specific)
            item1 = testDatabase.AddItem();
            category1 = testDatabase.AddCategory();
            restaurant1 = testDatabase.AddRestaurant();
            menu1 = testDatabase.AddMenu(restaurant1);
            menuCategory1 = testDatabase.AddMenuCategory(category1, menu1);
            table1 = testDatabase.AddTable(restaurant1);
            order1 = testDatabase.AddOrder(table1);
            user1 = testDatabase.AddUser("menuItem1UnitTest@email.com", table1, (int)SiteRoles.Admin);
            review1 = testDatabase.AddReview(restaurant1, order1, user1);
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup() 
        {
            //Remove test data (order specific)
            testDatabase.RemoveReview(review1);
            testDatabase.RemoveUser(user1);
            testDatabase.RemoveOrder(order1);
            testDatabase.RemoveTable(table1);
            testDatabase.RemoveMenuCategory(menuCategory1);
            testDatabase.RemoveMenu(menu1);
            testDatabase.RemoveRestaurant(restaurant1);
            testDatabase.RemoveCategory(category1);
            testDatabase.RemoveItem(item1);
        }

        /// <summary>
        /// Use TestInitialize to run code before running each test
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //Add test data (order specific)
            db = new touch_for_foodEntities();
            menuItem1 = testDatabase.AddMenuItem(item1, menuCategory1);
            orderItem1 = testDatabase.AddOrderItem(order1, menuItem1);
            reviewOrderItem1 = testDatabase.AddReviewOrderItem(review1, orderItem1, "testing1", 2);
            reviewOrderItem2 = testDatabase.AddReviewOrderItem(review1, orderItem1, "testing2", 3);

            //set parameters for menuItem2
            menuItem2 = new menu_item();
            menuItem2.item_id = item1.id;
            menuItem2.menu_category_id = menuCategory1.id;
            menuItem2.price = new decimal(10.99);
            menuItem2.is_active = false;
            menuItem2.is_deleted = false;
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveReviewOrderItem(reviewOrderItem1);
            testDatabase.RemoveReviewOrderItem(reviewOrderItem2);
            testDatabase.RemoveOrderItem(orderItem1);
            testDatabase.RemoveMenuItem(menuItem2);
            testDatabase.RemoveMenuItem(menuItem1);
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
            Menu_ItemController target = new Menu_ItemController();
            SelectList expectedItemList = new SelectList(db.items, "id", "name");
            SelectList expectedCategoryList = new SelectList(db.menu_category, "id", "id");
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Create();

            //Assert
            SelectList actualItemList = actual.ViewBag.item_id;
            Assert.AreEqual(expectedItemList.GetType(), actualItemList.GetType());
            Assert.AreEqual(expectedItemList.Count(), actualItemList.Count());

            SelectList actualCategoryList = actual.ViewBag.menu_category_id;
            Assert.AreEqual(expectedCategoryList.GetType(), actualCategoryList.GetType());
            Assert.AreEqual(expectedCategoryList.Count(), actualCategoryList.Count());
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Menu_ItemController target = new Menu_ItemController();
            int expectedMenuItems = db.menu_item.ToList<menu_item>().Count() + 1;

            // Act
            var actualResult = target.Create(menuItem2) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            int actualMenuItems = db.menu_item.ToList<menu_item>().Count();
            Assert.AreEqual(expectedMenuItems, actualMenuItems);
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
            Menu_ItemController target = new Menu_ItemController();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            SelectList expectedItemList = new SelectList(db.items, "id", "name", menuItem2.item_id);
            SelectList expectedCategoryList = new SelectList(db.menu_category, "id", "id", menuItem2.menu_category_id);
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Create(menuItem2);

            // Assert
            SelectList actualItemList = actual.ViewBag.item_id;
            Assert.AreEqual(expectedItemList.GetType(), actualItemList.GetType());
            Assert.AreEqual(expectedItemList.Count(), actualItemList.Count());

            SelectList actualCategoryList = actual.ViewBag.menu_category_id;
            Assert.AreEqual(expectedCategoryList.GetType(), actualCategoryList.GetType());
            Assert.AreEqual(expectedCategoryList.Count(), actualCategoryList.Count());

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
            Menu_ItemController target = new Menu_ItemController();
            int expected = menuItem1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Delete(menuItem1.id);

            //Assert
            Assert.AreEqual(expected, ((menu_item)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for DeleteConfirmed
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Menu_ItemController target = new Menu_ItemController();

            //CheckSetup
            Assert.IsFalse(db.menu_item.Find(menuItem1.id).is_deleted);

            // Act
            var actualResult = target.DeleteConfirmed(menuItem1.id) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];

            Assert.IsTrue(db.menu_item.Find(menuItem1.id).is_deleted);
            Assert.AreEqual("Index", actualResultURI);
        }

        /// <summary>
        ///A test for DeleteConfirmed
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedTestItemActiveException()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Menu_ItemController target = new Menu_ItemController();
            int expectedMenuItems = db.menu_item.ToList<menu_item>().Count();
            menuItem1 = db.menu_item.Find(menuItem1.id);
            menuItem1.is_active = true;
            db.Entry(menuItem1).State = EntityState.Modified;
            db.SaveChanges();

            // Act
            var actualResult = target.DeleteConfirmed(menuItem1.id) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            int actualMenuItems = db.menu_item.ToList<menu_item>().Count();

            Assert.AreEqual(expectedMenuItems, actualMenuItems);
            Assert.IsFalse(db.menu_item.Find(menuItem1.id).is_deleted);
            Assert.AreEqual("Index", actualResultURI);
        }

        /// <summary>
        ///A test for Details
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            Menu_ItemController target = new Menu_ItemController();
            int expected = menuItem1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Details(menuItem1.id);

            //Assert
            Assert.AreEqual(expected, ((menu_item)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditViewTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Menu_ItemController target = new Menu_ItemController();
            int expected = menuItem1.id;
            SelectList expectedItemList = new SelectList(db.items, "id", "name", menuItem1.item_id);
            SelectList expectedCategoryList = new SelectList(db.menu_category, "id", "id", menuItem1.menu_category_id);
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Edit(menuItem1.id);

            // Assert
            Assert.AreEqual(expected, ((menu_item)actual.ViewData.Model).id);

            SelectList actualItemList = actual.ViewBag.item_id;
            Assert.AreEqual(expectedItemList.GetType(), actualItemList.GetType());
            Assert.AreEqual(expectedItemList.Count(), actualItemList.Count());

            SelectList actualCategoryList = actual.ViewBag.menu_category_id;
            Assert.AreEqual(expectedCategoryList.GetType(), actualCategoryList.GetType());
            Assert.AreEqual(expectedCategoryList.Count(), actualCategoryList.Count());
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Menu_ItemController target = new Menu_ItemController();
            int expectedVersion = menuItem1.version + 1;
            decimal changePrice = new decimal(9.99);
            menuItem1.price = changePrice;

            //Check Setup
            Assert.IsFalse(db.menu_item.Find(menuItem1.id).price == changePrice);

            // Act
            var actualResult = target.Edit(menuItem1) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            Assert.AreEqual(db.menu_item.Find(menuItem1.id).version, expectedVersion); //version was incremented
            Assert.IsTrue(db.menu_item.Find(menuItem1.id).price == changePrice); //price was changed
            Assert.AreEqual("Index", actualResultURI); //directed to correct location
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTestLockFailure()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Menu_ItemController target = new Menu_ItemController();
            int expected = menuItem1.id;
            SelectList expectedItemList = new SelectList(db.items, "id", "name", menuItem1.item_id);
            SelectList expectedCategoryList = new SelectList(db.menu_category, "id", "id", menuItem1.menu_category_id);
            ViewResult actual;
            menuItem1.version += 5;

            // Act
            actual = (ViewResult)target.Edit(menuItem1);

            // Assert
            Assert.AreEqual(expected, ((menu_item)actual.ViewData.Model).id);

            SelectList actualItemList = actual.ViewBag.item_id;
            Assert.AreEqual(expectedItemList.GetType(), actualItemList.GetType());
            Assert.AreEqual(expectedItemList.Count(), actualItemList.Count());

            SelectList actualCategoryList = actual.ViewBag.menu_category_id;
            Assert.AreEqual(expectedCategoryList.GetType(), actualCategoryList.GetType());
            Assert.AreEqual(expectedCategoryList.Count(), actualCategoryList.Count());

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
            Menu_ItemController target = new Menu_ItemController();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            int expected = menuItem1.id;
            SelectList expectedItemList = new SelectList(db.items, "id", "name", menuItem1.item_id);
            SelectList expectedCategoryList = new SelectList(db.menu_category, "id", "id", menuItem1.menu_category_id);
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Edit(menuItem1);

            // Assert
            Assert.AreEqual(expected, ((menu_item)actual.ViewData.Model).id);

            SelectList actualItemList = actual.ViewBag.item_id;
            Assert.AreEqual(expectedItemList.GetType(), actualItemList.GetType());
            Assert.AreEqual(expectedItemList.Count(), actualItemList.Count());

            SelectList actualCategoryList = actual.ViewBag.menu_category_id;
            Assert.AreEqual(expectedCategoryList.GetType(), actualCategoryList.GetType());
            Assert.AreEqual(expectedCategoryList.Count(), actualCategoryList.Count());

            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for GetAllReviews
        ///</summary>
        [TestMethod()]
        public void GetAllReviewsTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_ItemController target = new Menu_ItemController();
            PartialViewResult actual;
            List<review_order_item> expectedReviews = db.menu_item.Find(menuItem1.id)
                                                        .order_item
                                                        .SelectMany(oi => oi.review_order_item)
                                                        .OrderByDescending(r => r.submitted_on)
                                                        .ToList<review_order_item>();
            //Act
            actual = target.GetAllReviews(menuItem1);

            //Assert
            List<review_order_item> actualReviews = actual.Model as List<review_order_item>;
            Assert.AreEqual(expectedReviews.Count(), actualReviews.Count());
            Assert.AreEqual("_PastReviewsPartial", actual.ViewName);
        }

        /// <summary>
        ///A test for Index
        ///</summary>
        [TestMethod()]
        public void IndexTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_ItemController target = new Menu_ItemController();
            List<menu_item> expectedMenuItems = db.menu_item.Where(mi => mi.is_active == true && mi.is_deleted == false).ToList();
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Index();

            //Assert
            List<menu_item> actualMenuItems = actual.Model as List<menu_item>;
            Assert.AreEqual(expectedMenuItems.Count(), actualMenuItems.Count());
        }

        /// <summary>
        ///A test for OnTheMenu
        ///</summary>
        [TestMethod()]
        public void OnTheMenuTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_ItemController target = new Menu_ItemController();
            List<menu_item> expectedMenuItems = db.menu_item.Where(mi => mi.menu_category_id == menuCategory1.id && mi.is_deleted == false).ToList();
            PartialViewResult actual;

            //Act
            actual = (PartialViewResult)target.OnTheMenu(menuCategory1.id);

            //Assert
            List<menu_item> actualMenuItems = actual.Model as List<menu_item>;
            Assert.AreEqual(expectedMenuItems.Count(), actualMenuItems.Count());
            Assert.AreEqual("_OnTheMenu", actual.ViewName);
        }

        /// <summary>
        ///A test for PartialDetails
        ///</summary>
        [TestMethod()]
        public void PartialDetailsTest()
        {
            //Arrange
            Menu_ItemController target = new Menu_ItemController();
            int expected = menuItem1.id;
            PartialViewResult actual;

            //Act
            actual = (PartialViewResult)target.PartialDetails(menuItem1.id);

            //Assert
            Assert.AreEqual(expected, ((menu_item)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for PickSides
        ///</summary>
        [TestMethod()]
        public void PickSidesTest()
        {
            //Arrange
            Menu_ItemController target = new Menu_ItemController();
            int expected = menuItem1.id;
            PartialViewResult actual;

            //Act
            actual = (PartialViewResult)target.PickSides(menuItem1.id);

            //Assert
            Assert.AreEqual(expected, ((menu_item)actual.ViewData.Model).id);
        }
        #endregion
    }
}
