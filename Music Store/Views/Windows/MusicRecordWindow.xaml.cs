using MahApps.Metro.Controls.Dialogs;
using Microsoft.Win32;
using Music_Store.Data;
using System;
using System.Collections.Generic;
using System.IO;
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
    /// Логика взаимодействия для MuscRecordWindow.xaml
    /// </summary>
    public partial class MusicRecordWindow : BaseWindow
    {
        private MusicStoreContext _context;
        private bool isEdit;
        private string musicRecordTitle;
        private string label;
        private DateTime releaseDate;
        private string performeres;
        private decimal price;
        private int countInStock;
        private string treckList;
        private byte[] image;
        private MusicRecordType selectedType;
        public MusicRecordWindow(MusicStoreContext context)
        {
            _context = context;
            MusicRecord = new MusicRecord();
            isEdit = false;
            InitializeComponent();
            LoadGenres();
            LoadTypes();
            ReleaseDate = DateTime.Now;
            DataContext = this;
        }
        public MusicRecordWindow(MusicStoreContext context, MusicRecord musicRecord) : this(context)
        {
            if (musicRecord != null)
            {
                isEdit = true;
                MusicRecord = musicRecord;
                MusicRecordTitle = MusicRecord.Title;
                Label = MusicRecord.Label;
                ReleaseDate = MusicRecord.ReleaseDate;
                Performers = MusicRecord.Performers;
                Price = MusicRecord.Price;
                CountInStock = MusicRecord.CountInStock;
                Image = MusicRecord.Image;
                treckListDocument.AppendText(MusicRecord.TreckList);
                SelectedType = Types.FirstOrDefault(p => p.Id == MusicRecord.TypeId);

                LoadRecordGenres();
            }
        }
        public MusicRecord MusicRecord { get; set; }
        public List<Genre> Genres { get; set; }
        public List<MusicRecordType> Types { get; set; }
        public string MusicRecordTitle { get => musicRecordTitle; set { musicRecordTitle = value; OnPropertyChanged(); } }
        public string Label { get => label; set { label = value; OnPropertyChanged(); } }
        public DateTime ReleaseDate { get => releaseDate; set { releaseDate = value; OnPropertyChanged(); } }
        public string Performers { get => performeres; set { performeres = value; OnPropertyChanged(); } }
        public decimal Price { get => price; set { price = value; OnPropertyChanged(); } }
        public int CountInStock { get => countInStock; set { countInStock = value; OnPropertyChanged(); } }
        public string TreckList { get => treckList; set { treckList = value; OnPropertyChanged(); } }
        public byte[] Image { get => image; set { image = value; OnPropertyChanged(); } }
        public MusicRecordType SelectedType { get { return selectedType; } set { selectedType = value; OnPropertyChanged(); } }
        private void LoadGenres()
        {
            using (var db = new MusicStoreContext())
            {
                Genres = db.Genre.OrderBy(p => p.Title).ToList();
            }
        }

        private void LoadRecordGenres()
        {
            if (MusicRecord.Genre.Count > 0)
            {
                foreach (var item in MusicRecord.Genre)
                {
                    var genre = Genres.FirstOrDefault(p => p.Id == item.Id);
                    if (genre != null)
                    {
                        genre.IsSelected = true;
                    }
                }
            }
        }

        private void LoadTypes()
        {
            Types = _context.MusicRecordType.ToList();
            SelectedType = Types.FirstOrDefault();
        }

        private async void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (Validate())
                {
                    MusicRecord.Title = MusicRecordTitle;
                    MusicRecord.CountInStock = CountInStock;
                    MusicRecord.Label = Label;
                    MusicRecord.Image = Image;
                    MusicRecord.TypeId = SelectedType.Id;
                    MusicRecord.TreckList = TreckList;
                    MusicRecord.ReleaseDate = ReleaseDate;
                    MusicRecord.Price = Price;
                    MusicRecord.Performers = Performers;
                    var selectedGenres = Genres.Where(p => p.IsSelected).ToList();
                    MusicRecord.Genre.Clear();
                    foreach (var item in selectedGenres)
                    {
                        MusicRecord.Genre.Add(_context.Genre.FirstOrDefault(p => p.Id == item.Id));
                        // _context.Genre.Attach(item);
                    }

                    if (isEdit)
                    {
                        var musicRecord = _context.MusicRecord.FirstOrDefault(p => p.Id == MusicRecord.Id);
                        if (musicRecord != null)
                        {
                            //_context.MusicRecord.Attach(MusicRecord);
                            _context.Entry(musicRecord).CurrentValues.SetValues(MusicRecord);
                        }
                    }
                    else
                    {
                        TextRange textRange = new TextRange(treckListDocument.Document.ContentStart, treckListDocument.Document.ContentEnd);
                        var musicRecord = new MusicRecord()
                        {
                            Title = MusicRecord.Title,
                            CountInStock = MusicRecord.CountInStock,
                            Image = MusicRecord.Image,
                            Label = MusicRecord.Label,
                            TypeId = MusicRecord.TypeId,
                            TreckList = textRange.Text,
                            ReleaseDate = MusicRecord.ReleaseDate,
                            Price = MusicRecord.Price,
                            Performers = MusicRecord.Performers,
                            Genre = MusicRecord.Genre
                        };
                        _context.MusicRecord.Add(musicRecord);
                    }
                    _context.SaveChanges();
                    DialogResult = true;
                }
                else
                {
                    await this.ShowMessageAsync("Ошибка проверки данных!", "Не все данные подходят. Проверьте, все ли поля заполнены верно: дата не позднее текщей даты, цена и количество положительные, выбран хотя бы 1 жанр и все поля не пустые.", MessageDialogStyle.Affirmative, new MetroDialogSettings { ColorScheme = MetroDialogColorScheme.Inverted });
                    return;
                }
            }
            catch (Exception ex)
            {

            }
        }

        private bool Validate()
        {
            if (!string.IsNullOrEmpty(MusicRecordTitle) &&
                !string.IsNullOrEmpty(Label) &&
                !string.IsNullOrEmpty(Performers) &&
                ReleaseDate <= DateTime.Now &&
                Price >= 1 &&
                Genres.Any(p => p.IsSelected) &&
                Image != null &&
                CountInStock >= 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void LoadImage_Click(object sender, RoutedEventArgs e)
        {
            var openFilDialog = new OpenFileDialog();
            openFilDialog.Filter = "Картинки | *.jpg; *.jpeg; *.png.;";
            if (openFilDialog.ShowDialog() == true)
            {
                Image = File.ReadAllBytes(openFilDialog.FileName);
            }
        }

        private void ListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            (sender as ListBox).SelectedItem = null;
        }
    }
}
