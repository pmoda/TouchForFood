using TouchForFood.Controllers;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using System.Web.Mvc;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;
using TouchForFood.Util.Security;

namespace TouchForFood.Tests
{
    
    
    /// <summary>
    ///This is a test class for Review_Order_ItemControllerTest and is intended
    ///to contain all Review_Order_ItemControllerTest Unit Tests
    ///</summary>
    [TestClass()]
    public class Review_Order_ItemControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static restaurant restaurant1;
        private static table table1;
        private static user user1;
        private static order order1;
        private static order_item orderItem1;
        private static review review1;
        private static menu_item menuItem1;
        private static item item1;
        private static category category1;
        private static menu menu1;
        private static menu_category menuCategory1;
        private static review_order_item reviewItem1;
        #endregion

        #region Test Attributes
        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        private TestContext testContextInstance;
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
            user1 = testDatabase.AddUser("rOrderItemUnitTest@email.com", table1, (int)SiteRoles.Customer);
            order1 = testDatabase.AddOrder(table1);
            item1 = testDatabase.AddItem();
            category1 = testDatabase.AddCategory();
            menu1 = testDatabase.AddMenu(restaurant1);
            menuCategory1 = testDatabase.AddMenuCategory(category1, menu1);
            menuItem1 = testDatabase.AddMenuItem(item1, menuCategory1);
            orderItem1 = testDatabase.AddOrderItem(order1, menuItem1);

            //Session
            db = new touch_for_foodEntities();
            Review_Order_ItemController target = new Review_Order_ItemController();
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
            testDatabase.RemoveOrderItem(orderItem1);
            testDatabase.RemoveMenuItem(menuItem1);
            testDatabase.RemoveMenuCategory(menuCategory1);
            testDatabase.RemoveMenu(menu1);
            testDatabase.RemoveCategory(category1);
            testDatabase.RemoveItem(item1);
            testDatabase.RemoveOrder(order1);
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
            review1 = testDatabase.AddReview(restaurant1, order1, user1);
            reviewItem1 = testDatabase.AddReviewOrderItem(review1, orderItem1, "rexiewOITest", 1);
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            foreach (review_order_item roi in db.reviews.Find(review1.id).review_order_item)
            {
                testDatabase.RemoveReviewOrderItem(roi);
            }
            testDatabase.RemoveReview(review1);


        }

        #endregion

        #region Test Methods


        /// <summary>
        ///A test for Create
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void CreateTest1()
        {
            Review_Order_ItemController target = new Review_Order_ItemController(); // TODO: Initialize to an appropriate value
            review_order_item roi = reviewItem1; // TODO: Initialize to an appropriate value
            ActionResult actual;
            actual = (ActionResult)target.Create(roi);
            Assert.IsNotNull(actual);
            // Assert.AreEqual("Index", actual.ToString());
            // Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Index
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void IndexTest()
        {
            Review_Order_ItemController target = new Review_Order_ItemController(); // TODO: Initialize to an appropriate value
            ViewResult expected = null; // TODO: Initialize to an appropriate value
            ViewResult actual;
            actual = target.Index();
            Assert.IsNotNull(actual);
            //Assert.AreEqual(expected, actual);
        }
        
        /// <summary>
        ///A test for Edit
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void EditTest()
        {
            Review_Order_ItemController target = new Review_Order_ItemController(); // TODO: Initialize to an appropriate value
            review_order_item review = reviewItem1; // TODO: Initialize to an appropriate value
            ActionResult expected = null; // TODO: Initialize to an appropriate value
            ActionResult actual;
            actual = target.Edit(reviewItem1);
            //Assert.AreEqual(expected, actual);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void EditTest1()
        {
            Review_Order_ItemController target = new Review_Order_ItemController(); // TODO: Initialize to an appropriate value
            int id = reviewItem1.id; // TODO: Initialize to an appropriate value
            ActionResult expected = null; // TODO: Initialize to an appropriate value
            ActionResult actual;
            actual = target.Edit(id);
            Assert.IsNotNull(actual);
            //Assert.AreEqual(expected, actual);
            //Assert.Inconclusive("Verify the correctness of this test method.");
        }

        /// <summary>
        ///A test for Dispose
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void DisposeTest()
        {
            Review_Order_ItemController_Accessor target = new Review_Order_ItemController_Accessor(); // TODO: Initialize to an appropriate value
            bool disposing = false; // TODO: Initialize to an appropriate value
            target.Dispose(disposing);
        }

        /// <summary>
        ///A test for Details
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void DetailsTest()
        {
            Review_Order_ItemController target = new Review_Order_ItemController(); // TODO: Initialize to an appropriate value
            int id = reviewItem1.id; // TODO: Initialize to an appropriate value
            ViewResult expected = null; // TODO: Initialize to an appropriate value
            ViewResult actual;
            actual = target.Details(id);
            Assert.IsNotNull(actual);
            //Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for DeleteConfirmed
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void DeleteConfirmedTest()
        {
            Review_Order_ItemController target = new Review_Order_ItemController(); // TODO: Initialize to an appropriate value
            int id = reviewItem1.id; // TODO: Initialize to an appropriate value
            ActionResult expected = null; // TODO: Initialize to an appropriate value
            ActionResult actual;
            actual = target.DeleteConfirmed(id);
            Assert.IsNotNull(actual);
            //Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void DeleteTest()
        {
            Review_Order_ItemController target = new Review_Order_ItemController(); // TODO: Initialize to an appropriate value
            int id = reviewItem1.id; // TODO: Initialize to an appropriate value
            ActionResult expected = null; // TODO: Initialize to an appropriate value
            ActionResult actual;
            actual = target.Delete(id);
            Assert.IsNotNull(actual);
            //Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void CreateTest()
        {
            Review_Order_ItemController target = new Review_Order_ItemController(); // TODO: Initialize to an appropriate value
            ActionResult expected = null; // TODO: Initialize to an appropriate value
            ActionResult actual;

            actual = target.Create();
            // Assert.AreEqual(expected, actual);
            Assert.IsNotNull(actual);
        }

        /// <summary>
        ///A test for AjaxDelete
        ///</summary>
        [TestMethod()]
        public void AjaxDeleteTest()
        {
            Review_Order_ItemController target = new Review_Order_ItemController(); // TODO: Initialize to an appropriate value
            int id = reviewItem1.id; // TODO: Initialize to an appropriate value
            int expected = reviewItem1.id; // TODO: Initialize to an appropriate value
            int actual;
            actual = target.AjaxDelete(id);
            //Assert.IsNotNull(actual);
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///A test for Review_Order_ItemController Constructor
        ///</summary>
        // TODO: Ensure that the UrlToTest attribute specifies a URL to an ASP.NET page (for example,
        // http://.../Default.aspx). This is necessary for the unit test to be executed on the web server,
        // whether you are testing a page, web service, or a WCF service.
        [TestMethod()]
        public void Review_Order_ItemControllerConstructorTest()
        {
            Review_Order_ItemController target = new Review_Order_ItemController();
        }
    }
        #endregion
}
