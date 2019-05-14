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
        List<string> files = new List<string>();
        short fileIndex;
        string PlayerPath = "wmplayer.exe";
        bool pChange = false;
        Song selcted = null;


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
            string name = selcted != null ? selcted.Name : "";

            byte[] arr = null;
            if (name != "")
            {
                var priority = Songs.Find(e => e.Name == name).Popularity;
                Songs.Find(e => e.Name == name).PopularityUp();
                DataBaseHelper.Edit(name, "Popularity", priority + 1);
                arr = DataBaseHelper.GetMp3(name);
                fileIndex++;
                if (arr != null)
                {
                    try
                    {
                        var file = Environment.CurrentDirectory + "\\Data\\Temp\\.file" + fileIndex.ToString() + ".mp3";
                        files.Add(file);
                        System.IO.File.WriteAllBytes(file, arr);

                        //var process = new ProcessStartInfo(PlayerPath, "\"" + file + "\"");
                        //Process.Start(process);
                        state = State.NaN;
                        Play(Environment.CurrentDirectory + @"\1.mp3");

                    }
                    catch (Exception e)
                    {
                        Log.Error(e.StackTrace + " => " + e.Message);
                    }
                }
            }
            GC.Collect();
        }
        #region Edit/Add/Delete
        private void Edit(object sender, RoutedEventArgs e)
        {
            var name = SongName.Text.Replace($"{SongPlayer.Settings.GetWord(Words.Name)} : ", "");
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
            var name = SongName.Text.Replace($"{SongPlayer.Settings.GetWord(Words.Name)} : ", "");
            if (name != "" && name != null)
            {
                DeleteDialog delete = new DeleteDialog(name);
                delete.ShowDialog();
                if (delete.GetDelete())
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
        private void SettingsClick(object sender, RoutedEventArgs e)
        {
           new SettingsDialog().ShowDialog();
           SongPlayer.Settings.Update();
           UpdateNames();
           PlayerPath = SongPlayer.Settings.Path;
        }
        private void SongClick(object sender,RoutedEventArgs e)
        {
            Song item = (Song)(sender as Button).DataContext;
            if (item != null)
            {
                var num = item.Number.ToString();
                num = num == "0" ? $"{SongPlayer.Settings.GetWord(Words.Rewritten)}" : num;
                SongName.Text = $"{SongPlayer.Settings.GetWord(Words.Name)} : " + item.Name;
                SongLang.Text = $"{SongPlayer.Settings.GetWord(Words.Language)} : " + item.Lang;
                SongNumber.Text = $"{SongPlayer.Settings.GetWord(Words.Number)} : " + num;
                SongCategory.Text = $"{SongPlayer.Settings.GetWord(Words.Category)} : " + item.Category;
                selcted = item;
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
                        Content.ItemsSource = newContent;
                    }
                }
                else
                {
                    var newContent = Songs.FindAll((f) => f.Name.ToUpper().Contains((sender as TextBox).Text.ToUpper()));
                    Content.ItemsSource = newContent;
                }
            }
            else
            {
                Content.ItemsSource = Songs;
            }
            GC.Collect();
        }
        private void Category_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if ((sender as ComboBox).SelectedItem != null && pChange && (string)(sender as ComboBox).SelectedItem != "Популярні")
            {
                Search.Text = "";
                var newContent = Songs.FindAll((f) => f.Category.ToUpper().Contains(((sender as ComboBox).SelectedValue as string).Replace("Все", "").ToUpper()));
                Content.ItemsSource = newContent;
                GC.Collect();
            }
            else if((sender as ComboBox).SelectedItem != null && (string)(sender as ComboBox).SelectedItem == "Популярні") 
            {
                Content.ItemsSource = Songs.OrderByDescending(f => f.Popularity);
                GC.Collect();
            }
            else
            {
                var newContent = Songs.FindAll((f) => f.Category.ToUpper().Contains(((sender as ComboBox).SelectedValue as string).Replace("Все", "").ToUpper()));
                Content.ItemsSource = newContent;
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

        MediaPlayer player = new MediaPlayer();
        MediaTimeline timeLine = new MediaTimeline();
        State state = State.NaN;

        private void Play(string file)
        {
            if (state == State.NaN || state == State.Stop)
            {
                player.Open(new Uri(file));
                player.Play();
                state = State.Play;
                PausePlay.Content = "Pause";
            }
        }


        private void Stop (object sender, RoutedEventArgs e)
        {
            if (state == State.Play || state == State.Pause)
            {
                player.Stop();
                state = State.Stop;
                PausePlay.Content = "Pause";
            }
        }

        private void Pause (object sender, RoutedEventArgs e)
        {
            if (state == State.Play)
            {
                player.Pause();
                state = State.Pause;
                PausePlay.Content = "Resume";
                return;
            }
            if (state == State.Pause)
            {
                player.Play();
                state = State.Play;
                PausePlay.Content = "Pause";
            }
        }
        private void ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
        }
        private void TimeChange(object sender, System.Windows.Controls.Primitives.DragCompletedEventArgs e)
        {

        }
        private void TimeChangeBegin(object sender, System.Windows.Controls.Primitives.DragStartedEventArgs e)
        {

        }
        #endregion
        #region Window events
        private void ButtonContextClick(object sender, RoutedEventArgs e)
        {
            var s = ((sender as MenuItem).Parent as ContextMenu).Parent;
            //SongClick(sender, e);
            //Run(sender, e);
        }
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            #region Check direcrories
            System.IO.DirectoryInfo DataTemp = new System.IO.DirectoryInfo(Environment.CurrentDirectory + "\\Data\\Temp");
            if (!DataTemp.Exists)
            {
                DataTemp.Create();
                Log.Message("Folder Data\\Temp crated.");
            }
            System.IO.DirectoryInfo DataSettings = new System.IO.DirectoryInfo(Environment.CurrentDirectory + "\\Data\\Settings");
            if (!DataSettings.Exists)
            {
                DataSettings.Create();
                Log.Message("Folder Data\\Settings crated.");
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
            if(!System.IO.File.Exists(Consts.SettingsPath))
                System.IO.File.WriteAllText(Consts.SettingsPath,"{\"PlayerPath\":\"wmplayer.exe\",\"Lang\":\"ua\",\"Tips\":false,\"SQLiteConsole\":false}");
            UpdateSongs();
            #endregion
            if (System.IO.File.Exists(Consts.SettingsPath))
            {
                SongPlayer.Settings.Update();
                PlayerPath = SongPlayer.Settings.Path;
            }
            UpdateNames();
            
            Log.Message("Window Loaded.");
            GC.Collect();
        }
        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs args )
        {
            System.IO.DirectoryInfo info = new System.IO.DirectoryInfo(Environment.CurrentDirectory + "\\Data\\Temp");
            var files = info.GetFiles();
            var pName = Cut(PlayerPath).Replace(".exe", "");
            foreach (var item in Process.GetProcessesByName(pName))
            {
                item.Kill();
                Log.Message($"Process {item.ProcessName} killed.");
            }

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
        private void Button_MouseDoubleClick(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            Run(sender, new RoutedEventArgs());
        }
        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if ((Keyboard.Modifiers == ModifierKeys.Control) && (e.Key == Key.Q) && SongPlayer.Settings.Console)
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
            Content.ItemsSource = Songs;
            var Categories = new List<string>{"Все","Популярні"};
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
        private void UpdateNames()
        {
            SongName.Text = $"{SongPlayer.Settings.GetWord(Words.Name)} : ";
            SongLang.Text = $"{SongPlayer.Settings.GetWord(Words.Language)} : ";
            SongNumber.Text = $"{SongPlayer.Settings.GetWord(Words.Number)} : ";
            SongCategory.Text = $"{SongPlayer.Settings.GetWord(Words.Category)} : ";
            SongPlay.Content = SongPlayer.Settings.GetWord(Words.Play);
            SongEdit.Content = SongPlayer.Settings.GetWord(Words.Edit);
            SongAdd.Content = SongPlayer.Settings.GetWord(Words.Add);
            SongDelete.Content = SongPlayer.Settings.GetWord(Words.Delete);
            Settings.Content = SongPlayer.Settings.GetWord(Words.Settings);
            Info.Text = SongPlayer.Settings.GetWord(Words.Info);
        }
        private void ResetInfo()
        {
            SongName.Text = "Назва :";
            SongLang.Text = "Мова :";
            SongNumber.Text = "Номер :";
            SongCategory.Text = "Категорія :";
        }
        private string Cut(string str)
        {
            int j = str.Length - 1;
            int index = str.Length - 1;
            for (int i = 0; i < str.Length; i++,j--)
            {
                if (str[j] == '\\')
                {
                    index = i;
                    break;
                }
            }
            var result = str.Remove(0, str.Length - index - 1).Replace("\\","");
            return result;
        }
        #endregion
    }
}
