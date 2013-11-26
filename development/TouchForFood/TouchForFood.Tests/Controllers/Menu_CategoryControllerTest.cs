using System.Data;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Controllers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;
using System.Collections.Generic;

namespace TouchForFood.Tests
{   
    /// <summary>
    ///This is a test class for Menu_CategoryControllerTest and is intended
    ///to contain all Menu_CategoryControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Menu_CategoryControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static restaurant restaurant1;
        private static menu menu1;
        private static category category1;
        private static item item1;
        private static menu_category menuCategory1;
        private static menu_item menuItem1;
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
        /// <param name="testContext"></param>
        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            //Add test data (order specific)
            testDatabase = new TestDatabaseHelper();
            restaurant1 = testDatabase.AddRestaurant();
            menu1 = testDatabase.AddMenu(restaurant1);
            category1 = testDatabase.AddCategory();            
            item1 = testDatabase.AddItem();         
        }
               
        /// <summary>
        ///Use ClassCleanup to run code after all tests in a class have run 
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveItem(item1);
            testDatabase.RemoveCategory(category1);
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
            menuCategory1 = testDatabase.AddMenuCategory(category1, menu1);
            menuItem1 = testDatabase.AddMenuItem(item1, menuCategory1);
        }
        
        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            db = new touch_for_foodEntities();

            //Remove test data (order specific)
            foreach (menu_category mc in db.categories.Find(category1.id).menu_category)
            {
                foreach (menu_item mi in db.menu_category.Find(mc.id).menu_item)
                {
                    testDatabase.RemoveMenuItem(mi);
                }
                testDatabase.RemoveMenuCategory(mc);
            }
        }        
        #endregion

        #region Test Methods
        /// <summary>
        ///Given a menu category id, an item id and a price, this method create a menu item and adds it to the menu category.
        ///Then redirects to menu category details page passing in the menu category that was specified by the user.
        ///</summary>
        [TestMethod()]
        public void AddItemTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);
            int NumberOfMenuItemsBefore = db.menu_item.ToList<menu_item>().Count();
 
            //Act
            var actual = (RedirectToRouteResult)target.AddItem(menuCategory1.id, item1.id, (decimal)menuItem1.price);
            var actualResultURI = actual.RouteValues["action"];
            
            //Assert
            db = new touch_for_foodEntities();
            int numberOfMenuItemsAfter = db.menu_item.ToList<menu_item>().Count();          
            //menu item was created
            Assert.AreEqual((NumberOfMenuItemsBefore + 1), numberOfMenuItemsAfter);
            //navigating to details page
            Assert.AreEqual("Details", actualResultURI);
        }

        /// <summary>
        ///When creating a menu category, a select list of category id and menu id need to be provided.
        ///</summary>
        [TestMethod()]
        public void CreateViewTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);
            SelectList mcList = new SelectList(db.categories, "id", "name");
            SelectList menuList = new SelectList(db.menus, "id", "name");

            //Act
            var actual = (ViewResult)target.Create();

            //Assert         
            Assert.AreEqual(mcList.GetType(), actual.ViewBag.category_id.GetType());
            Assert.AreEqual(menuList.GetType(), actual.ViewBag.menu_id.GetType());
        }

        /// <summary>
        ///Test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);
            int numberOfMenuCategoriesBefore = db.categories.Find(category1.id).menu_category.ToList<menu_category>().Count();
            menu_category menuCategory2 = new menu_category();
            menuCategory2.is_active = false;
            menuCategory2.category_id = category1.id;
            menuCategory2.menu_id = menu1.id;

            //Act
            target.Create(menuCategory2);

            //Assert
            db = new touch_for_foodEntities();
            int numberOfMenuCategoriesAfter = db.categories.Find(category1.id).menu_category.ToList<menu_category>().Count();
            Assert.AreEqual((numberOfMenuCategoriesBefore + 1), numberOfMenuCategoriesAfter);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);
            target.ModelState.AddModelError("error", "ModelState is invalid");
            SelectList expectedMenuList = new SelectList(db.menus, "id", "name", menuCategory1.menu_id);
            SelectList expectedCategoryList = new SelectList(db.categories, "id", "name", menuCategory1.category_id);
            
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Create(menuCategory1);

            // Assert
            SelectList actualMenuList = actual.ViewBag.menu_id;
            Assert.AreEqual(expectedMenuList.GetType(), actualMenuList.GetType());
            Assert.AreEqual(expectedMenuList.Count(), actualMenuList.Count());

            SelectList actualCategoryList = actual.ViewBag.category_id;
            Assert.AreEqual(expectedCategoryList.GetType(), actualCategoryList.GetType());
            Assert.AreEqual(expectedCategoryList.Count(), actualCategoryList.Count());

            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        /// When editing a menu category, a select list of category id and menu id need to be provided.
        ///</summary>
        [TestMethod()]
        public void EditViewTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);
            SelectList mcList = new SelectList(db.categories, "id", "name", menuCategory1.category_id);
            SelectList menuList = new SelectList(db.menus, "id", "name", menuCategory1.menu_id);

            //Act
            var actual = (ViewResult)target.Edit(menuCategory1.id);

            //Assert         
            Assert.AreEqual(mcList.GetType(), actual.ViewBag.category_id.GetType());
            Assert.AreEqual(menuList.GetType(), actual.ViewBag.menu_id.GetType());
        }

        /// <summary>
        /// The changes made to the item should be applied.
        ///</summary>
        [TestMethod()]
        public void EditTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);
            Assert.IsFalse(db.menu_category.Find(menuCategory1.id).is_active);
            menuCategory1.is_active = true;

            //Act
            RedirectToRouteResult actual = (RedirectToRouteResult)target.Edit(menuCategory1);

            //Assert    
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.menu_category.Find(menuCategory1.id).is_active);
        }

        /// <summary>
        /// Version error in edit. Should send error to display.
        ///</summary>
        [TestMethod()]
        public void EditVersionErrorTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);
            SelectList mcList = new SelectList(db.categories, "id", "name", menuCategory1.category_id);
            SelectList menuList = new SelectList(db.menus, "id", "name", menuCategory1.menu_id);
            Assert.IsFalse(db.menu_category.Find(menuCategory1.id).is_active);
            menuCategory1.is_active = true;
            menuCategory1.version -= 1;

            //Act
            var actual = (ViewResult)target.Edit(menuCategory1);

            //Assert    
            db = new touch_for_foodEntities();
            Assert.IsFalse(db.menu_category.Find(menuCategory1.id).is_active);
            Assert.AreEqual(mcList.GetType(), actual.ViewBag.category_id.GetType());
            Assert.AreEqual(menuList.GetType(), actual.ViewBag.menu_id.GetType());
            Assert.IsNotNull(actual.ViewBag.Error);
        }

        /// <summary>
        ///To delete a menu category from the DB. If the menu category is active
        ///a warning message must be passed.
        ///</summary>
        [TestMethod()]
        public void DeleteActiveWarningTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);           
            menuCategory1.is_active = true;
            db.Entry(menuCategory1).State = EntityState.Modified;
            db.SaveChanges();
            menu_category expected = menuCategory1;

            //Act
            ViewResult actual = (ViewResult)target.Delete(menuCategory1.id);

            //Assert          
            Assert.AreEqual(expected.id, ((menu_category)actual.ViewData.Model).id);
            Assert.IsNotNull(actual.ViewBag.Warning);
        }

        /// <summary>
        ///To delete a menu category from the DB. If there is a menu item associated to the menu category
        ///a warning message must be passed.
        ///</summary>
        [TestMethod()]
        public void DeleteMenuItemExistWarningTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);
            menu_category expected = menuCategory1;

            //Act
            ViewResult actual = (ViewResult)target.Delete(menuCategory1.id);

            //Assert          
            Assert.AreEqual(expected.id, ((menu_category)actual.ViewData.Model).id);
            Assert.IsNotNull(actual.ViewBag.Warning);
        }

        /// <summary>
        ///Given the id of a menu item, disassociate it from the menu
        ///</summary>
        [TestMethod()]
        public void RemoveMenuItemTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);
            int expected = 1;//# of rows affected by this

            //Act
            int actual = target.RemoveMenuItem(menuItem1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(db.menu_item.Find(menuItem1.id).is_deleted);
        }

        /// <summary>
        ///Once the deletion has been confirmed, we must make sure that the menu category has been deleted
        ///and any associated menu items are also deleted.
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);

            //Act
            RedirectToRouteResult actual = (RedirectToRouteResult)target.DeleteConfirmed(menuCategory1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.AreEqual("Index", actual.RouteValues["action"]);
            Assert.IsTrue(db.menu_category.Find(menuCategory1.id).is_deleted);
            foreach (menu_item mi in db.menu_category.Find(menuCategory1.id).menu_item.ToList())
            {
                Assert.IsTrue(mi.is_deleted);
            }
        }

        /// <summary>
        ///Deletes a menu category from the menu. If there are menu items associated to that menu category then they are deleted
        ///as well.
        ///</summary>
        [TestMethod()]
        public void AjaxDeleteTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);
            int expected = menuCategory1.id;

            //Act
            int actual = target.AjaxDelete(menuCategory1.id);

            //Assert      
            Assert.AreEqual(expected, actual);
            Assert.IsTrue(db.menu_category.Find(menuCategory1.id).is_deleted);
        }

        /// <summary>
        ///A test for Index
        ///</summary>
        [TestMethod()]
        public void IndexTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);
            List<menu_category> expectedMenuCategories = db.menu_category.Where(mc => mc.is_active == true && mc.is_deleted == false).ToList();
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Index();

            //Assert
            List<menu_category> actualMenuCategories = actual.Model as List<menu_category>;
            Assert.AreEqual(expectedMenuCategories.Count(), actualMenuCategories.Count());
        }

        /// <summary>
        ///A test for Details
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            Menu_CategoryController target = new Menu_CategoryController(db);
            int expected = menuCategory1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Details(menuCategory1.id);

            //Assert
            Assert.AreEqual(expected, ((menu_category)actual.ViewData.Model).id);
        }
        #endregion
    }
}
