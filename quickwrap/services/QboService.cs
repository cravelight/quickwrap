using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Intuit.Ipp.Data;
using Intuit.Ipp.DataService;
using quickwrap.connections;

namespace quickwrap.services
{
    public abstract class QboService<T> where T: IEntity
    {
        protected QboService(IQboConnection connection)
        {
            Svc = connection.DataService;
        }
        protected DataService Svc;


        // Create
        public abstract T Add(T item);


        // Read
        public abstract IEnumerable<T> List(int startPosition = 1, int maxResults = 500); 


        // Update


        // Delete


    }
}
