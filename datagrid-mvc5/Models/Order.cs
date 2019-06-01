namespace datagrid_mvc5.Models {
    using Newtonsoft.Json;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public partial class Order {
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
        [CustomCheck("Для  города {0} не меньше {2} было {1} ")]
        public decimal? Freight { get; set; }

        [StringLength(40)]
        public string ShipName { get; set; }

        [MinLength(10)]
        [StringLength(60)]
        public string ShipAddress { get; set; }

        [Required(ErrorMessage = "Поле ShipCity обязательно для заполнения")]
        [StringLength(15)]
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
    }

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

            if (account.ShipCity == "Paris" && val<1000)

            {
               result = new ValidationResult(string.Format(this.ErrorMessage,account.ShipCity , val,1000), memberNames);
            }
           


            return result;
        }
    }

}
