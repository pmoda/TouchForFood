using TouchForFood.Models;
using System;
using System.Linq;
using System.Data;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Exceptions;


namespace TouchForFood.Mappers
{
    public class ServiceRequestOM : GenericOM
    {
        public ServiceRequestOM() : base() { }
        public ServiceRequestOM(touch_for_foodEntities db) : base(db) { }

        /// <summary>
        /// Writes a service_request object to the database
        /// </summary>
        /// <param name="sr">The service_request object to write</param>
        /// <returns>True if successful, false otherwise</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public bool Create(service_request sr)
        {
            db.service_request.Add(sr);
            return (db.SaveChanges() == 1);
        }

        /// <summary>
        /// Saves the service request to the db and updates the version
        /// </summary>
        /// <param name="service_request">service_request to be edited</param>
        /// <returns>boolean</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public Boolean edit(service_request serviceRequest)
        {
            ServiceRequestIM im = new ServiceRequestIM(db);
            service_request dbVersion = im.find(serviceRequest.id);
            if (dbVersion.version == serviceRequest.version)
            {
                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(serviceRequest).State = EntityState.Modified;

                serviceRequest.version = serviceRequest.version + 1;
                db.SaveChanges();
                return true;
            }
            return false;

        }

        /// <summary>
        /// Deletes a service request
        /// </summary>
        /// <param name="id">id to be deleted</param>
        /// <returns>int the number of effected items</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public override int delete(int req_id)
        {
            service_request req = db.service_request.Find(req_id);
            int result = 0;
            
            db.service_request.Remove(req);
            result = db.SaveChanges();

            return result;
        }
    }
}