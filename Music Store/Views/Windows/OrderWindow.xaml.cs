using MahApps.Metro.Controls.Dialogs;
using Music_Store.Data;
using Music_Store.Models;
using Music_Store.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Music_Store.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для OrderWindow.xaml
    /// </summary>
    public partial class OrderWindow : BaseWindow
    {
        private readonly MusicStoreContext _context;
        private string search;
        private MusicRecord selectedMusicRecord;
        private decimal totalPrice;
        private MusicRecordOrderAndDelivery selectedMusicRecordInOrder;

        public OrderWindow(MusicStoreContext context)
        {
            _context = context;
            search = string.Empty;
            InitializeComponent();
            LoadMusicRecords();
            OnPropertyChanged(nameof(Search));
            DataContext = this;
        }

        /// <summary>
        /// Загрузка музыкальных пластинок
        /// </summary>
        private void LoadMusicRecords()
        {
            MusicRecords = new ObservableCollection<MusicRecord>(_context.MusicRecord.OrderBy(p => p.Title));
            MusicRecordsInOrder = new ObservableCollection<MusicRecordOrderAndDelivery>();
            OnPropertyChanged(nameof(DisplayedMusicRecords));
        }
        public IEnumerable<MusicRecord> DisplayedMusicRecords => MusicRecords.Where(p => p.Title.ToLower().Contains(Search.ToLower()) || p.Label.ToLower().Contains(Search.ToLower()));
        public ObservableCollection<MusicRecord> MusicRecords { get; set; }
        public ObservableCollection<MusicRecordOrderAndDelivery> MusicRecordsInOrder { get; set; }
        public MusicRecord SelectedMusicRecord { get { return selectedMusicRecord; } set { selectedMusicRecord = value; OnPropertyChanged(); } }
        public MusicRecordOrderAndDelivery SelectedMusicRecordInOrder { get { return selectedMusicRecordInOrder; } set { selectedMusicRecordInOrder = value; OnPropertyChanged(); } }
        public decimal TotalPrice { get { return totalPrice; } set { totalPrice = value; OnPropertyChanged(); } }
        public string Search { get { return search; } set { search = value; OnPropertyChanged(); OnPropertyChanged(nameof(DisplayedMusicRecords)); } }
        /// <summary>
        /// Кнопка добавления пластинки в заказ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
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
                    if (res > SelectedMusicRecord.CountInHallInt)
                    {
                        await this.ShowMessageAsync("Введённое количество больше, чем выставлено в зале!", "Количество больше, чем доступно в зале.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                        return;
                    }
                    else
                    {
                        if (MusicRecordsInOrder.FirstOrDefault(p => p.MusicRecord.Id == SelectedMusicRecord.Id) == null)
                        {
                            MusicRecordsInOrder.Add(new MusicRecordOrderAndDelivery(SelectedMusicRecord, res));
                            TotalPrice += SelectedMusicRecord.Price * res;
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

        /// <summary>
        /// Унопка удаления пластинки из заказа
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void removeFromOrder_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMusicRecordInOrder != null)
            {
                TotalPrice -= SelectedMusicRecordInOrder.MusicRecord.Price * SelectedMusicRecordInOrder.Count;
                MusicRecordsInOrder.Remove(SelectedMusicRecordInOrder);
            }
        }
        
        /// <summary>
        /// Сохранить заказ
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void save_Click(object sender, RoutedEventArgs e)
        {
            if (MusicRecordsInOrder.Count > 0)
            {
                var order = new Order
                {
                    Date = DateTime.Now,
                    EmployeeId = UserService.Instance.Employee.Id,
                    MusicRecordInOrder = new List<MusicRecordInOrder>()
                };
                foreach (var item in MusicRecordsInOrder)
                {
                    order.MusicRecordInOrder.Add(new MusicRecordInOrder { CountInOrder = item.Count, MusicRecordId = item.MusicRecord.Id });
                }
                _context.Order.Add(order);

                foreach (var item in order.MusicRecordInOrder)
                {
                    var musicRecordInHall = _context.MusinRecordsInTheHall.FirstOrDefault(p => p.MusicRecordId == item.MusicRecordId);
                    musicRecordInHall.Count -= item.CountInOrder;
                }

                _context.SaveChanges();
                await this.ShowMessageAsync("Заказ успешно оформлен!", "Товар из зала в скором времени будет списан.", MessageDialogStyle.Affirmative);
                this.DialogResult = true;
            }
            else
            {
                await this.ShowMessageAsync("Список пластинок в заказе пуст!", "Не выбрано ни одной пластинки в заказе.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                return;
            }
        }

        /// <summary>
        /// Отмена
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void cancel_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = false;
        }
    }
}
