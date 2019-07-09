using System;
using System.Data.Entity;
using System.Transactions;

namespace Orders
{
    public class Class1
    {
    }
}
public class BaseObject : IObject
{
    DbContext _context;
    DbContext Context
    {
        get;
        set;
    }

    public  void Save(Transaction transaction = null)
    {
        if (transaction == null)
        {
            Context.Database.BeginTransaction();
        }
        Context.SaveChanges();

    }

    public  void Delete(Transaction transaction = null)
    {
        if (transaction == null)
        {
            Context.Database.BeginTransaction();
        }
      //  Context.Re
    }

}
public interface  IObject
{

     void Save(Transaction transaction=null);

     void Delete(Transaction transaction=null);

}
