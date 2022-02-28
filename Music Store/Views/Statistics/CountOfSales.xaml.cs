using LiveCharts;
using LiveCharts.Wpf;
using Music_Store.Data;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    /// Логика взаимодействия для CountOfSales.xaml
    /// </summary>
    public partial class CountOfSales : BasePage
    {
        private string mostSales;
        private MusicStoreContext _context;
        private SeriesCollection seriesCollection;
        private ObservableCollection<string> labels;
        public CountOfSales(MusicStoreContext context, DateTime start, DateTime end)
        {
            InitializeComponent();
            _context = context;
            GenerateGenresStatistics(start, end);
            DataContext = this;
        }
        public Func<double, string> Formatter { get; set; }

        public SeriesCollection SeriesCollection { get => seriesCollection; set { seriesCollection = value; OnPropertyChanged(); } }
        public string MostSales { get => mostSales; set { mostSales = value; OnPropertyChanged(); } }
        public ObservableCollection<string> Labels { get => labels; set { labels = value; OnPropertyChanged(); } }
        private void GenerateGenresStatistics(DateTime start, DateTime end)
        {
            SeriesCollection = new SeriesCollection();
            var types = _context.MusicRecordType.ToList();
            var genres = _context.Genre.OrderBy(p => p.MusicRecord.Count).ToList();
            Dictionary<string, int> totalSales = new Dictionary<string, int>();
            MostSales = "Всего продано: ";
            foreach (var genre in genres)
            {
                var values = new ChartValues<double>();

                foreach (var type in types)
                {

                    int sale = 0;
                    foreach (var musicRecord in genre.MusicRecord.Where(p => p.TypeId == type.Id))
                    {
                        sale += musicRecord.MusicRecordInOrder.Where(p => p.Order.Date <= end && p.Order.Date >= start).Sum(p => p.CountInOrder);

                    }

                    if (totalSales.ContainsKey(type.Title))
                    {
                        totalSales[type.Title] += sale;
                    }
                    else
                    {
                        totalSales.Add(type.Title, sale);
                    }

                    values.Add(sale);
                }

                SeriesCollection.Add(new ColumnSeries
                {
                    Title = genre.Title,
                    LabelPoint = point => $"Продаж: {point.Y}",
                    Values = values
                });
            }
            Labels = new ObservableCollection<string>();
            foreach (var item in types)
            {
                Labels.Add(item.Title);
            }

            foreach (var item in totalSales)
            {
                if (item.Key != totalSales.LastOrDefault().Key)
                {
                    MostSales += $"{item.Key} - {item.Value} шт., ";
                }
                else
                {
                    MostSales += $"{item.Key} - {item.Value} шт.";
                }
            }

            Formatter = value => value.ToString("N");
        }
    }
}
