using System.Linq;

namespace datagrid_mvc5.Models {
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order: IOrder {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Order() {
            Order_Details = new HashSet<Order_Detail>();
        }

        public int OrderID { get; set; }

        [StringLength(5)]
        public string CustomerID { get; set; }

        public int? EmployeeID { get; set; }

        public DateTime? OrderDate { get; set; }

        public DateTime? RequiredDate { get; set; }

        public DateTime? ShippedDate { get; set; }

        public int? ShipVia { get; set; }
        [Required(ErrorMessage = "Поле Freight обязательно для заполнения")]
        [CustomCheck("К городу {0} особое уважение поэтому не меньше {2} Вы ввели {1}")]
        public decimal? Freight { get; set; }

        [StringLength(40)]
        public string ShipName { get; set; }


        [MinMaxLengthAttribute(10,60 ,FieldTitle = "Адрес корабля")]
        public string ShipAddress { get; set; }

        [CheckCityAttribute("Поле ShipCity обязательно для заполнения")]
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
      
            ErrorMessage = $"Длина поля {FieldTitle} должна быть от {MinimumLength} до {MaximumLength} символов." +
                           $" Текущая длина  {stringLength}  символов.";
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
                ErrorMessage = $"Длина поля {name} должна быть от {MinimumLength} до {MaximumLength} символов." +
                               $" Текущая длина  {stringLength}  символов." ;
            }

            return base.IsValid(value, validationContext);
        }
    }




}
