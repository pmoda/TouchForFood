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
    /// <summary>
    ///This is a test class for ItemControllerTest and is intended
    ///to contain all ItemControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ItemControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static ItemController target;
        private static restaurant restaurant1;
        private static table table1;
        private static user user1;
        private static category category1;
        private static item item1;
        private static menu_category menuCategory1;
        private static menu menu1;
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
            user1 = testDatabase.AddUser("itemUnitTest@email.com", table1, (int)SiteRoles.Admin);
            category1 = testDatabase.AddCategory(); 
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveCategory(category1);
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
            item1 = testDatabase.AddItem(category1);
            menu1 = testDatabase.AddMenu(restaurant1);
            menuCategory1 = testDatabase.AddMenuCategory(category1, menu1);

            //Session
            db = new touch_for_foodEntities();
            target = new ItemController(db);
            Session session = new Session(db, target);
            session.simulateLogin(user1.username, user1.password);
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            db = new touch_for_foodEntities();

            //Remove test data (order specific)
            testDatabase.RemoveMenuCategory(menuCategory1);
            testDatabase.RemoveMenu(menu1);
            foreach (item i in db.categories.Find(category1.id).items)
            {
                testDatabase.RemoveItem(i);
            }
        }

        #endregion

        #region Test Methods
        /// <summary>
        ///Create partial returns an ItemFilterVM and calls the view _ItemCreate
        ///</summary>
        [TestMethod()]
        public void CreatePartialTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            int cat_id = db.menu_category.Where(mc => mc.category_id == item1.category_id).First().id;
            PartialViewResult actual;
            ItemFilterVM iFilter = new ItemFilterVM();

            //Act
            iFilter.menu_cat = db.menu_category.Find(cat_id);
            iFilter.addItem(new item());
            actual = (PartialViewResult)target.CreatePartial(cat_id);

            //Assert
            Assert.AreEqual("_ItemCreate", actual.ViewName);
            Assert.AreEqual(cat_id, ((ItemFilterVM)actual.Model).menu_cat.id);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateViewTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            SelectList expectedCategoryList = new SelectList(db.categories, "id", "name");
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Create();

            //Assert
            SelectList actualCategoryList = actual.ViewBag.category_id;
            Assert.AreEqual(expectedCategoryList.GetType(), actualCategoryList.GetType());
            Assert.AreEqual(expectedCategoryList.Count(), actualCategoryList.Count());
        }

        ///<summary>
        ///A test for Edit.
        ///</summary>
        [TestMethod()]
        public void EditConfirmedLockTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Assert.IsFalse(db.items.Find(item1.id).name.ToString().Equals("UnitTesting", StringComparison.OrdinalIgnoreCase));
            item1.version = 3;

            //Act
            ViewResult actual = (ViewResult)target.Edit(item1);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(((item)actual.Model).id == item1.id);
            Assert.IsTrue(actual.ViewBag.Error != null);
        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Act
            ViewResult actual = (ViewResult)target.Delete(item1.id);

            //Assert
            Assert.AreEqual(((item)actual.Model).id, item1.id);
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTest()
        {
            //Act
            ViewResult actual = (ViewResult)target.Edit(item1.id);

            //Assert
            Assert.AreEqual(((item)actual.Model).id, item1.id);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateItemTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            int numberOfTotalItemsBefore = db.items.ToList<item>().Count();
            item item2 = new item();
            item2.category_id = category1.id;

            //Act
            ActionResult actual = (ActionResult)target.Create(item2);

            //Assert
            db = new touch_for_foodEntities();
            int numberOfTotalItemsAfter = db.items.ToList<item>().Count();
            Assert.IsTrue((numberOfTotalItemsBefore + 1) == numberOfTotalItemsAfter);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedItemTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            int numberOfTotalItemsBefore = db.items.ToList<item>().Count();

            //Act
            ActionResult actual = (ActionResult)target.DeleteConfirmed(item1.id);

            //Assert
            db = new touch_for_foodEntities();
            int numberOfTotalItemsAfter = db.items.ToList<item>().Count();
            Assert.IsTrue((numberOfTotalItemsBefore - 1) == numberOfTotalItemsAfter);
        }

        /// <summary>
        ///A test for FilterItems
        ///</summary>
        [TestMethod()]
        public void FilterItemsTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            menu_category menu_cat = db.items.Find(item1.id).category.menu_category.First();

            //Act
            PartialViewResult actual = target.FilterItems(menu_cat);

            //Assert
            Assert.AreEqual("_ItemTable", actual.ViewName);
        }

        /// <summary>
        ///A test for addItemToMenu. Given an ItemFilterVM a menu category ID and a price this method will create an association
        ///between an item and the menu category it will go under. The item does not exist, so that also needs to be created
        ///</summary>
        [TestMethod()]
        public void addItemToMenuTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Decimal price = new Decimal(9.99);
            bool expected = true;
            ItemFilterVM i_filter = new ItemFilterVM();
            i_filter.menu_cat = menuCategory1;
            i_filter.addItem(item1);

            //Act
            bool actual = target.addItemToMenu(menuCategory1.id, i_filter, price);

            //Assert           
            Assert.AreEqual(expected, actual);

            //Cleanup
            db = new touch_for_foodEntities();
            List<menu_item> menuItems = db.menu_category.Find(menuCategory1.id).menu_item.ToList<menu_item>();
            foreach (menu_item m in menuItems)
            {
                testDatabase.RemoveMenuItem(m);
            }
        }

        /// <summary>
        ///A test for Details
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            int expected = item1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Details(item1.id);

            //Assert
            Assert.AreEqual(expected, ((item)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for View
        ///</summary>
        [TestMethod()]
        public void ViewTest()
        {
            //Arrange
            int expected = item1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.View(item1.id);

            //Assert
            Assert.AreEqual(expected, ((item)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for Index
        ///</summary>
        [TestMethod()]
        public void IndexTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            List<item> expectedItems = db.items.ToList<item>();
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Index();

            //Assert
            List<item> actualItems = actual.Model as List<item>;
            Assert.AreEqual(expectedItems.Count(), actualItems.Count());
        }
        #endregion
    }
}

