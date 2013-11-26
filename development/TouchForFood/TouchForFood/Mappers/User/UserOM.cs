using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Infrastructure;
using System.Linq;
using TouchForFood.Mappers.Abstract;
using TouchForFood.Models;

namespace TouchForFood.Mappers
{
    public class UserOM : GenericOM
    {
        public UserOM():base(){}
        public UserOM(touch_for_foodEntities db):base(db){}

        /// <summary>
        /// Writes a user object to the database
        /// </summary>
        /// <param name="user">The user object to write</param>
        /// <returns>True if successful, false otherwise</returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="DBEntityValidationException"></exception>
        /// <exception cref="DbUpdateException"></exception>
        public bool Create(user user)
        {
            db.users.Add(user);
            return (db.SaveChanges() == 1);
        }

        public Boolean edit(user user)
        {
            UserIM im = new UserIM(db);
            user dbVersion =im.find(user.id);
            if (dbVersion.version == user.version)
            {
                user.current_table_id = dbVersion.current_table_id;

                // If password was blank (only editing details) make sure we keep it the same
                if (user.password == null)
                {
                    user.password = dbVersion.password;
                    user.ConfirmPassword = user.password;
                }

                // If the user's role is null (happens if the user is editing his/her own details)
                if (user.user_role == null)
                    user.user_role = dbVersion.user_role;

                ((IObjectContextAdapter)db).ObjectContext.Detach(dbVersion);
                db.Entry(user).State = EntityState.Modified;

                user.version = user.version + 1;
                db.SaveChanges();
                return true;
            }
            return false;

        }

        public override int delete(int id)
        {
            user user = db.users.Find(id);
            int result = 0;
         
            clearFriendship(user.friendships);
            db.users.Remove(user);
            result = db.SaveChanges();

            return result;
        }

        private void clearFriendship(ICollection<friendship> friends)
        {
            for (int i = 0; i < friends.Count; i++)
            {
                friendship f = friends.ElementAt(i);
                db.friendships.Remove(db.friendships.Find(f.id));
            }
        }
    }
}