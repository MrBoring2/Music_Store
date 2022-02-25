using MahApps.Metro.Controls.Dialogs;
using Music_Store.Data;
using Music_Store.Services;
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
    /// Логика взаимодействия для LoginWindow.xaml
    /// </summary>
    public partial class LoginWindow : BaseWindow
    {
        private bool btnEnable;
        private string login;
        private string password;
        public LoginWindow()
        {
            InitializeComponent();
            btnEnable = true;
            DataContext = this;
        }
        public string Login { get => login; set { login = value; OnPropertyChanged(); } }
        public string Password { get => password; set { password = value; OnPropertyChanged(); } }
        public bool BtnEnable { get => btnEnable; set { btnEnable = value; OnPropertyChanged(); } }
        private async void Login_Click(object sender, RoutedEventArgs e)
        {
            BtnEnable = false;
            using (var db = new MusicStoreContext())
            {
                Employee user = null;
                await Task.Run(() => { user = db.Employee.FirstOrDefault(p => p.Login.Equals(login) && p.Password.Equals(password)); });
                if (user != null)
                {
                    UserService.Instance.SetEmployee(user);
                    if (user.RoleId == 1)
                    {
                        var musicRecordWindow = new MusicRecordsListWindow();
                        musicRecordWindow.Show();
                        this.Close();
                    }

                    BtnEnable = true;
                }
                else
                {
                    await this.ShowMessageAsync("Неверный логин или пароль!", "Введён неверный логин или ппароль. Проверьте вводимые данные.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                    BtnEnable = true;
                }
            }
        }
    }
}
