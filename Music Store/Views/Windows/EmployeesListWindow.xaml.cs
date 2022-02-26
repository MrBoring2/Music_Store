using MahApps.Metro.Controls.Dialogs;
using Music_Store.Data;
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
    /// Логика взаимодействия для EmployeesListWindow.xaml
    /// </summary>
    public partial class EmployeesListWindow : BaseWindow
    {
        private readonly MusicStoreContext _context;
        private ObservableCollection<Employee> displayedEmployees;
        private ObservableCollection<int> pages;
        private Employee selectedEmployee;
        private int currentPage = 1;
        private int maxPage;
        private string selectedRole;
        private string search;
        private int itemsPerPage;
        public EmployeesListWindow()
        {
            _context = new MusicStoreContext();
            InitializeComponent();
            LoadData();
            DataContext = this;
        }
        public List<Employee> Employees { get; set; }
        public List<string> Roles { get; set; }
        public ObservableCollection<Employee> DisplayedEmployees
        {
            get { return displayedEmployees; }
            set { displayedEmployees = value; OnPropertyChanged(); }
        }
        public ObservableCollection<int> DisplayedPages => new ObservableCollection<int>(Pages.Take(maxPage > 0 ? maxPage : 1));
        public ObservableCollection<int> Pages { get => pages; set { pages = value; OnPropertyChanged(); } }
        public Employee SelectedEmployee { get { return selectedEmployee; } set { selectedEmployee = value; OnPropertyChanged(); } }
        public string SelectedRole { get { return selectedRole; } set { selectedRole = value; OnPropertyChanged(); RefreshEmployees(); } }
        public string Search { get { return search; } set { search = value; OnPropertyChanged(); RefreshEmployees(); } }
        public int CurrentPage { get => currentPage; set { currentPage = value; OnPropertyChanged(); } }
        private void LoadData()
        {
            itemsPerPage = 10;
            search = string.Empty;
            Pages = new ObservableCollection<int>();
            LoadRoles();
            LoadEmployees();
            LoadPages();
            OnPropertyChanged(nameof(SelectedRole));
            OnPropertyChanged(nameof(Search));
        }

        private void LoadEmployees()
        {
            Employees = _context.Employee.ToList();

            DisplayedEmployees = new ObservableCollection<Employee>();
            RefreshEmployees();
        }

        private void LoadRoles()
        {
            Roles = new List<string>() { "Все роли" };
            var rolesInDb = _context.Role.ToList();
            foreach (var role in rolesInDb)
            {
                Roles.Add(role.Title);
            }
            selectedRole = Roles.FirstOrDefault();
        }
        private void LoadPages()
        {
            foreach (int index in Enumerable.Range(1, maxPage))
            {
                Pages.Add(index);
            }
            CurrentPage = Pages.FirstOrDefault();
        }
        private void RefreshEmployees()
        {
            List<Employee> list = SortMusicRecords();

            list = list.Where(p => p.FullName.ToLower().Contains(Search.ToLower())).ToList();

            list = list.Where(p => SelectedRole != "Все роли" ? p.Role.Title.Contains(SelectedRole) : true).ToList();


            if (maxPage != (int)Math.Ceiling((float)list.Count / (float)itemsPerPage))
            {
                maxPage = (int)Math.Ceiling((float)list.Count / (float)itemsPerPage);
                OnPropertyChanged(nameof(DisplayedPages));
            }
            list = list.Skip((CurrentPage - 1) * itemsPerPage).Take(itemsPerPage).ToList();
            if (DisplayedEmployees != null)
            {

                DisplayedEmployees.Clear();
            }
            foreach (var item in list)
            {
                DisplayedEmployees.Add(item);
            }
            if (CurrentPage > maxPage)
            {
                CurrentPage = maxPage;
            }
        }
        private List<Employee> SortMusicRecords()
        {

            return Employees.OrderBy(p => p.FullName).ToList();

        }
        private void paginator_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshEmployees();
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
        private void Back_Click(object sender, RoutedEventArgs e)
        {
            var adminWidnow = new AdminPanelWindow();
            adminWidnow.Show();
            this.Close();
        }
        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var employeeWindow = new EmployeeWindow(_context);
            if (employeeWindow.ShowDialog() == true)
            {
                LoadEmployees();
                await this.ShowMessageAsync("Сотрудник добавлен!", "СОрудник успешно добавлен в базу данных.", MessageDialogStyle.Affirmative);
            }
        }

        private async void Edit_Click(object sender, RoutedEventArgs e)
        {
            var employeeWindow = new EmployeeWindow(_context, SelectedEmployee);
            if (employeeWindow.ShowDialog() == true)
            {
                LoadEmployees();

                await this.ShowMessageAsync("Сотрудник обновлён!", "Данные сотрудника успешно обновлены.", MessageDialogStyle.Affirmative);
            }
        }

        private async void Remove_Click(object sender, RoutedEventArgs e)
        {
            if (SelectedEmployee != null)
            {
                var employee = _context.Employee.Find(SelectedEmployee.Id);
                if (employee != null)
                {
                    if (employee.Id == UserService.Instance.Employee.Id)
                    {
                        await this.ShowMessageAsync("Невозможно удалить!", "Вы пытаетесь удалить текущего пользователя (себя).", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                        return;
                    }
                    if (employee.Order.Count == 0)
                    {

                        _context.Employee.Remove(SelectedEmployee);
                        _context.SaveChanges();
                        await this.ShowMessageAsync("Удаление успешно!", "Сотрудник был успешно удален.", MessageDialogStyle.Affirmative);
                    }
                    else
                    {
                        await this.ShowMessageAsync("Невозможно удалить!", "У данного сотрудника енсть история продаж. Товар с историей продаж удалить нельзя.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                    }
                }
            }
        }
    }
}
