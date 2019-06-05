using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Threading;


namespace SongPlayer
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static List<Song> Songs { get; private set; }
        public DispatcherTimer Timer { get => timer; set => timer = value; }

        readonly List<string> files = new List<string>();
        short fileIndex;
        bool pChange = false;
        Song selected = null;

        public MainWindow()
        {
            Log.Init();
            try
            {
                InitializeComponent();
            }
            catch(Exception e)
            {
                Log.Error(e.StackTrace + " => " + e.Message);
            }
            
        }
        private void Run(object sender, RoutedEventArgs args)
        {
            string name = selected != null ? selected.Name : "";
            if (name != "")
            {
                try
                {
                    byte[] arr = null;
                    arr = DataBaseHelper.GetMp3(name);
                    if (arr != null)
                    {
                        fileIndex++;
                        var file = Environment.CurrentDirectory + "\\Data\\Temp\\.file" + fileIndex.ToString() + ".mp3";
                        files.Add(file);
                        System.IO.File.WriteAllBytes(file, arr);
                        state = State.NaN;
                        Play(file);
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.StackTrace + " => " + e.Message);
                }
            }
            GC.Collect();
        }
        #region Edit/Add/Delete
        private void Edit(object sender, RoutedEventArgs e)
        {
            var name = selected.Name;
            if (name != "" && name != null)
            {
                EditDialog edit = new EditDialog(name);
                edit.ShowDialog();
                if (edit.DialogStatus())
                {
                    ResetInfo();
                    Search.Text = "";
                    UpdateSongs();
                }
            }
            GC.Collect();
        }
        private void Delete(object sender, RoutedEventArgs e)
        {
            var name = selected.Name;
            if (name != "" && name != null)
            {
                if (MessageBox.Show($"Видалити {name} ?", "Видалити", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    DataBaseHelper.Delete(name);
                    ResetInfo();
                    Search.Text = "";
                    UpdateSongs();
                }
                GC.Collect();
            }
        }
        private void AddNew(object sender, RoutedEventArgs e)
        {
            AddSongDialog add = new AddSongDialog();
            add.ShowDialog();
            if (add.DialogStatus())
            {
                ResetInfo();
                Search.Text = "";
                UpdateSongs();
            }
            GC.Collect();
        }
        #endregion
        #region Song actions
        private void SongClick(object sender,RoutedEventArgs e)
        {
            var item = Songs.Find((p) => p.Name == ((sender as Button).Content as Label).Content as string);
            if (item != null)
            {
                var num = item.Number.ToString();
                num = num == "0" ? $"Переписаний" : num;
                SongName.Text = $"Назва : " + item.Name;
                SongLang.Text = $"Мова : " + item.Lang;
                SongNumber.Text = $"Номер : " + num;
                SongCategory.Text = $"Категорія : " + item.Category;
                selected = item;
            }
            GC.Collect();
        }
        private void Search_TextChanged(object sender, TextChangedEventArgs e)
        {
            pChange = false;
            Category.SelectedItem = "Все";
            pChange = true;
            if (Search.Text != "" && Search.Text != null)
            {
                if (Search.Text.IsDigitsOnly() && Search.Text.Length <= 4)
                {
                    if (Convert.ToInt32((Search.Text)) < 2500)
                    {
                        var newContent = Songs.FindAll((f) => f.Number == Convert.ToInt32((Search.Text)));
                        UpdateSomgsLis(newContent.ToArray());
                    }
                }
                else
                {
                    var newContent = Songs.FindAll((f) => f.Name.ToUpper().Contains((sender as TextBox).Text.ToUpper()));
                    UpdateSomgsLis(newContent.ToArray());
                }
            }
            else
            {
                UpdateSomgsLis(Songs.ToArray());
            }
            GC.Collect();
        }
        private void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null && pChange && (string)(sender as ComboBox).SelectedItem != "Популярні")
            {
                Search.Text = "";
                var newContent = Songs.FindAll((f) => f.Category.ToUpper().Contains(((sender as ComboBox).SelectedValue as string).Replace("Все", "").ToUpper()));
                UpdateSomgsLis(newContent.ToArray());
                GC.Collect();
            }
            else
            {
                var newContent = Songs.FindAll((f) => f.Category.ToUpper().Contains(((sender as ComboBox).SelectedValue as string).Replace("Все", "").ToUpper()));
                UpdateSomgsLis(newContent.ToArray());
                GC.Collect();
            }
        }
        #endregion
        #region Player
        enum State
        {
            Play,
            Pause,
            Stop,
            NaN
        }

        readonly MediaPlayer player = new MediaPlayer();
        DispatcherTimer timer = new DispatcherTimer();
        State state = State.NaN;
        bool PChange = false;
        private void Play(string file)
        {
            if (state == State.NaN || state == State.Stop)
            {
                TagLib.File f = TagLib.File.Create(file, TagLib.ReadStyle.Average);
                var time = Math.Floor(f.Properties.Duration.TotalSeconds);
                TimeTotal.Text = f.Properties.Duration.ToString(@"mm\:ss");
                TimeSlider.Maximum = time;
                TimeSlider.Value = 0;
                player.Open(new Uri(file));
                player.MediaEnded += (o, e) => 
                {
                    Stop(null, null);
                };
                Timer.Interval = TimeSpan.FromSeconds(1);
                Timer.Tick += (o,e) => 
                {
                    TimeNow.Text = player.Position.ToString(@"mm\:ss");
                    var now = Math.Floor(player.Position.TotalSeconds);
                    PChange = true;
                    TimeSlider.Value = now;
                    PChange = false;
                };

                Timer.Start();
                player.Play();

                state = State.Play;
                PausePlay.Content = "Пауза";
                TimeNow.Text = "00:00";
            }
        }
        private void Stop (object sender, RoutedEventArgs e)
        {
            if (state == State.Play || state == State.Pause)
            {
                player.Stop();
                state = State.Stop;
                PausePlay.Content = "Паузa";
                TimeNow.Text = "00:00";
                TimeTotal.Text = "00:00";
                TimeSlider.Value = 0;
                TimeSlider.Maximum = 0;
                Timer.Stop();
                player.Close();
            }
        }
        private void Pause (object sender, RoutedEventArgs e)
        {
            if (state == State.Play)
            {
                player.Pause();
                state = State.Pause;
                PausePlay.Content = "Продовжити";
                return;
            }
            if (state == State.Pause)
            {
                player.Play();
                state = State.Play;
                PausePlay.Content = "Пауза";
            }
        }
        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!PChange)
            {
                player.Position = new TimeSpan(0,0,(int)e.NewValue);
                TimeNow.Text = player.Position.ToString(@"mm\:ss");
            }
        }
        private void VolumeChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            player.Volume = e.NewValue / 100;
            VolumeText.Text = $"Гучність : {(int)e.NewValue}";
        }
        #endregion
        #region Window events
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Check direcrories
            System.IO.DirectoryInfo DataTemp = new System.IO.DirectoryInfo(Environment.CurrentDirectory + "\\Data\\Temp");
            if (!DataTemp.Exists)
            {
                DataTemp.Create();
                Log.Message("Folder Data\\Temp crated.");
            }
            System.IO.DirectoryInfo DataDataBase = new System.IO.DirectoryInfo(Environment.CurrentDirectory + "\\Data\\DataBase");
            if (!DataDataBase.Exists || !System.IO.File.Exists(Consts.DBPath))
            {
                DataDataBase.Create();
                Log.Message("Folder Data\\DataBase crated.");
                System.Data.SQLite.SQLiteConnection db = new System.Data.SQLite.SQLiteConnection($"Data source={Consts.DBPath}; Version=3");
                db.Open();
                var cmd = new System.Data.SQLite.SQLiteCommand("CREATE TABLE Songs (Name TEXT NOT NULL UNIQUE, Lang TEXT NOT NULL, Number INTEGER (0, 2500) NOT NULL, Category TEXT NOT NULL, MP3 BLOB NOT NULL, Popularity INTEGER NOT NULL DEFAULT(0));", db);
                cmd.ExecuteNonQuery();
                Log.Message("File Data\\Temp\\Songs.db crated.");
                db.Close();
            }
            UpdateSongs();
            #endregion
            
            Log.Message("Window Loaded.");
            GC.Collect();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs args )
        {
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(Environment.CurrentDirectory + "\\Data\\Temp");
            var files = info.GetFiles();

            foreach (var item in files)
            {
                try
                {
                    if (item.Exists)
                    {
                        item.Delete();
                        Log.Message($"File {item.Name} deleted.");
                    }
                }
                catch (Exception e)
                {
                    Log.Error(e.StackTrace + " => " + e.Message);
                }
            }
            System.IO.File.Delete("ffmpeg.exe");
            Log.Message("Window closed.");
        }
        private void Button_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            Run(sender, new RoutedEventArgs());
        }
        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.Q))
            {
                if (System.IO.File.Exists(@"Data\DataBase\sqlite3.exe"))
                {
                    Process.Start(Environment.CurrentDirectory + @"\Data\DataBase\sqlite3.exe", "\"" + Environment.CurrentDirectory + @"\Data\DataBase\Songs.db" + "\"");
                }
            }
        }
        #endregion
        #region Help methods
        void UpdateSongs()
        {
            Songs = DataBaseHelper.Get();
            Songs.Sort((x, y) => String.CompareOrdinal(x.Name, y.Name));
            UpdateSomgsLis(Songs.ToArray());
            var Categories = new List<string>{"Все"};
            foreach (var item in Songs)
            {
                if (!Categories.Exists((c) => c == item.Category))
                    Categories.Add(item.Category);
            }
            Categories.Sort();
            Category.ItemsSource = Categories;
            Category.SelectedItem = "Все";
            GC.Collect();
        }
        private void ResetInfo()
        {
            SongName.Text = "Назва :";
            SongLang.Text = "Мова :";
            SongNumber.Text = "Номер :";
            SongCategory.Text = "Категорія :";
        }
        private void UpdateSomgsLis(Song[] songs)
        {
            Content.Children.Clear();
            foreach (var item in songs)
            {
                var button = new Button
                {
                    Content = new Label()
                    {
                        Content = item.Name
                    },
                    Width = 290,
                    Margin = new Thickness(0,2,0,3),
                    Background = new SolidColorBrush(Color.FromArgb(255, 239, 239, 239))
                };
                button.Click += SongClick;
                button.MouseDoubleClick += Button_MouseDoubleClick;
                Content.Children.Add(button);
            }
        }
        #endregion
    }
}
