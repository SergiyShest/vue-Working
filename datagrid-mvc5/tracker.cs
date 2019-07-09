using System;
using System.Collections.Generic;
using System.Threading;
using DevExtreme.MVC.Demos.Hubs;
using Microsoft.AspNet.SignalR;
using Microsoft.AspNet.SignalR.Hubs;

namespace DevExtreme.MVC.Demos.Models.SignalR
{
    public class StockTicker
    {
        public readonly static StockTicker Instance = new StockTicker(GlobalHost.ConnectionManager.GetHubContext<LiveUpdateSignalRHub>().Clients);

        private readonly List<Stock> _stocks = new List<Stock>();
        private IHubConnectionContext<dynamic> Clients { get; set; }

        private readonly TimeSpan _updateInterval = TimeSpan.FromMilliseconds(500);
        private readonly Random _updateOrNotRandom = new Random();

        private readonly Timer _timer;

        private readonly object _updateStockPricesLock = new object();

        static readonly Random random = new Random();

        private StockTicker(IHubConnectionContext<dynamic> clients)
        {
            Clients = clients;

            _stocks = GenerateStocks();

            _timer = new Timer(UpdateStockPrices, null, _updateInterval, _updateInterval);

        }

        public List<Stock> GetAllStocks()
        {
            return _stocks;
        }

        static List<Stock> GenerateStocks()
        {
            return new List<Stock> {
                new Stock(37.95M) { Symbol = "MSFT", DayOpen=36.5M, LastUpdate = DateTime.Now },
                new Stock(24.85M) { Symbol = "INTC", DayOpen=24.9M, LastUpdate = DateTime.Now },
                new Stock(22.99M){ Symbol = "CSCO", DayOpen=22.7M, LastUpdate = DateTime.Now },
                new Stock(30.71M){ Symbol = "SIRI", DayOpen=30.7M, LastUpdate = DateTime.Now },
                new Stock(58.73M){ Symbol = "AAPL", DayOpen=54.9M, LastUpdate = DateTime.Now },
                new Stock(110M){ Symbol = "HOKU", DayOpen=121.2M, LastUpdate = DateTime.Now },
                new Stock(38.11M){ Symbol = "ORCL", DayOpen=37.9M, LastUpdate = DateTime.Now },
                new Stock(17.61M) { Symbol = "AMAT", DayOpen=17.5M, LastUpdate = DateTime.Now },
                new Stock(40.80M){ Symbol = "YHOO", DayOpen=39.9M, LastUpdate = DateTime.Now },
                new Stock(31.85M){ Symbol = "LVLT", DayOpen=32.9M, LastUpdate = DateTime.Now },
                new Stock(20.63M){ Symbol = "DELL", DayOpen=17.9M, LastUpdate = DateTime.Now },
                new Stock(63.70M) { Symbol = "GOOG", DayOpen=55.9M, LastUpdate = DateTime.Now }
            };
        }
        private void UpdateStockPrices(object state)
        {
            lock (_updateStockPricesLock)
            {
                foreach (var stock in _stocks)
                {
                    if (TryUpdateStockPrice(stock))
                    {
                        BroadcastStockPrice(stock);
                    }
                }
            }
        }

        private bool TryUpdateStockPrice(Stock stock)
        {
            var r = _updateOrNotRandom.NextDouble();
            if (r > .1)
            {
                return false;
            }

            stock.Update();
            return true;
        }

        private void BroadcastStockPrice(Stock stock)
        {
            Clients.All.updateStockPrice(stock);
        }
    }

    public class Stock
    {
        public Stock(decimal price)
        {
            Price = price;
            DayMax = price;
            DayMin = price;
            _initPrice = Price;
        }
        public string Symbol { get; set; }
        public decimal Price { get; set; }
        public decimal DayMax { get; set; }
        public decimal DayMin { get; set; }
        public decimal DayOpen { get; set; }
        public DateTime LastUpdate { get; set; }
        public decimal Change
        {
            get
            {
                return Price - DayOpen;
            }
        }
        public double PercentChange
        {
            get
            {
                return (double)Math.Round(Change * 100 / DayOpen, 2);
            }
        }

        decimal _initPrice;
        public void Update()
        {
            bool isNewDay = LastUpdate.Day != DateTime.Now.Day;
            decimal change = GenerateChange();
            decimal newPrice = _initPrice + _initPrice * change;

            Price = newPrice;
            LastUpdate = DateTime.Now;
            if (Price > DayMax || isNewDay)
                DayMax = Price;
            if (Price < DayMin || isNewDay)
                DayMin = Price;
        }

        static Random random = new Random();

        decimal GenerateChange()
        {
            return (decimal)random.Next(-200, 200) / 10000;
        }
    }
}