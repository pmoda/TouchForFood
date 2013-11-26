using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TouchForFood.Controllers;
using TouchForFood.Models;
using TouchForFood.Tests.Classes;
using TouchForFood.Util.Security;
using TouchForFood.Util.ServiceRequest;
using TouchForFood.ViewModels;

namespace TouchForFood.Tests
{    
    /// <summary>
    ///This is a test class for ServiceRequestController and is intended to contain all ServiceRequestController Unit 
    ///Tests
    ///</summary>
    [TestClass()]
    public class ServiceRequestControllerTest
    {
        #region Fields
        private static touch_for_foodEntities db;
        private static TestDatabaseHelper testDatabase;
        private static user user1;
        private static restaurant restaurant1;
        private static table table1;
        private static service_request request1;
        private static ServiceRequestController target;
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
            user1 = testDatabase.AddUser("serviceUnitTest@email.com", table1, (int)SiteRoles.Admin);
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
        /// Use TestInitialize to run code before running each test
        /// </summary>
        [TestInitialize()]
        public void MyTestInitialize()
        {
            request1 = testDatabase.AddServiceRequest(table1);

            //Session
            db = new touch_for_foodEntities();
            target = new ServiceRequestController();
            Session session = new Session(db, target);
            session.simulateLogin(user1.username, user1.password);
        }
        
        /// <summary>
        /// Use TestCleanup to run code after each test has run
        /// </summary>
        [TestCleanup()]
        public void MyTestCleanup()
        {
            testDatabase.RemoveServiceRequest(request1);
        }
        #endregion

