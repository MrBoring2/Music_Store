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
    /// Логика взаимодействия для AdminPanelWindow.xaml
    /// </summary>
    public partial class AdminPanelWindow : BaseWindow
    {
        public AdminPanelWindow()
        {
            InitializeComponent();
            DataContext = this;
        }

        private void OperMusicRecords_Click(object sender, RoutedEventArgs e)
        {
            var musicRecordsWindow = new MusicRecordsListWindow();
            musicRecordsWindow.Show();
            this.Close();
        }

        private void OpenEmployees_Click(object sender, RoutedEventArgs e)
        {
            var empoloyyesListWindow = new EmployeesListWindow();
            empoloyyesListWindow.Show();
            this.Close();
        }

        private void Exit_Click(object sender, RoutedEventArgs e)
        {
            UserService.Instance.Logout();
            var loginWidnow = new LoginWindow();
            loginWidnow.Show();
            this.Close();
        }
    }
}
