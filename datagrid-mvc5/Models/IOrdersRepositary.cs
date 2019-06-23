using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;

namespace datagrid_mvc5.Models
{
    public interface IOrdersRepositary : IObjectContextAdapter
    {
         IQueryable<IOrder>  Orders { get;  }

         void Delete(int id);

    }
}