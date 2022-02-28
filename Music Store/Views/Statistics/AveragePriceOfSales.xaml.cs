using Music_Store.Data;
using Music_Store.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Music_Store.Views.Statistics
{
    /// <summary>
    /// Логика взаимодействия для AveragePriceOfSales.xaml
    /// </summary>
    public partial class AveragePriceOfSales : BasePage
    {
        private MusicStoreContext _context;
        private ObservableCollection<StatisticsAveragePriceModel> statisticsAveragePriceList;
        public AveragePriceOfSales(MusicStoreContext context, DateTime start, DateTime end)
        {
            _context = context;
            InitializeComponent();
            LoadStatistics(start, end);
            DataContext = this;
        }
        public ObservableCollection<StatisticsAveragePriceModel> StatisticsAveragePriceList { get => statisticsAveragePriceList; set { statisticsAveragePriceList = value; OnPropertyChanged(); } }
        private void LoadStatistics(DateTime start, DateTime end)
        {
            StatisticsAveragePriceList = new ObservableCollection<StatisticsAveragePriceModel>();
            for (DateTime i = start; i.Date <= end.Date; i = i.AddDays(1))
            {
                var orders = _context.Order.Where(p => p.Date.Day == i.Day && p.Date.Month == i.Month && p.Date.Year == i.Year).ToList();
                decimal averagePrice = 0;
                foreach (var order in orders)
                {
                    var a = order.MusicRecordInOrder.Sum(d => d.MusicRecord.Price * d.CountInOrder);
                    averagePrice += order.MusicRecordInOrder.Sum(d => d.MusicRecord.Price * d.CountInOrder);
                }

                if (orders.Count > 0)
                {
                    averagePrice /= orders.Count;
                }

                StatisticsAveragePriceList.Add(new StatisticsAveragePriceModel(i, orders.Count, averagePrice));
            }
        }
    }
}
