using System;
namespace BootcampCapstone.Queries
{
    public interface IEventQueries
    {
        PagedList.IPagedList<BootcampCapstone.Event> GetEvents(int page, int pageSize, string sortOrder, string searchString);
    }
}
