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
    /// Логика взаимодействия для MusicRecordsListWindow.xaml
    /// </summary>
    public partial class MusicRecordsListWindow : BaseWindow
    {
        private readonly MusicStoreContext _context;
        private ObservableCollection<MusicRecord> displayedMusicRecords;
        private ObservableCollection<int> pages;
        private MusicRecord selectedMusicRecord;
        private int currentPage;
        private int maxPage;
        private string selectedType;
        private string search;
        private SortModel selectedSort;
        private int itemsPerPage;
        private Visibility workerVisibility;
        private Visibility adminVisibility;
        public MusicRecordsListWindow()
        {
            _context = new MusicStoreContext();
            InitializeComponent();
            LoadData();
            DataContext = this;
        }
        public List<MusicRecord> MusicRecords { get; set; }
        public List<Genre> Genres { get; set; }
        public List<string> Types { get; set; }
        public List<SortModel> SortParams { get; set; }
        public ObservableCollection<MusicRecord> DisplayedMusicRecords
        {
            get { return displayedMusicRecords; }
            set { displayedMusicRecords = value; OnPropertyChanged(); }
        }
        public ObservableCollection<int> DisplayedPages => new ObservableCollection<int>(Pages.Take(maxPage > 0 ? maxPage : 1));
        public ObservableCollection<int> Pages { get => pages; set { pages = value; OnPropertyChanged(); } }
        public MusicRecord SelectedMusicRecord { get { return selectedMusicRecord; } set { selectedMusicRecord = value; OnPropertyChanged(); } }
        public string SelectedType { get { return selectedType; } set { selectedType = value; OnPropertyChanged(); RefreshMusicRecords(); } }
        public string Search { get { return search; } set { search = value; OnPropertyChanged(); RefreshMusicRecords(); } }
        public int CurrentPage { get => currentPage; set { currentPage = value; OnPropertyChanged(); } }
        public SortModel SelectedSort { get { return selectedSort; } set { selectedSort = value; OnPropertyChanged(); RefreshMusicRecords(); } }
        public Visibility AdminVisibility { get => adminVisibility; set { adminVisibility = value; OnPropertyChanged(); } }
        public Visibility WorkerVisibility { get => workerVisibility; set { workerVisibility = value; OnPropertyChanged(); } }
        private void LoadData()
        {
            itemsPerPage = 10;
            search = string.Empty;
            Pages = new ObservableCollection<int>();

            if (UserService.Instance.Employee.Id == 1)
            {
                AdminVisibility = Visibility.Collapsed;
                WorkerVisibility = Visibility.Visible;
            }
            else
            {
                AdminVisibility = Visibility.Visible;
                WorkerVisibility = Visibility.Collapsed;
            }

            LoadGenres();
            LoadTypes();
            LoadSort();
            LoadMusicRecords();
            LoadEvents();
            LoadPages();
            OnPropertyChanged(nameof(SelectedType));
            OnPropertyChanged(nameof(SelectedSort));
            OnPropertyChanged(nameof(Search));
        }
        private void LoadTypes()
        {
            Types = new List<string>() { "Все типы" };
            var typesInDb = _context.MusicRecordType.ToList();
            foreach (var type in typesInDb)
            {
                Types.Add(type.Title);
            }
            selectedType = Types.FirstOrDefault();
        }
        private void LoadSort()
        {
            SortParams = new List<SortModel>
            {
                new SortModel("Название", "Title", true),
                new SortModel("Стоимость", "Price", true),
                new SortModel("Количество на складе", "CountInStock", true),
                new SortModel("Количество в зале", "CountInHallInt", true)
            };
            foreach (var sort in SortParams)
            {
                sort.PropertyChanged += SortDirection_PropertyChanged;
            }
            selectedSort = SortParams.FirstOrDefault();
        }

        private void SortDirection_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshMusicRecords();
        }

        private void LoadGenres()
        {
            Genres = _context.Genre.OrderBy(p => p.Title).ToList();
            foreach (var genre in Genres)
            {
                genre.PropertyChanged += Genre_PropertyChanged;
            }
        }
        private void LoadPages()
        {
            foreach (int index in Enumerable.Range(1, maxPage))
            {
                Pages.Add(index);
            }
            CurrentPage = Pages.FirstOrDefault();
        }
        private void LoadMusicRecords()
        {
            MusicRecords = _context.MusicRecord.ToList();
           
            DisplayedMusicRecords = new ObservableCollection<MusicRecord>();
            RefreshMusicRecords();
        }

        private void LoadEvents()
        {
            foreach (var musicRecord in MusicRecords)
            {
                musicRecord.MenuItems[0].Click += setInHall_Click;
                musicRecord.MenuItems[1].Click += removeFromHall_Click;
                musicRecord.MenuItems[2].Click += Edit_Click;
                musicRecord.MenuItems[3].Click += Remove_Click;

                if (UserService.Instance.Employee.RoleId == 2)
                {
                    musicRecord.MenuItems[0].Visibility = Visibility.Collapsed;
                    musicRecord.MenuItems[1].Visibility = Visibility.Collapsed;
                    musicRecord.MenuItems[2].Visibility = Visibility.Visible;
                    musicRecord.MenuItems[3].Visibility = Visibility.Visible;
                }
                else
                {
                    musicRecord.MenuItems[0].Visibility = Visibility.Visible;
                    musicRecord.MenuItems[1].Visibility = Visibility.Visible;
                    musicRecord.MenuItems[2].Visibility = Visibility.Collapsed;
                    musicRecord.MenuItems[3].Visibility = Visibility.Collapsed;
                }
            }
        }

        private void RefreshMusicRecords()
        {
            List<MusicRecord> list = SortMusicRecords();

            list = list.Where(p => p.Title.ToLower().Contains(Search.ToLower()) || p.Label.ToLower().Contains(Search.ToLower()) || p.Performers.ToLower().Contains(Search.ToLower())).ToList();

            list = list.Where(p => SelectedType != "Все типы" ? p.MusicRecordType.Title.Contains(SelectedType) : true).ToList();

            var selectedGenred = Genres.Where(p => p.IsSelected == true).ToList();
            if (selectedGenred.Count() > 0)
            {
                list = list.Where(p => selectedGenred.Any(g => p.Genre.Contains(g))).ToList();
            }

            if (maxPage != (int)Math.Ceiling((float)list.Count / (float)itemsPerPage))
            {
                maxPage = (int)Math.Ceiling((float)list.Count / (float)itemsPerPage);
                OnPropertyChanged(nameof(DisplayedPages));
            }
            list = list.Skip((CurrentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
            //////selectedGenred.Clear();
            if (DisplayedMusicRecords != null)
            {

                DisplayedMusicRecords.Clear();
                //    //GC.Collect();
                //    //GC.WaitForPendingFinalizers();
            }
            foreach (var item in list)
            {
                DisplayedMusicRecords.Add(item);
            }
            if (CurrentPage > maxPage)
            {
                CurrentPage = maxPage;
            }
            //DisplayedMusicRecords = new ObservableCollection<MusicRecord>(list);
            //list.Clear();
        }



        private List<MusicRecord> SortMusicRecords()
        {
            if (SelectedSort.IsAscending)
            {
                return MusicRecords.OrderBy(p => p.GetProperty(SelectedSort.Property)).ToList();
            }
            else
            {
                return MusicRecords.OrderByDescending(p => p.GetProperty(SelectedSort.Property)).ToList();
            }
        }

        private void Genre_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            RefreshMusicRecords();
        }

        private void ListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as ListView).SelectedItem = null;
        }

        private void paginator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshMusicRecords();
        }

        private void toFirts_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != 1)
            {
                CurrentPage = 1;
            }
        }

        private void back_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage > 0)
            {
                CurrentPage--;
            }
        }

        private void forward_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage + 1 <= maxPage)
            {
                CurrentPage++;
            }
        }

        private void toLast_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentPage != maxPage)
            {
                CurrentPage = maxPage;
            }
        }

        private async void setInHall_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowInputAsync("Сколько выставить?", "Введите количество для выставки товара", new MetroDialogSettings { DialogResultOnCancel = MessageDialogResult.Canceled });
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
                if (res > SelectedMusicRecord.CountInStock)
                {
                    await this.ShowMessageAsync("Не хватает пластинок на складе", "Введено количество пластинок, которго нет на складе. Введите другое количество.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                    return;
                }
                if (SelectedMusicRecord.MusicRecordsInTheHall == null)
                {
                    SelectedMusicRecord.MusicRecordsInTheHall = new MusicRecordsInTheHall() { MusicRecordId = SelectedMusicRecord.Id, Count = res };
                }
                else
                {
                    SelectedMusicRecord.MusicRecordsInTheHall.Count += res;
                }
                SelectedMusicRecord.CountInStock -= res;

                _context.SaveChanges();
                RefreshMusicRecords();
            }
            else
            {
                await this.ShowMessageAsync("Неккоректный ввод", "Введено неверное значение для количества, введите его повторно", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
            }
        }

        private async void removeFromHall_Click(object sender, RoutedEventArgs e)
        {
            var result = await this.ShowInputAsync("Сколько выставить?", "Введите количество для выставки товара", new MetroDialogSettings { DialogResultOnCancel = MessageDialogResult.Canceled });
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
                if (SelectedMusicRecord.MusicRecordsInTheHall == null || SelectedMusicRecord.MusicRecordsInTheHall.Count < res)
                {
                    await this.ShowMessageAsync("Не хватает платинок в зале!", "Введено количество пластинок, которго нет в зале. Введите другое количество.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                    return;
                }
                else
                {
                    SelectedMusicRecord.MusicRecordsInTheHall.Count -= res;
                    SelectedMusicRecord.CountInStock += res;

                    _context.SaveChanges();
                    RefreshMusicRecords();
                }
            }
            else
            {
                await this.ShowMessageAsync("Неккоректный ввод", "Введено неверное значение для количества, введите его повторно", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
            }
        }

        private void order_Click(object sender, RoutedEventArgs e)
        {
            var orderWindow = new OrderWindow(_context);
            if (orderWindow.ShowDialog() == true)
            {
                LoadMusicRecords();
            }
        }

        private void delivery_Click(object sender, RoutedEventArgs e)
        {
            var deliveryWindow = new DeliveryWindow(_context);
            if (deliveryWindow.ShowDialog() == true)
            {
                LoadMusicRecords();
            }
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            UserService.Instance.Logout();
            var loginWidnow = new LoginWindow();
            loginWidnow.Show();
            this.Close();
        }

        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var adminWidnow = new AdminPanelWindow();
            adminWidnow.Show();
            this.Close();
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedMusicRecord != null)
            {
                var musicRecord = _context.MusicRecord.Find(SelectedMusicRecord.Id);
                if (musicRecord != null)
                {
                    if (musicRecord.MusicRecordInOrder.Count == 0)
                    {

                        _context.MusicRecord.Remove(SelectedMusicRecord);
                        _context.SaveChanges();
                        await this.ShowMessageAsync("Удаление успешно!", "Пластинка была успешно удалена.", MessageDialogStyle.Affirmative);
                    }
                    else
                    {
                        await this.ShowMessageAsync("Невозможно удалить!", "Данная пластинка имеет историю продаж. Товар с историей продаж удалить нельзя.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                    }
                }
            }
        }

        private void Edit_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var musicRecordWindow = new MusicRecordWindow(_context, SelectedMusicRecord);
                if (musicRecordWindow.ShowDialog() == true)
                {
                    LoadMusicRecords();
                }
            }
            catch(Exception ex)
            {

            }
        }

        private void Add_Click(object sender, RoutedEventArgs e)
        {
            var musicRecordWindow = new MusicRecordWindow(_context);
            if (musicRecordWindow.ShowDialog() == true)
            {
                LoadMusicRecords();
            }
        }
    }
}
