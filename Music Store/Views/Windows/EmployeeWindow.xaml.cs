using MahApps.Metro.Controls.Dialogs;
using Music_Store.Data;
using System;
using System.Collections.Generic;
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
    /// Логика взаимодействия для EmployeeWindow.xaml
    /// </summary>
    public partial class EmployeeWindow : BaseWindow
    {
        private readonly MusicStoreContext _context;
        private bool isEdit;
        private string fullName;
        private string phoneNumber;
        private string login;
        private string password;
        private Role selectedRole;
        public EmployeeWindow(MusicStoreContext context)
        {
            _context = context;
            isEdit = false;
            Employee = new Employee();
            InitializeComponent();
            LoadRoles();
            DataContext = this;
        }


        public EmployeeWindow(MusicStoreContext context, Employee employee) : this(context)
        {
            isEdit = true;
            Employee = employee;
            SelectedRole = Roles.FirstOrDefault(p => p.Id == Employee.RoleId);
            FullName = Employee.FullName;
            PhoneNumber = Employee.PhoneNumber;
            Login = Employee.Login;
            Password = Employee.Password;
        }
        public Employee Employee { get; set; }
        public List<Role> Roles { get; set; }
        public Role SelectedRole { get => selectedRole; set { selectedRole = value; OnPropertyChanged(); } }
        public string FullName { get => fullName; set { fullName = value; OnPropertyChanged(); } }
        public string PhoneNumber { get => phoneNumber; set { phoneNumber = value; OnPropertyChanged(); } }
        public string Login { get => login; set { login = value; OnPropertyChanged(); } }
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }


        private void LoadRoles()
        {
            Roles = _context.Role.ToList();
            SelectedRole = Roles.FirstOrDefault();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            if (Validate())
            {
                Employee.Login = Login;
                Employee.Password = Password;
                Employee.PhoneNumber = PhoneNumber;
                Employee.FullName = FullName;
                Employee.RoleId = SelectedRole.Id;

                if (isEdit)
                {
                    var employee = _context.Employee.FirstOrDefault(p => p.Id == Employee.Id);
                    if (employee != null)
                    {
                        //_context.MusicRecord.Attach(MusicRecord);
                        _context.Entry(employee).CurrentValues.SetValues(Employee);
                    }
                }
                else
                {
                    var employee = new Employee()
                    {
                        FullName = Employee.FullName,
                        Login = Employee.Login,
                        Password = Employee.Password,
                        PhoneNumber = Employee.PhoneNumber,
                        RoleId = Employee.RoleId,
                    };
                    _context.Employee.Add(employee);
                }
                _context.SaveChanges();
                DialogResult = true;
            }
            else
            {
                await this.ShowMessageAsync("Ошибка проверки данных!", "Не все данные подходят. Проверьте, все ли поля заполнены верно: в телефоне 11 символов и все поля не пустые.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                return;
            }
        }

        private bool Validate()
        {
            return !string.IsNullOrEmpty(FullName) &&
                !string.IsNullOrEmpty(PhoneNumber) &&
                !string.IsNullOrEmpty(Login) &&
                !string.IsNullOrEmpty(Password) &&
                PhoneNumber.Length == 11;

        }
    }
}
