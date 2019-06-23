using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace datagrid_mvc5.Models
{


    public interface IOrder
    {

        int OrderID { get; set; }

        [StringLength(5)]
        string CustomerID { get; set; }

        int? EmployeeID { get; set; }

        DateTime? OrderDate { get; set; }

        DateTime? RequiredDate { get; set; }

        DateTime? ShippedDate { get; set; }

        int? ShipVia { get; set; }

        [CustomCheck("К городу {0} особое уважение поэтому не меньше {2} Вы ввели {1}")]
        decimal? Freight { get; set; }

        [StringLength(40)]
        string ShipName { get; set; }


        [MinMaxLengthAttribute(10, 60, FieldTitle = "Адрес корабля")]
        string ShipAddress { get; set; }

        [CheckCityAttribute("Поле ShipCity обязательно для заполнения")]
        string ShipCity { get; set; }


        string ShipRegion { get; set; }





        [StringLength(10)]
        string ShipPostalCode { get; set; }

        [StringLength(15)]
        string ShipCountry { get; set; }

        [JsonIgnore]
        Customer Customer { get; set; }

        [JsonIgnore]
        Employee Employee { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        [JsonIgnore]
        ICollection<Order_Detail> Order_Details { get; set; }

        [JsonIgnore]
        Shipper Shipper { get; set; }
    }


}
