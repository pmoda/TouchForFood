using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Exceptions;
using TouchForFood.Mappers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;

namespace TouchForFood.Tests
{
    /// <summary>
    ///This is a test class for CategoryOMTest and is intended
    ///to contain all CategoryOMTest Unit Tests
    ///</summary>
    [TestClass()]
    public class CategoryOMTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static category category1;
        private static category category2;
        private static category category3;
        private static menu_category menuCategory1;
        private static item item1;
        private static menu menu1;
        private static restaurant restaurant1;
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
            category1 = testDatabase.AddCategory();
            category2 = testDatabase.AddCategory();
            category3 = testDatabase.AddCategory();
            restaurant1 = testDatabase.AddRestaurant();
            menu1 = testDatabase.AddMenu(restaurant1);
            menuCategory1 = testDatabase.AddMenuCategory(category1, menu1);
            item1 = testDatabase.AddItem(category3);
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveItem(item1);
            testDatabase.RemoveMenuCategory(menuCategory1);
            testDatabase.RemoveMenu(menu1);
            testDatabase.RemoveRestaurant(restaurant1);
            testDatabase.RemoveCategory(category3);
            testDatabase.RemoveCategory(category2);
            testDatabase.RemoveCategory(category1);
        }
        #endregion

        #region Test Methods
        /// <summary>
        ///Pass an id to delete a category. The returning value has to be
        ///greater then 1.
        ///</summar>
        [TestMethod()]
        public void DeleteTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            CategoryOM target = new CategoryOM(db);
            int expected = 1;
            int actual;

            //Check Setup
            Assert.IsNotNull(db.categories.Find(category2.id));

            //Act
            actual = target.delete(category2.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.AreEqual(expected, actual);
            Assert.IsNull(db.categories.Find(category2.id));
        }

        ///<summary>
        ///Pass an id to delete a category. If the category has items 
        ///an AssociationExistsException must be thrown.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(AssociationExistsException))]
        public void DeleteAssociationExistsExceptionItemTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            CategoryOM target = new CategoryOM(db);
            int actual;

            //Act
            actual = target.delete(category3.id);

            //Assert
            Assert.IsNotNull(db.categories.Find(category3.id));
            Assert.IsNotNull(db.items.Find(item1.id));
        }

        ///<summary>
        ///Pass an id to delete a category. If the category has a menu
        ///category an AssociationExistsException must be thrown.
        ///</summary>
        [TestMethod()]
        [ExpectedException(typeof(AssociationExistsException))]
        public void DeleteAssociationExistsExceptionMenuCategoryTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            CategoryOM target = new CategoryOM(db);
            int actual;

            //Act
            actual = target.delete(category1.id);

            //Assert
            Assert.IsNotNull(db.categories.Find(category1.id));
            Assert.IsNotNull(db.menu_category.Find(menuCategory1.id));
        }
        #endregion
    }
}
