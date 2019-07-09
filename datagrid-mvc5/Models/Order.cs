using System.Linq;

namespace datagrid_mvc5.Models {
    using Core.Interfaces;
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class OrderOld: IOrder {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public OrderOld() {
            Order_Details = new HashSet<Order_Detail>();
        }

       int IOrder.Id  {
            get {return OrderID;}
            set { OrderID = value; }
        }

        public int OrderID { get; set; }

        [StringLength(5)]
        public string CustomerID { get; set; }

        public int? EmployeeID { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public int? ShipVia { get; set; }
        [Required(ErrorMessage = "���� Freight ����������� ��� ����������")]
        [CustomCheck("� ������ {0} ������ �������� ������� �� ������ {2} �� ����� {1}")]
        public decimal? Freight { get; set; }

        [StringLength(40)]
        public string ShipName { get; set; }


        [MinMaxLengthAttribute(10,60 ,FieldTitle = "����� �������")]
        public string ShipAddress { get; set; }

        [CheckCityAttribute("���� ShipCity ����������� ��� ����������")]
        public string ShipCity { get; set; }

        [StringLength(15)]
        public string ShipRegion { get; set; }

       
        [StringLength(10)]
        public string ShipPostalCode { get; set; }

        [StringLength(15)]
        public string ShipCountry { get; set; }

        [JsonIgnore]
        public virtual Customer Customer { get; set; }

        [JsonIgnore]
        public virtual Employee Employee { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        public virtual ICollection<Order_Detail> Order_Details { get; set; }

        [JsonIgnore]
        public virtual Shipper Shipper { get; set; }
        ICustomer IOrder.Customer { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public  void Delete()
        {
         //   var orderRepos = Facrory.Get<IOrdersRepositary>();
         //   var order = orderRepos.Delete(this.OrderID);
        }

    }
    /// <summary>
    /// Custom Attribute Example
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public  class CustomCheckAttribute : ValidationAttribute
    {
        public  CustomCheckAttribute(string message)
        {
            this.ErrorMessage = message;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult result = ValidationResult.Success;
            string[] memberNames = new string[] { validationContext.MemberName };
            decimal val = Convert.ToDecimal(value);
            Order account = (Order)validationContext.ObjectInstance;
            if (account.ShipCity == "Rio de Janeiro" && val<100)
            {
               result = new ValidationResult(string.Format(this.ErrorMessage,account.ShipCity , val,100), memberNames);
            }
            return result;
        }
    }
  
    /// <summary>
    /// Custom Attribute Example
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false)]
    public  class CheckCityAttribute : ValidationAttribute
    {
        public CheckCityAttribute(string message)
        {
            this.ErrorMessage = message;
        }
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            ValidationResult result = ValidationResult.Success;
            string[] memberNames = new string[] { validationContext.MemberName };
            string val = value?.ToString();
            Northwind _db = new Northwind();
            Order order = (Order)validationContext.ObjectInstance;
           bool exsist  =  _db.Orders.FirstOrDefault(o => o.ShipCity == val && o.ShipCountry == order.ShipCountry)!=null;
           
            if (!exsist)
            {
               result = new ValidationResult(string.Format(this.ErrorMessage,order.ShipCity , val), memberNames);
            }
            return result;
        }
    }

    public class MinMaxLengthAttribute : StringLengthAttribute
    {
        int? _stringLength = null;
        public  string FieldTitle { get; set; }
        public MinMaxLengthAttribute(int minimum, int maximum)
            : base(maximum)
        {
            // SetErrorMessage();
            MinimumLength = minimum;
        }

        void SetErrorMessage(string stringLength)
        {
      
            ErrorMessage = $"����� ���� {FieldTitle} ������ ���� �� {MinimumLength} �� {MaximumLength} ��������." +
                           $" ������� �����  {stringLength}  ��������.";
        }
        //public override bool IsValid(object value)
        //{
        //    //string s = value as string;
        //    //if (s != null)
        //    //{
        //    //   var  stringLength = s.Length.ToString();
        //    //    SetErrorMessage( stringLength);
        //    //}

        //    return base.IsValid(value);
        //}
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {

            var name = FieldTitle ?? validationContext.MemberName;
            string s = value as string;
            if (s != null)
            {

                var stringLength = s.Length;
                ErrorMessage = $"����� ���� {name} ������ ���� �� {MinimumLength} �� {MaximumLength} ��������." +
                               $" ������� �����  {stringLength}  ��������." ;
            }

            return base.IsValid(value, validationContext);
        }
    }




}
