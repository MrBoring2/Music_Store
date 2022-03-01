using LiveCharts;
using LiveCharts.Wpf;
using MahApps.Metro.Controls.Dialogs;
using Music_Store.Data;
using Music_Store.Views.Statistics;
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
using System.Windows.Shapes;

namespace Music_Store.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для StaticsticsWindow.xaml
    /// </summary>
    public partial class StaticsticsWindow : BaseWindow
    {
        private BasePage statisticsPage;
        private readonly MusicStoreContext _context;
        private string selectedStatistics;

        private DateTime startDate;
        private DateTime endDate;
        public StaticsticsWindow(MusicStoreContext context)
        {
            _context = context;
            InitializeComponent();
            LoadStatistics();
            StartDate = DateTime.Now - new TimeSpan(24 * 7, 0, 0);
            EndDate = DateTime.Now;
            DataContext = this;
        }
        public List<string> StatisticsTypes { get; set; }
        public string SelectedStatistics { get => selectedStatistics; set { selectedStatistics = value; OnPropertyChanged(); } }

        public DateTime StartDate { get => startDate; set { startDate = value; OnPropertyChanged(); } }

        public DateTime EndDate
        {
            get => endDate; set { endDate = value; OnPropertyChanged(); }
        }
        public BasePage StatisticsPage { get => statisticsPage; set { statisticsPage = value; OnPropertyChanged(); } }
        private void LoadStatistics()
        {
            StatisticsTypes = new List<string>
            {
                "Количество продаж",
                "Средняя стоимость продаж"
            };
            SelectedStatistics = StatisticsTypes.FirstOrDefault();
        }
        private async void GenerateStatistics_Click(object sender, RoutedEventArgs e)
        {
            if (StartDate.Date <= EndDate.Date && EndDate.Date <= DateTime.Now.Date)
            {
                if (SelectedStatistics == "Количество продаж")
                {
                    StatisticsPage = new CountOfSalesPage(_context, StartDate, EndDate);
                }
                else if (SelectedStatistics == "Средняя стоимость продаж")
                {
                    StatisticsPage = new AveragePriceOfSalesPage(_context, StartDate, EndDate);
                }
            }
            else
            {
                await this.ShowMessageAsync("Ошибка формирования!", "Выставлена неккоректная дата начала или конца периода!", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
            }
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}

