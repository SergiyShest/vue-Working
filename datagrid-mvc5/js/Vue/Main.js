new Vue({
    el: "#app",
    mixins: [baseMixin],
    data: {
        id: ID,
        selected: '',
        order: {
            OrderID: 0,
            CustomerID: 0,
            EmployeeID: 0,
            OrderDate: "",
            RequiredDate: "",
            ShippedDate: "",
            ShipVia: 0,
            Freight: 0,
            ShipName: "",
            ShipAddress: "",
            ShipCity: "",
            ShipRegion: "",
            ShipPostalCode: "",
            ShipCountry: ""
        },
        oldOrder: {
            ShipCountry: ""
        },
        errors: {
            OrderID: null,
            CustomerID: null,
            EmployeeID: null,
            OrderDate: null,
            RequiredDate: null,
            ShippedDate: null,
            ShipVia: null,
            Freight: null,
            ShipName: null,
            ShipAddress: null,
            ShipCity: null,
            ShipRegion: null,
            ShipPostalCode: null,
            ShipCountry: null
        },
        IsChanged: false,
        AvaialbeCustomers: [],
        AvaialbeCitys: [],
        AvaialbeRegions: [],
        AvaialbeCountrys: []
    },
    computed:
    {
        hasError: function () {
            for (var err in this.errors) {
                var error = this.errors[err];
                if (error !== '' || null) return true;
            }
            return false;
        }
    },
    methods: {
        fetchOrder() {
            var path = "../Orders/GetById?Id=" + this.id;
            this.fetchJson(path, json => this.order = json);
        },

        fetchCityList() {
            //Avaiable city depended on  country
            var country = this.order.ShipCountry;
            if (country == null || country === "") {
                country = '';
            }
            var path = "../Orders/AvaiableCityList?country=" + country;
            this.fetchJson(path, json => { this.AvaialbeCitys = json; });
        },

        fetchCountrys() {
            var path = "../Orders/AvaiableCountrys";
            this.fetchJson(path, jsonResult => { this.AvaialbeCountrys = jsonResult; });
        },

        fetchAvaialbeCustomers() {
            var path = "../Orders/AvaialbeCustomers";
            this.fetchJson(path, jsonResult => { this.AvaialbeCustomers = jsonResult; });
        },

        SaveOldValue() {
            this.oldOrder.ShipCountry = this.order.ShipCountry;
        }
    },
    watch: {
        order: {
            handler: function (after) {
                this.IsChanged = true;
                if (this.oldOrder.ShipCountry !== after.ShipCountry) {
                    this.fetchCityList();
                }
                this.SaveOldValue();
                this.Validate();
            },
            deep: true
        }
    },
    mounted: function () {
        this.ActionPath = "../Orders/";
        this.fetchOrder();
        this.SaveOldValue();
        this.fetchCountrys();
        this.fetchAvaialbeCustomers();
    }
});