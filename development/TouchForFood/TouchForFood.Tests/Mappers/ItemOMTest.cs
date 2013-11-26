using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Exceptions;
using TouchForFood.Mappers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;

namespace TouchForFood.Tests
{    
    /// <summary>
    ///This is a test class for ItemOMTest and is intended
    ///to contain all ItemOMTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ItemOMTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static item item1;
        private static item item2;
        private static menu_item menuItem1;
        private static category category1;
        private static restaurant restaurant1;
        private static menu menu1;
        private static menu_category menuCategory1;
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
            //Add test data (order specific)
            testDatabase = new TestDatabaseHelper();
            item1 = testDatabase.AddItem();
            item2 = testDatabase.AddItem();
            category1 = testDatabase.AddCategory();
            restaurant1 = testDatabase.AddRestaurant();
            menu1 = testDatabase.AddMenu(restaurant1);
            menuCategory1 = testDatabase.AddMenuCategory(category1, menu1);
            menuItem1 = testDatabase.AddMenuItem(item2, menuCategory1);
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveMenuItem(menuItem1);
            testDatabase.RemoveMenuCategory(menuCategory1);
            testDatabase.RemoveMenu(menu1);
            testDatabase.RemoveRestaurant(restaurant1);
            testDatabase.RemoveCategory(category1);
            testDatabase.RemoveItem(item2);
            testDatabase.RemoveItem(item1);
        }
        #endregion

        #region Test Methods
        /// <summary>
        ///A test for deleting an item. The result should be 1, for the
        ///number of affected rows.
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            ItemOM target = new ItemOM(db);
            int expected = 1;
            int actual;

            //Check setup
            Assert.IsNotNull(db.items.Find(item1.id));
            
            //Act
            actual = target.delete(item1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.AreEqual(expected, actual);
            Assert.IsNull(db.items.Find(item1.id));
        }

        /// <summary>
        ///If the item has a menu item association an AssociationExistsException must be thrown
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(AssociationExistsException))]
        public void DeleteAssociationExistsExceptionTest()
        {
            //arrange
            db = new touch_for_foodEntities();
            ItemOM target = new ItemOM(db);
            int actual;

            //act
            actual = target.delete(item2.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsNotNull(db.items.Find(item2.id));
            Assert.IsNotNull(db.menu_item.Find(menuItem1.id));

        }
        #endregion
    }
}
