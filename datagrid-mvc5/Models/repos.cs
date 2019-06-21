using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using datagrid_mvc5.Models;

namespace AutoFacWithWebAPI.Repository.Common
{
    public class GenericRepository<T>   where T : class
    {

        private Northwind dbContext;

        protected IDbFactory DbFactory
        {
            get;
            private set;
        }

        protected Northwind DbContext
        {
            get { return dbContext ?? (dbContext = DbFactory.Init()); }
        }

        public GenericRepository(IDbFactory dbFactory)
        {
            DbFactory = dbFactory;
        }

        public IQueryable<T> GetAll()
        {
            return DbContext.Set<T>();
        }
    }

    public interface IDbFactory
    {
        Northwind Init();
    }
}