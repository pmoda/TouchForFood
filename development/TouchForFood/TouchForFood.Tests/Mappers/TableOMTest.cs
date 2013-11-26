using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Mappers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;

namespace TouchForFood.Tests
{  
    /// <summary>
    ///This is a test class for TableOMTest and is intended
    ///to contain all TableOMTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TableOMTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static restaurant restaurant1;
        private static table table1;
        private static order order1;
        private static service_request serviceRequest1;
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
        }

        /// <summary>
        /// Use ClassCleanup to run code after all tests in a class have run
        /// </summary>
        [ClassCleanup()]
        public static void MyClassCleanup() { }

        /// <summary>
        /// Use TestInitialize to run code before running each test 
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            //Add test data (order specific)
            restaurant1 = testDatabase.AddRestaurant();
            table1 = testDatabase.AddTable(restaurant1);
            order1 = testDatabase.AddOrder(table1);
            serviceRequest1 = testDatabase.AddServiceRequest(table1);
        }

        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            //Remove test data (order specific)
            testDatabase.RemoveServiceRequest(serviceRequest1);
            testDatabase.RemoveOrder(order1);
            testDatabase.RemoveTable(table1);
            testDatabase.RemoveRestaurant(restaurant1);
        }
        #endregion

        #region Test Methods
        /// <summary>
        ///A test for delete. Returns number of effected rows.
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            TableOM target = new TableOM(db);
            int expected = 3;
            int actual;

            table t = db.tables.Find(table1.id);

            //Check Setup
            Assert.IsNotNull(db.tables.Find(table1.id));
            Assert.IsNotNull(db.orders.Find(order1.id).table_id);
            Assert.IsNotNull(db.service_request.Find(serviceRequest1.id).table_id);

            //Act
            actual = target.delete(table1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsNull(db.tables.Find(table1.id));
            Assert.IsNull(db.orders.Find(order1.id).table_id);
            Assert.IsNull(db.service_request.Find(serviceRequest1.id).table_id);
            Assert.AreEqual(expected, actual);
        }
        #endregion
    }
}
