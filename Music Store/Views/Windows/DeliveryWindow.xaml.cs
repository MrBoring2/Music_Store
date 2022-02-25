using MahApps.Metro.Controls.Dialogs;
using Music_Store.Data;
using Music_Store.Models;
using Music_Store.Services;
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
    /// Логика взаимодействия для DeliveryWindow.xaml
    /// </summary>
    public partial class DeliveryWindow : BaseWindow
    {
        private readonly MusicStoreContext _context;
        private string search;
        private MusicRecord selectedMusicRecord;
        private MusicRecordOrderAndDelivery selectedMusicRecordInOrder;
        private Supplier selectedSupplier;

        public DeliveryWindow(MusicStoreContext context)
        {
            _context = context;
            search = string.Empty;
            InitializeComponent();
            LoadMusicRecords();
            LoadSuppliers();
            OnPropertyChanged(nameof(Search));
            DataContext = this;
        }

        private void LoadSuppliers()
        {
            Suppliers = new List<Supplier>(_context.Supplier);
            SelectedSupplier = Suppliers.FirstOrDefault();
        }

        private void LoadMusicRecords()
        {
            MusicRecords = new ObservableCollection<MusicRecord>(_context.MusicRecord.OrderBy(p => p.Title));
            MusicRecordsInOrder = new ObservableCollection<MusicRecordOrderAndDelivery>();
            OnPropertyChanged(nameof(DisplayedMusicRecords));
        }
        public IEnumerable<MusicRecord> DisplayedMusicRecords => MusicRecords.Where(p => p.Title.ToLower().Contains(Search.ToLower()) || p.Label.ToLower().Contains(Search.ToLower()));
        public ObservableCollection<MusicRecord> MusicRecords { get; set; }
        public List<Supplier> Suppliers { get; set; }
        public Supplier SelectedSupplier { get => selectedSupplier; set { selectedSupplier = value; OnPropertyChanged(); } }
        public ObservableCollection<MusicRecordOrderAndDelivery> MusicRecordsInOrder { get; set; }
        public MusicRecord SelectedMusicRecord { get { return selectedMusicRecord; } set { selectedMusicRecord = value; OnPropertyChanged(); } }
        public MusicRecordOrderAndDelivery SelectedMusicRecordInOrder { get { return selectedMusicRecordInOrder; } set { selectedMusicRecordInOrder = value; OnPropertyChanged(); } }
        public string Search { get { return search; } set { search = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayedMusicRecords)); } }
        private async void addToDelivery_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMusicRecord != null)
            {
                var result = await this.ShowInputAsync("Количество в поставке?", "Введите количество пластинок для поставки", new MetroDialogSettings { DialogResultOnCancel = MessageDialogResult.Canceled });
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

        private void removeFromDelivery_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMusicRecordInOrder != null)
            {
                MusicRecordsInOrder.Remove(SelectedMusicRecordInOrder);
            }
        }

        private async void save_Click(object sender, RoutedEventArgs e)
        {
            if (MusicRecordsInOrder.Count > 0)
            {
                var delivery = new Delivery
                {
                    Date = DateTime.Now,
                    EmployeeId = UserService.Instance.Employee.Id,
                    SupplierId = SelectedSupplier.Id,
                    MusicRecordInDelivery = new List<MusicRecordInDelivery>()
                };
                foreach (var item in MusicRecordsInOrder)
                {
                    delivery.MusicRecordInDelivery.Add(new MusicRecordInDelivery { CountInDelivery = item.Count, MusicRecordId = item.MusicRecord.Id });
                }
                _context.Delivery.Add(delivery);

                foreach (var item in delivery.MusicRecordInDelivery)
                {
                    var musicRecord = _context.MusicRecord.FirstOrDefault(p => p.Id == item.MusicRecordId);
                    musicRecord.CountInStock += item.CountInDelivery;
                }

                _context.SaveChanges();
                await this.ShowMessageAsync("Поставка успешно оформлена!", "Товар прибудет в скором времени.", MessageDialogStyle.Affirmative);

                this.DialogResult = true;
            }
            else
            {
                await this.ShowMessageAsync("Список пластинок в заказе пуст!", "Не выбрано ни одной пластинки в заказе.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                return;
            }
        }

        private void remove_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
