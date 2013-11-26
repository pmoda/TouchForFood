using System;
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
    ///This is a test class for MenuControllerTest and is intended
    ///to contain all MenuControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class MenuControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static MenuController target;
        private static restaurant restaurant1;
        private static menu menu1;
        private static category category1;
        private static menu_category menuCategory1;
        private static user user1;
        private static table table1;
        #endregion

        #region Properties
        /// <summary>
        ///Gets or sets the test context which provides information about and 
        ///functionality for the current test run.
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
            user1 = testDatabase.AddUser("menuUnitTest@email.com", table1, (int)SiteRoles.Admin);
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
        ///Use TestInitialize to run code before running each test 
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //Add test data (order specific)
            menu1 = testDatabase.AddMenu(restaurant1);
            category1 = testDatabase.AddCategory();
            menuCategory1 = testDatabase.AddMenuCategory(category1, menu1);

            //Session
            db = new touch_for_foodEntities();
            target = new MenuController(db);
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
            db = new touch_for_foodEntities();
            foreach (menu_category mc in db.categories.Find(category1.id).menu_category)
            {
                testDatabase.RemoveMenuCategory(mc);
            }            
            testDatabase.RemoveCategory(category1);
            foreach (menu m in db.restaurants.Find(restaurant1.id).menus)
            {
                testDatabase.RemoveMenu(m);
            }
        }
        #endregion

        #region Test Methods
        /// <summary>
        /// Test create view
        /// </summary>
        [TestMethod()]
        public void CreateViewTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            SelectList expectedRestoList = new SelectList(db.restaurants, "id", "name", db.restaurants.First().id);

            //Act
            ViewResult actual = (ViewResult)target.Create();

            //Assert
            Assert.AreEqual(expectedRestoList.GetType(), actual.ViewBag.resto_id.GetType());
            SelectList actualRestoList = actual.ViewBag.resto_id;
            Assert.AreEqual(expectedRestoList.Count(), actualRestoList.Count());
        }

        /// <summary>
        /// Test create method
        /// </summary>
        [TestMethod()]
        public void CreateTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            int numberOfMenusBefore = db.restaurants.Find(restaurant1.id).menus.ToList<menu>().Count();

            //Menu object that will be added.
            menu menu2 = new menu();
            menu2.resto_id = restaurant1.id;
            menu2.name = "UnitTest";
            menu2.is_active = false;
            menu2.is_deleted = false;

            //Act
            var actual = (RedirectToRouteResult)target.Create(menu2);

            //Assert
            db = new touch_for_foodEntities();
            int numberOfMenusAfter = db.restaurants.Find(restaurant1.id).menus.ToList<menu>().Count();
            Assert.AreEqual((numberOfMenusBefore + 1), numberOfMenusAfter);
        }

        ///<summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            SelectList expectedRestoList = new SelectList(db.restaurants, "id", "name", menu1.resto_id);
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Create(menu1);

            // Assert
            SelectList actualRestoList = actual.ViewBag.resto_id;
            Assert.AreEqual(expectedRestoList.GetType(), actualRestoList.GetType());
            Assert.AreEqual(expectedRestoList.Count(), actualRestoList.Count());

            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///Adding a  category to the menu should create a new association object between the two.
        ///</summary>
        [TestMethod()]
        public void AddCategoryTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            int numberOfMenuCategoriesBefore = db.categories.Find(category1.id).menu_category
                .ToList<menu_category>().Count();

            //Act
            target.AddCategory(menu1.id, category1.id);

            //Assert
            db = new touch_for_foodEntities();
            int numberOfMenuCategoriesAfter = db.categories.Find(category1.id).menu_category
                .ToList<menu_category>().Count();

            //only one menu category added
            Assert.AreEqual((numberOfMenuCategoriesBefore + 1), numberOfMenuCategoriesAfter);

            //each menu category has the right menu and category attributes.
            foreach (menu_category mc in db.categories.Find(category1.id).menu_category.ToList<menu_category>())
            {
                Assert.IsTrue(mc.menu_id == menu1.id);
            }
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditViewTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            SelectList expectedRestoList = new SelectList(db.restaurants, "id", "name", menu1.id);

            //Act
            ViewResult actual = (ViewResult)target.Edit(menu1.id);

            //Assert
            Assert.AreEqual(((menu)actual.Model).id, menu1.id);
            Assert.AreEqual(expectedRestoList.GetType(), actual.ViewBag.resto_id.GetType());
            SelectList actualRestoList = actual.ViewBag.resto_id;
            Assert.AreEqual(expectedRestoList.Count(), actualRestoList.Count());
        }

        /// <summary>
        /// Test that edit(menu) works correctly, changeing the values in 
        /// the method
        /// </summary>
        [TestMethod()]
        public void EditTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            string changeString = "UnitTesting";
            menu1.name = changeString;
            
            //Check setup
            Assert.IsFalse(db.menus.Find(menu1.id).name.Equals(changeString));

            //Act
            ActionResult actual = (ActionResult)target.Edit(menu1);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.menus.Find(menu1.id).name.Equals(changeString));
        }

        /// <summary>
        /// Test that edit(menu) doesn't work when version is incorrect.
        /// </summary>
        [TestMethod()]
        public void EditVersionErrorTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            SelectList expectedRestoList = new SelectList(db.restaurants, "id", "name", menu1.id);
            string changeString = "UnitTesting";
            menu1.version += 5;
            menu1.name = changeString;

            //Act
            var actual = target.Edit(menu1) as ViewResult;
            
            //Assert
            db = new touch_for_foodEntities();
            Assert.IsFalse(db.menus.Find(menu1.id).name.Equals(changeString));
            Assert.AreEqual(((menu)actual.Model).id, menu1.id);
            Assert.AreEqual(expectedRestoList.GetType(), actual.ViewBag.resto_id.GetType());
            SelectList actualRestoList = actual.ViewBag.resto_id;
            Assert.AreEqual(expectedRestoList.Count(), actualRestoList.Count());
        }

        /// <summary>
        ///If the menu is not active and doesn't have menu categories associated, 
        ///then the ViewBag must not contain a warning before the view is returned.
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            testDatabase.RemoveMenuCategory(menuCategory1);

            //Act
            ViewResult actual = (ViewResult)target.Delete(menu1.id);

            //Assert
            string actualWarning = actual.ViewBag.Warning;
            Assert.IsTrue(String.IsNullOrWhiteSpace(actualWarning));
            menu actualMenu = (menu)actual.ViewData.Model;
            Assert.AreEqual(actualMenu.id, menu1.id);
        }

        /// <summary>
        ///If the menu has a menu category associate to it, then the ViewBag must contain a 
        ///warning before the view is returned.
        ///</summary>
        [TestMethod()]
        public void DeleteMenuCategoryAssociatedWarningTest()
        {
            //Act
            ViewResult actual = (ViewResult)target.Delete(menu1.id);

            //Assert
            string actualWarning = actual.ViewBag.Warning;
            Assert.IsFalse(String.IsNullOrWhiteSpace(actualWarning));
            menu actualMenu = (menu)actual.ViewData.Model;
            Assert.AreEqual(actualMenu.id, menu1.id);
        }

        /// <summary>
        ///If the menu is active, then the ViewBag must contain a 
        ///warning before the view is returned.
        ///</summary>
        [TestMethod()]
        public void DeleteActiveMenuWarningTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            testDatabase.RemoveMenuCategory(menuCategory1);
            menu1.is_active= true;
            db.Entry(menu1).State = EntityState.Modified;
            db.SaveChanges();

            //Act
            ViewResult actual = (ViewResult)target.Delete(menu1.id);

            //Assert
            string actualWarning = actual.ViewBag.Warning;
            Assert.IsFalse(String.IsNullOrWhiteSpace(actualWarning));
            menu actualMenu = (menu)actual.ViewData.Model;
            Assert.AreEqual(actualMenu.id, menu1.id);
        }

        /// <summary>
        ///Make sure that the menu it's associate menu categories and menu items are now set to is_deleted = true
        ///if the menu is active a ItemActiveException must be thrown
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            item item1 = testDatabase.AddItem();
            menu_item menuItem1 = testDatabase.AddMenuItem(item1, menuCategory1);

            //Check setup
            Assert.IsFalse(db.menus.Find(menu1.id).is_deleted);
            Assert.IsFalse(db.menu_category.Find(menuCategory1.id).is_deleted);
            Assert.IsFalse(db.menu_item.Find(menuItem1.id).is_deleted);

            //Act
            ActionResult actual = (ActionResult) target.DeleteConfirmed(menu1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.menus.Find(menu1.id).is_deleted);
            Assert.IsTrue(db.menu_category.Find(menuCategory1.id).is_deleted);
            Assert.IsTrue(db.menu_item.Find(menuItem1.id).is_deleted);

            //Clean-up (order specific)
            testDatabase.RemoveMenuItem(menuItem1);
            testDatabase.RemoveItem(item1);
        }

        /// <summary>
        /// Test delete confirmed exception; If the menu is active a ItemActiveException must be thrown.
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedActiveExceptionTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            item item1 = testDatabase.AddItem();
            menu_item menuItem1 = testDatabase.AddMenuItem(item1, menuCategory1);
            //make menu active
            menu1.is_active = true;
            db.Entry(menu1).State = EntityState.Modified;
            db.SaveChanges();

            //Act
            ActionResult actual = (ActionResult)target.DeleteConfirmed(menu1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsFalse(db.menus.Find(menu1.id).is_deleted);
            Assert.IsFalse(db.menu_category.Find(menuCategory1.id).is_deleted);
            Assert.IsFalse(db.menu_item.Find(menuItem1.id).is_deleted);

            //Clean-up (order specific)
            testDatabase.RemoveMenuItem(menuItem1);
            testDatabase.RemoveItem(item1);
        }

        /// <summary>
        ///Sets menu category attribute is deleted to true, same goes for the associated menu items.
        ///</summary>
        [TestMethod()]
        public void RemoveMenuCategoryTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            item item1 = testDatabase.AddItem();
            menu_item menuItem1 = testDatabase.AddMenuItem(item1, menuCategory1);
            int expected = 1; //1 row should have been affected

            //Check setup
            Assert.IsFalse(db.menu_category.Find(menuCategory1.id).is_deleted);
            Assert.IsFalse(db.menu_item.Find(menuItem1.id).is_deleted);

            //Act
            int actual = target.RemoveMenuCategory(menuCategory1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(db.menu_category.Find(menuCategory1.id).is_deleted);
            Assert.IsTrue(db.menu_item.Find(menuItem1.id).is_deleted);

            //Clean-up (order specific)
            testDatabase.RemoveMenuItem(menuItem1);
            testDatabase.RemoveItem(item1);
        }

        /// <summary>
        /// Makes sure menu category and subsequent menu items are not set to is_deleted = true
        /// when menu category is set to active.
        ///</summary>
        [TestMethod()]
        public void RemoveMenuCategoryExceptionTest()
        {
            //Arrange
            db = new touch_for_foodEntities();

            //populat db (order specific)
            item item1 = testDatabase.AddItem();
            menu_item menuItem1 = testDatabase.AddMenuItem(item1, menuCategory1);

            //0 rows should have been affected
            int expected = 0; 

            //make menu category active
            menuCategory1.is_active = true;
            db.Entry(menuCategory1).State = EntityState.Modified;
            db.SaveChanges();

            //Act
            int actual = target.RemoveMenuCategory(menuCategory1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.AreEqual(expected, actual);
            Assert.IsFalse(db.menu_category.Find(menuCategory1.id).is_deleted);
            Assert.IsFalse(db.menu_item.Find(menuItem1.id).is_deleted);

            //Clean-up (order specific)
            testDatabase.RemoveMenuItem(menuItem1);
            testDatabase.RemoveItem(item1);
        }

        /// <summary>
        ///A test for Details
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            int expected = menu1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Details(menu1.id);

            //Assert
            Assert.AreEqual(expected, ((menu)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for Order
        ///</summary>
        [TestMethod()]
        public void OrderTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            int expected = menu1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Order(menu1.id);

            //Assert
            Assert.AreEqual(expected, ((menu)actual.ViewData.Model).id);
        }
        #endregion
    }
}
