using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Music_Store.Models
{
    public class StatisticsAveragePriceModel
    {
        public StatisticsAveragePriceModel(DateTime day, int count, decimal averagePrice)
        {
            Day = day;
            Count = count;
            AveragePrice = averagePrice;
        }

        public DateTime Day { get; set; }
        public int Count { get; set; }
        public decimal AveragePrice { get; set; }
        public string GetCount => Count > 0 ? $"{Count} шт." : "Не было продаж";
        public string GetAveragePrice => AveragePrice > 0 ? $"{Math.Round(AveragePrice, 2)} ₽" : "Не было продаж";
    }
}
