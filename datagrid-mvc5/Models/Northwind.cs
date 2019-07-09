using System.Data.Entity.Validation;

namespace datagrid_mvc5.Models {
    using System;
    using System.Data.Entity;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Linq;
    using System.Diagnostics;
    using System.Collections.Generic;
    using System.Collections;
    using Core.Interfaces;

    public partial class Northwind : DbContext,IOrdersRepositary,ICustomerRepositary,IEmployeeRepositary {
        public Northwind(): base("name=Northwind") {
            Configuration.ProxyCreationEnabled = false;
            base.Database.Log = WriteToLog;
        }

        void WriteToLog(string message)
        {
            Debug.WriteLine(message);
        }
        public virtual DbSet<Category> Categories { get; set; }
        public virtual DbSet<Customer> Customers { get; set; }
        public virtual DbSet<Employee> Employees { get; set; }
        void IEmployeeRepositary.Delete(int id)
        {
            throw new NotImplementedException();
        }

        void IEmployeeRepositary.Add(IEmployee order)
        {
            throw new NotImplementedException();
        }

        public virtual DbSet<Order_Detail> Order_Details { get; set; }
        public virtual DbSet<Order> orders { get; set; }
        public virtual DbSet<Product> Products { get; set; }
        public virtual DbSet<Shipper> Shippers { get; set; }
        public virtual DbSet<Supplier> Suppliers { get; set; }

       public   IQueryable<IOrder> Orders { get { return orders; } }

        IQueryable<ICustomer> ICustomerRepositary.Customers => throw new NotImplementedException();

        IQueryable<IEmployee> IEmployeeRepositary.Employees { get; }

        void IOrdersRepositary.Delete(int id)
        {
            var order =  orders.Find(id);
            orders.Remove(order);
            SaveChanges();
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder) {
            modelBuilder.Entity<Customer>()
                .Property(e => e.CustomerID)
                .IsFixedLength();

            modelBuilder.Entity<Employee>()
                .HasMany(e => e.Employees1)
                .WithOptional(e => e.Employee1)
                .HasForeignKey(e => e.ReportsTo);

            modelBuilder.Entity<Order_Detail>()
                .Property(e => e.UnitPrice)
                .HasPrecision(9, 2);

            modelBuilder.Entity<Order>()
                .Property(e => e.CustomerID)
                .IsFixedLength();

            modelBuilder.Entity<Order>()
                .Property(e => e.Freight)
                .HasPrecision(9, 2);

            modelBuilder.Entity<Order>()
                .HasMany(e => e.Order_Details)
                .WithRequired(e => e.Order)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Product>()
                .Property(e => e.UnitPrice)
                .HasPrecision(9, 2);

            modelBuilder.Entity<Product>()
                .HasMany(e => e.Order_Details)
                .WithRequired(e => e.Product)
                .WillCascadeOnDelete(false);

            modelBuilder.Entity<Shipper>()
                .HasMany(e => e.Orders);
            //  .WithOptional(e => e.Shipper)
            //  .HasForeignKey(e => e.ShipVia);
        }

        void IOrdersRepositary.Add(IOrder order)
        {
            var ord= order as Order;
            if(ord==null)throw new NullReferenceException("В репозитарий можно добавлять только объекты типа Order");
            orders.Add(ord);
        }

        IOrder IOrdersRepositary.Find(int id)
        {
            return orders.Find(id);
        }

      public bool HasChanges()
       {
          return ChangeTracker.HasChanges();
       }

     public  void Save()
       {
           SaveChanges();
       }

       //public  IEnumerable<DbEntityValidationResult> GetValidationErrors()
       //{
       //  return  base.GetValidationErrors();
       //}

       void ICustomerRepositary.Add(ICustomer customer)
        {
            var cust = customer as Customer;
            if (cust == null) throw new NullReferenceException("В репозитарий можно добавлять только объекты типа Customer");
            Customers.Add(cust);

        }

        void ICustomerRepositary.Delete(int id)
        {
            var customer = Customers.Find(id);
            Customers.Remove(customer);
            SaveChanges();
        }
    }
}