        #region TestMethods
        /// <summary>
        ///A test for Details
        ///</summary>
        [TestMethod()]
        public void DetailsTest()
        {
            //Arrange
            int expected = request1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Details(request1.id);

            //Assert
            Assert.AreEqual(expected, ((service_request)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for Cancel
        ///</summary>
        [TestMethod()]
        public void CancelTest()
        {
            //Arrange
            int expected = request1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Cancel(request1.id);

            //Assert
            Assert.AreEqual(expected, ((service_request)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for CancelConfirmed
        ///</summary>
        [TestMethod()]
        public void CancelConfirmedServiceRequestTest()
        {
            //Check Setup
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id).status == (int)ServiceRequestUtil.ServiceRequestStatus.OPEN);

            //Act
            target.CancelConfirmed(request1);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id).status == (int)ServiceRequestUtil.ServiceRequestStatus.CANCELLED);
        }

        /// <summary>
        ///A test for CancelConfirmed Lock Error
        ///</summary>
        [TestMethod()]
        public void CancelConfirmedServiceRequestLockErrorTest()
        {
            //Check Setup
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id).status == (int)ServiceRequestUtil.ServiceRequestStatus.OPEN);

            //Arrange
            request1.version++;

            //Act
            target.CancelConfirmed(request1);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id).status == (int)ServiceRequestUtil.ServiceRequestStatus.OPEN);
        }

        /// <summary>
        ///A test for CancelConfirmed
        ///</summary>
        [TestMethod()]
        public void CancelConfirmedTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            int expected = request1.id;
            ViewResult actual;

            // Act
            actual = (ViewResult)target.CancelConfirmed(request1);

            // Assert
            Assert.AreEqual(expected, ((service_request)actual.ViewData.Model).id);
            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        ///A test for Complete
        ///</summary>
        [TestMethod()]
        public void CompleteTest()
        {
            //Arrange
            int expected = request1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Complete(request1.id);

            //Assert
            Assert.AreEqual(expected, ((service_request)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for CompleteConfirmed
        ///</summary>
        [TestMethod()]
        public void CompleteConfirmedServiceRequestTest()
        {
            //Check Setup
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id).status == (int)ServiceRequestUtil.ServiceRequestStatus.OPEN);

            //Act
            target.CompleteConfirmed(request1);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id).status == (int)ServiceRequestUtil.ServiceRequestStatus.COMPLETED);
        }

        /// <summary>
        ///A test for CompleteConfirmed Lock Error
        ///</summary>
        [TestMethod()]
        public void CompleteConfirmedServiceRequestLockErrorTest()
        {
            //Check Setup
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id).status == (int)ServiceRequestUtil.ServiceRequestStatus.OPEN);

            //Arrange
            request1.version++;

            //Act
            target.CompleteConfirmed(request1);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id).status == (int)ServiceRequestUtil.ServiceRequestStatus.OPEN);
        }

        /// <summary>
        ///A test for CompleteConfirmed
        ///</summary>
        [TestMethod()]
        public void CompleteConfirmedTestInvalidStateModel()
        {
            // Arrange
            db = new touch_for_foodEntities();
            target.ModelState.AddModelError("error", "ModelState is invalid");
            int expected = request1.id;
            ViewResult actual;

            // Act
            actual = (ViewResult)target.CompleteConfirmed(request1);

            // Assert
            Assert.AreEqual(expected, ((service_request)actual.ViewData.Model).id);
            string errorMsg = actual.ViewBag.Error;
            Assert.IsNotNull(errorMsg); //error message is sent to view
        }

        /// <summary>
        /// test create redirect view
        /// </summary>
        [TestMethod()]
        public void CreateViewAdministratorServiceRequestTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            SelectList requestList = new SelectList(db.tables, "id", "name");

            //Act
            var actual = (ViewResult)target.Create();
            
            //Assert
            Assert.AreEqual(requestList.GetType(), actual.ViewBag.table_id.GetType());
            SelectList newRequestList = actual.ViewBag.table_id;
            Assert.AreEqual(requestList.Count(), newRequestList.Count());
        }

        /// <summary>
        ///A test for Create
        ///</summary>
        [TestMethod()]
        public void CreateServiceRequestTest()
        {
            //Check Setup
            db = new touch_for_foodEntities();
            int numberOfRequestsBefore = db.service_request.ToList<service_request>().Count();

            //Arrange
            table table2 = testDatabase.AddTable(restaurant1); //Create Second Table
            //Create second request to be added by method
            service_request request2 = new service_request();
            request2 = new service_request();
            request2.note = "ServiceRequest2";
            request2.created = DateTime.Now;
            request2.status = (int)ServiceRequestUtil.ServiceRequestStatus.OPEN;
            request2.version = 0;
            request2.table_id = table2.id;

            //Act
            target.Create(request2);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request2.id) != null);
            int numberOfServiceRequestsAfter = db.service_request.ToList<service_request>().Count();
            Assert.IsTrue((numberOfRequestsBefore + 1) == numberOfServiceRequestsAfter);

            //Cleanup
            testDatabase.RemoveServiceRequest(request2);
            testDatabase.RemoveTable(table2);
        }

        /// <summary>
        ///A test for Create Duplicate Error Test
        ///</summary>
        [TestMethod()]
        public void CreateServiceRequestDuplicateErrorTest()
        {
            //Check Setup
            db = new touch_for_foodEntities();
            int numberOfRequestsBefore = db.service_request.ToList<service_request>().Count();

            //Arrange
            //Create Second Request
            service_request request2 = new service_request();
            request2 = new service_request();
            request2.note = "ServiceRequest2";
            request2.created = DateTime.Now;
            request2.status = (int)ServiceRequestUtil.ServiceRequestStatus.OPEN;
            request2.version = 0;
            request2.table_id = table1.id;

            //Act
            target.Create(request2);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request2.id) == null);
            int numberOfServiceRequestsAfter = db.service_request.ToList<service_request>().Count();
            Assert.IsTrue(numberOfRequestsBefore == numberOfServiceRequestsAfter);
        }

        /// <summary>
        ///A test for Create Duplicate Null Table
        ///</summary>
        [TestMethod()]
        public void CreateServiceRequestNullTableErrorTest()
        {
            //Check Setup
            db = new touch_for_foodEntities();
            int numberOfRequestsBefore = db.service_request.ToList<service_request>().Count();

            //Arrange
            //Create Second Request
            service_request request2 = new service_request();
            request2 = new service_request();
            request2.note = "ServiceRequest2";
            request2.created = DateTime.Now;
            request2.status = (int)ServiceRequestUtil.ServiceRequestStatus.OPEN;
            request2.version = 0;
            request2.table_id = null;

            //Act
            target.Create(request2);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request2.id) == null);
            int numberOfServiceRequestsAfter = db.service_request.ToList<service_request>().Count();
            Assert.IsTrue(numberOfRequestsBefore == numberOfServiceRequestsAfter);
        }

        /// <summary>
        ///A test for Delete
        ///</summary>
        [TestMethod()]
        public void DeleteTest()
        {
            //Arrange
            int expected = request1.id;
            ViewResult actual;

            //Act
            actual = (ViewResult)target.Delete(request1.id);

            //Assert
            Assert.AreEqual(expected, ((service_request)actual.ViewData.Model).id);
        }

        /// <summary>
        ///A test for DeleteConfirmed
        ///</summary>
        [TestMethod()]
        public void DeleteConfirmedServiceRequestTest()
        {
            //Check Setup
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id) != null);
            int numberOfRequestsBefore = db.service_request.ToList<service_request>().Count();

            //Act 
            target.DeleteConfirmed(request1.id);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id) == null);
            int numberOfServiceRequestsAfter = db.service_request.ToList<service_request>().Count();
            Assert.IsTrue((numberOfRequestsBefore - 1) == numberOfServiceRequestsAfter);
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditViewTest()
        {
            // Arrange
            db = new touch_for_foodEntities();
            int expected = request1.id;
            SelectList expectedTableList = new SelectList(db.tables, "id", "name", request1.table_id);
            ViewResult actual;

            // Act
            actual = (ViewResult)target.Edit(request1.id);

            // Assert
            Assert.AreEqual(expected, ((service_request)actual.ViewData.Model).id);

            SelectList actualTableList = actual.ViewBag.table_id;
            Assert.AreEqual(expectedTableList.GetType(), actualTableList.GetType());
            Assert.AreEqual(expectedTableList.Count(), actualTableList.Count());
        }

        /// <summary>
        ///A test for Edit
        ///</summary>
        [TestMethod()]
        public void EditServiceRequestTest()
        {
            //Check Setup
            Assert.IsTrue(request1.version == 0);
            Assert.IsFalse(request1.note.ToString().Equals("TestingServiceRequest", StringComparison.OrdinalIgnoreCase));

            //Arrange
            request1.note = "TestingServiceRequest";            

            //Act
            target.Edit(request1);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id).version == 1);
            Assert.IsTrue(db.service_request.Find(request1.id).note.ToString()
                .Equals("TestingServiceRequest", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///A test for Edit Lock Error (Version number is off)
        ///</summary>
        [TestMethod()]
        public void EditServiceRequestLockErrorTest()
        {
            //Check Setup
            Assert.IsTrue(request1.version == 0);
            Assert.IsTrue(request1.note.ToString().Equals("UnitTest", StringComparison.OrdinalIgnoreCase));

            //Arrange
            request1.version++;
            request1.note = "TestingServiceRequest";

            //Act
            target.Edit(request1);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id).version == 0);
            Assert.IsTrue(db.service_request.Find(request1.id).note.ToString()
                .Equals("UnitTest", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///A test for Edit Null Table Error (table id is null)
        ///</summary>
        [TestMethod()]
        public void EditServiceRequestNullTableErrorTest()
        {
            //Check Setup
            Assert.IsTrue(request1.version == 0);
            Assert.IsTrue(request1.note.ToString().Equals("UnitTest", StringComparison.OrdinalIgnoreCase));

            //Arrange
            request1.table_id = null;
            request1.note = "TestingServiceRequest";

            //Act
            target.Edit(request1);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request1.id).version == 0);
            Assert.IsTrue(db.service_request.Find(request1.id).note.ToString()
                .Equals("UnitTest", StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        ///A test for Edit Duplicate Error. Trying to assign 2 requests to the same table.
        ///</summary>
        [TestMethod()]
        public void EditServiceRequestDuplicateErrorTest()
        {
            //Arrange
            table table2 = testDatabase.AddTable(restaurant1); //Create Second Table
            service_request request2 = testDatabase.AddServiceRequest(table2); //Create Second Request
            request2.table_id = table1.id;  //Change table id

            //Act
            target.Edit(request2);

            //Assert
            db = new touch_for_foodEntities();
            Assert.IsTrue(db.service_request.Find(request2.id).version == 0);
            Assert.IsTrue(db.service_request.Find(request2.id).table_id == table2.id);

            //Cleanup
            testDatabase.RemoveServiceRequest(request2);
            testDatabase.RemoveTable(table2);
        }

        /// <summary>
        /// Test Index method for an administrator
        /// </summary>
        [TestMethod()]
        public void IndexViewAdministratorServiceRequestTest()
        {
            //Arrange
            db = new touch_for_foodEntities();
            List<service_request> expectedList = db.service_request.ToList<service_request>();

            //Act
            var actual = target.Index();

            //Assert
            ServiceRequestVM actualModel = (ServiceRequestVM)actual.Model;
            Assert.AreEqual(expectedList.Count(), actualModel.AllServiceRequests.Count());
        }
        #endregion
    }
}
