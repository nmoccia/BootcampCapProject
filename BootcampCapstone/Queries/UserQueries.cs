using BootcampCapstone.Models;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BootcampCapstone.Queries
{
    public class UserQueries 
    {
        public User Get(string name)
        {
            User user;
            using (var db = new EventManagerEntities())
            {
                using (var tx = db.Database.Connection.BeginTransaction(IsolationLevel.Unspecified))
                {
                    user = db.Users.First(m => m.username == name);
                    tx.Commit();
                }
            }
            return user;
        }

        public VerificationResult VerifyUsernameAndPassword(string username, string password)
        {
            User user;
            using (var db = new EventManagerEntities())
            {
                user = db.Users.FirstOrDefault(m => m.username.ToUpper() == username.ToUpper());
                if (user == null)
                {
                    return VerificationResult.UserNameDoesNotExist;
                }
                else if (user.password != password)
                {
                    return VerificationResult.PasswordIncorrect;
                }

                return VerificationResult.Correct;
            }
        }

        public enum VerificationResult
        {
            UserNameDoesNotExist,
            PasswordIncorrect,
            Correct
        }

    }
}
