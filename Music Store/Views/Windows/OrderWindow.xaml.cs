using MahApps.Metro.Controls.Dialogs;
using Music_Store.Data;
using Music_Store.Models;
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
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : BaseWindow
    {
        private readonly MusicStoreContext _context;
        private MusicRecord selectedMusicRecord;
        private MusicRecordOrderAndDelivery selectedMusicRecordInOrder;
        public OrderWindow()
        {
            _context = new MusicStoreContext();
            InitializeComponent();
            LoadMusicRecords();
            DataContext = this;
        }

        private void LoadMusicRecords()
        {
            MusicRecords = new ObservableCollection<MusicRecord>(_context.MusicRecord);
            MusicRecordsInOrder = new ObservableCollection<MusicRecordOrderAndDelivery>();
        }

        public ObservableCollection<MusicRecord> MusicRecords { get; set; }
        public ObservableCollection<MusicRecordOrderAndDelivery> MusicRecordsInOrder { get; set; }
        public MusicRecord SelectedMusicRecord { get { return selectedMusicRecord; } set { selectedMusicRecord = value; OnPropertyChanged(); } }
        public MusicRecordOrderAndDelivery SelectedMusicRecordInOrder { get { return selectedMusicRecordInOrder; } set { selectedMusicRecordInOrder = value; OnPropertyChanged(); } }

        private async void addToOrder_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMusicRecord != null)
            {
                var result = await this.ShowInputAsync("Количество в заказе?", "Введите количество пластинок для заказа", new MetroDialogSettings { DialogResultOnCancel = MessageDialogResult.Canceled });
                if (result == null)
                {
                    return;
                }
                if (int.TryParse(result, out int res))
                {
                    if (res <= 0)
                    {
                        await this.ShowMessageAsync("Количество должно быть больше 0!", "Введено отрицательное или равно нулю количество. Введите положительное число.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                        return;
                    }
                    else
                    {
                        if (MusicRecordsInOrder.FirstOrDefault(p => p.MusicRecord.Id == SelectedMusicRecord.Id) == null)
                        {
                            MusicRecordsInOrder.Add(new MusicRecordOrderAndDelivery(SelectedMusicRecord, res));
                        }
                        else
                        {
                            await this.ShowMessageAsync("Данная пластинка уже есть в заказе!", "Выбрана пластинка, которая уже существует в заказе. Выберите другую.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                            return;
                        }
                    }
                }
            }
        }

        private void removeFromOrder_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMusicRecordInOrder != null)
            {
                MusicRecordsInOrder.Remove(SelectedMusicRecordInOrder);
            }
        }

        private void save_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
