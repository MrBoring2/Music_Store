using Music_Store.Views.Windows;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace Music_Store
{
    /// <summary>
    /// Логика взаимодействия для App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            var musicRecordsWindow = new MusicRecordsListWindow();
            musicRecordsWindow.Show();
            base.OnStartup(e);

        }
    }
}
