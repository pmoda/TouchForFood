using TouchForFood.Models;
using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using TouchForFood.Exceptions;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Util.ServiceRequest;

namespace TouchForFood.Mappers
{
    public class ServiceRequestIM : GenericIM
    {

        public ServiceRequestIM() : base() { }

        public ServiceRequestIM(touch_for_foodEntities db) : base(db) { }

        /// <summary>
        /// Allows you to find a service request using the service request id
        /// </summary>
        /// <param name="id">id to be found</param>
        /// <returns>the service request</returns>
        public service_request find(int id)
        {
            try
            {
                return db.service_request.Find(id);
            }
            catch (InvalidOperationException)
            {
                return null;
            }
        }

        /// <summary>
        /// Allows you to find a service request using the service request for the service request current table id with an open status
        /// </summary>
        /// <param name="tableId">service_request to be found</param>
        /// <returns>the service request</returns>
        public ICollection<service_request> FindByOpenRequestByTable(int tableId)
        {
            try
            {
                return db.service_request.Where(sr => sr.table_id == tableId &&
                                                sr.status == (int)ServiceRequestUtil.ServiceRequestStatus.OPEN)
                                                .ToList<service_request>();
            }
            catch (InvalidOperationException)
            {
                return null;
            }
            catch (ArgumentNullException)
            {
                return null;
            }
        }

        /// <summary>
        /// Allows you to find a list of all service_requests
        /// </summary>
        /// <returns>ICollection of service request</returns>
        public ICollection<service_request> find()
        {
            try
            {
                return db.service_request.ToList();
            }
            catch (System.ArgumentNullException)
            {
                return null;
            }
        }
    }
}