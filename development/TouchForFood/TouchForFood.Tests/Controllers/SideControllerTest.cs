using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Controllers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;
using TouchForFood.Util.Security;
using TouchForFood.ViewModels;
namespace TouchForFood.Tests
{    
    /// <summary>
    ///This is a test class for SideControllerTest and is intended
    ///to contain all SideControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class SideControllerTest
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
        private static table table1;
        private static order order1;
        private static user user1;
        private static order_item orderItem1;
        private static side side1;
        private static side side2;
        #endregion

        #region Properties
        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #endregion

        #region Additional test attributes
        
        //Use ClassInitialize to run code before running the first test in the class
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
            menuItem1 = testDatabase.AddMenuItem(item1, menuCategory1);
            table1 = testDatabase.AddTable(restaurant1);
            order1 = testDatabase.AddOrder(table1);
            user1 = testDatabase.AddUser("sideUnitTest@email.com", table1, (int)SiteRoles.Admin);
        }
                
        //Use ClassCleanup to run code after all tests in a class have run
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            //Remove test data (order specific)            
            testDatabase.RemoveUser(user1);
            testDatabase.RemoveOrder(order1);
            testDatabase.RemoveTable(table1);
            testDatabase.RemoveMenuItem(menuItem1);
            testDatabase.RemoveMenuCategory(menuCategory1);
            testDatabase.RemoveMenu(menu1);
            testDatabase.RemoveRestaurant(restaurant1);
            testDatabase.RemoveCategory(category1);
            testDatabase.RemoveItem(item1);
        }
        
        //Use TestInitialize to run code before running each test
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //Add test data (order specific)
            db = new touch_for_foodEntities();            
            side1 = testDatabase.AddSide(menuCategory1);
            orderItem1 = testDatabase.AddOrderItem(order1, menuItem1);   
            

            //set parameters for menuItem2
            side2 = new side();
            //side2.menu_category = menuCategory1; // This breaks CreateTest
            side2.menu_category_id = menuCategory1.id;
            side2.name = "TestingSide";
            side2.is_active = false;
            side2.is_deleted = false;

        }
        
        //Use TestCleanup to run code after each test has run
        [TestCleanup()]
        public void MyTestCleanup()
        {
            //Remove test data (order specific)            
            testDatabase.RemoveOrderItem(orderItem1);
            testDatabase.RemoveSide(side2);
            testDatabase.RemoveSide(side1);            
        }
        
        #endregion

        #region Test Methods

        /// <summary>
        ///A test for AjaxActive
        ///</summary>
        [TestMethod()]
        public void AjaxActiveSetActiveTest()
        {
            // Arrange
            db = new touch_for_foodEntities();            
            SideController target = new SideController();
            int expected = side1.id;
            bool isActive = true;            
            
            // Act
            int actual = target.AjaxActive(side1.id, isActive);

            // Assert
            side actualSide = db.sides.Find(side1.id);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(isActive, actualSide.is_active);
        }

        /// <summary>
        ///A test for AjaxActive
        ///</summary>
        [TestMethod()]
        public void AjaxActiveSetInactiveTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            SideController target = new SideController();
            int expected = side1.id;
            bool isActive = false;            

            // Act
            int actual = target.AjaxActive(side1.id, isActive);

            // Assert
            side actualSide = db.sides.Find(side1.id);
            Assert.AreEqual(expected, actual);
            Assert.AreEqual(isActive, actualSide.is_active);
        }
 
        /// <summary>
        ///A test for AjaxDelete
        ///</summary>
        [TestMethod()]
        public void AjaxDeleteTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            SideController target = new SideController();
            int expected = side1.id;            

            // Act
            int actual = target.AjaxDelete(side1.id);

            // Assert
            side actualSide = db.sides.Find(side1.id);
            Assert.AreEqual(expected, actual);
        }
                
        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            SideController target = new SideController();
            int expectedSides = db.sides.ToList<side>().Count() + 1;

            // Act
            var actualResult = target.Create(side2) as RedirectToRouteResult;

            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            int actualSides = db.sides.ToList<side>().Count();
            Assert.AreEqual(expectedSides, actualSides);
            Assert.AreEqual("Index", actualResultURI);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateViewTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            SideController target = new SideController();
            SelectList expectedMenuCategoryList = new SelectList(db.menu_category, "id", "id");
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Create();

            //Assert
            SelectList actualSideList = actual.ViewBag.menu_category_id;
            Assert.AreEqual(expectedMenuCategoryList.GetType(), actualSideList.GetType());
            Assert.AreEqual(expectedMenuCategoryList.Count(), actualSideList.Count());
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            SideController target = new SideController();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            SelectList expectedSideList = new SelectList(db.menu_category, "id", "id", side2.menu_category_id);            
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Create(side2);

            // Assert
            SelectList actualSideList = actual.ViewBag.menu_category_id;
            Assert.AreEqual(expectedSideList.GetType(), actualSideList.GetType());
            Assert.AreEqual(expectedSideList.Count(), actualSideList.Count());

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
            SideController target = new SideController();
            int expected = side1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Delete(side1.id);

            //Assert
            Assert.AreEqual(expected, ((side)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for DeleteConfirmed
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            SideController target = new SideController();

            //CheckSetup
            Assert.IsFalse(db.sides.Find(side1.id).is_deleted);

            // Act
            var actualResult = target.DeleteConfirmed(side1.id) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];

            Assert.IsTrue(db.sides.Find(side1.id).is_deleted);
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
            SideController target = new SideController();
            int expectedSides = db.sides.ToList<side>().Count();
            side1 = db.sides.Find(side1.id);
            side1.is_active = true;
            db.Entry(side1).State = EntityState.Modified;
            db.SaveChanges();

            // Act
            var actualResult = target.DeleteConfirmed(side1.id) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            int actualSides = db.sides.ToList<side>().Count();

            Assert.AreEqual(expectedSides, actualSides);
            Assert.IsFalse(db.sides.Find(side1.id).is_deleted);
            Assert.AreEqual("Index", actualResultURI);
        }

        /// <summary>
        ///A test for Details
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            SideController target = new SideController();
            int expected = side1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Details(side1.id);

            //Assert
            Assert.AreEqual(expected, ((side)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            SideController target = new SideController();
            int expectedVersion = side1.version + 1;
            string changeName = "thisIsANewName";
            side1.name = changeName;

            //Check Setup
            Assert.IsFalse(db.sides.Find(side1.id).name == changeName);

            // Act
            var actualResult = target.Edit(side1) as RedirectToRouteResult;

            // Assertions
            db = new touch_for_foodEntities();
            var actualResultURI = actualResult.RouteValues["action"];
            Assert.AreEqual(db.sides.Find(side1.id).version, expectedVersion); //version was incremented
            Assert.IsTrue(db.sides.Find(side1.id).name == changeName); //price was changed
            Assert.AreEqual("Index", actualResultURI); //directed to correct location
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditViewTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            SideController target = new SideController();
            int expected = side1.id;
            SelectList expectedSideList = new SelectList(db.menu_category, "id", "id", side1.menu_category_id);
            
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Edit(side1.id);

            // Assert
            Assert.AreEqual(expected, ((side)actual.ViewData.Model).id);

            SelectList actualSideList = actual.ViewBag.menu_category_id;
            Assert.AreEqual(expectedSideList.GetType(), actualSideList.GetType());
            Assert.AreEqual(expectedSideList.Count(), actualSideList.Count());
        }

        /// <summary>
        ///A test for FilterSides
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void FilterSidesTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            SideController target = new SideController(); // TODO: Initialize to an appropriate value
            menuCategory1.menu = menu1;
            List<side> expectedSides = db.sides.Where
            (
                si =>
                    si.menu_category.menu.resto_id == menuCategory1.menu.resto_id
                    && si.is_active == false
                    && si.is_deleted == false
                    && si.menu_category.id == menuCategory1.id                    
            ).ToList();            
            
            
            PartialViewResult actual;
            
            //Act
            actual = target.FilterSides(menuCategory1);
            
            //Assert
            SideFilterVM actualSideItems = actual.Model as SideFilterVM;
            Assert.AreEqual(expectedSides.Count(), actualSideItems.sides.Count());
        }

        /// <summary>
        ///A test for Index
        ///</summary>
        [TestMethod()]
        public void IndexTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            SideController target = new SideController();
            List<side> expectedSideItems = db.sides.Where(si => si.is_active == true && si.is_deleted == false).ToList();
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Index();

            //Assert
            List<side> actualSideItems = actual.Model as List<side>;
            Assert.AreEqual(expectedSideItems.Count(), actualSideItems.Count());
        }
        #endregion
    }
}
