using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.VisualStudio.TestTools.UnitTesting.Web;
using TouchForFood.Mappers;
using TouchForFood.Models;
using TouchForFood.Exceptions;
using System.Data;

namespace TouchForFood.Tests
{
    
    
    /// <summary>
    ///This is a test class for ReviewOMTest and is intended
    ///to contain all ReviewOMTest Unit Tests
    ///</summary>
    [TestClass()]
    public class ReviewOMTest
    {


        private TestContext testContextInstance;
        private touch_for_foodEntities db = new touch_for_foodEntities();
        private static review m_review;
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

        #region Additional test attributes


        [ClassInitialize()]
        public static void MyClassInitialize(TestContext testContext)
        {
            touch_for_foodEntities db = new touch_for_foodEntities();
            m_review = new review();
            m_review.restaurant_id = db.restaurants.First().id;
            m_review.rating = 1;
            m_review.is_anonymous = true;

            db.reviews.Add(m_review);
            db.SaveChanges();
            db.Dispose();
        }
        //
        
        [ClassCleanup()]
        public static void MyClassCleanup()
        {
            touch_for_foodEntities db = new touch_for_foodEntities();
            review r = db.reviews.Find(m_review.id);
            db.reviews.Remove(r);
            db.SaveChanges();
            db.Dispose();
        }
        
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for delete
        ///</summary>
        [TestMethod()]
        public void deleteTest()
        {
            //arrange
            ReviewOM target = new ReviewOM(db);
            int id = m_review.id;
            int expected = 2;
            int actual;
            //act
            actual = target.delete(id);
            //assert
            Assert.AreEqual(expected, actual);
        }
    }
}
