using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using PagedList;
using System.Data;
using System.Data.Entity;

namespace BootcampCapstone.Queries
{
    public class EventQueries : BootcampCapstone.Queries.IEventQueries
    {
        
        public IPagedList<Event> GetEvents(int page, int pageSize, string sortOrder, string searchString)
        {
            IPagedList<Event> viewModel;
            searchString = searchString.ToUpper();
            using (var db = new EventManagerEntities())
            {
                using (var tx = db.Database.Connection.BeginTransaction(IsolationLevel.Unspecified))
                {
                    var events = from i in db.Events select i;

                    if (!String.IsNullOrEmpty(searchString))
                    {
                        events = events.Where(i => i.title.ToUpper().Contains(searchString)
                                    || i.location.ToUpper().Contains(searchString)
                                    || i.Category.ToString().ToUpper().Contains(searchString)
                                    || i.eventDescription.ToUpper().Contains(searchString)
                                    || i.Type.ToString().ToUpper().Contains(searchString));
                    }
                    viewModel = events.ToPagedList(page, pageSize);
                    tx.Commit();
                    return viewModel;
                }
            }
        }
        
    }
}