using System;
using System.Windows;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.Win32;

namespace SongPlayer
{
    /// <summary>
    /// Логика взаимодействия для SettingsDialog.xaml
    /// </summary>
    public partial class SettingsDialog : Window
    {
        string path;
        public SettingsDialog()
        {
            InitializeComponent();
            UpdateNames();
        }

        private void Save(object sender, RoutedEventArgs e)
        {
            JObject root = new JObject
            {
                ["PlayerPath"] = path,
                ["Lang"] = Language.SelectedItem as string,
                ["Tips"] = Tips.IsChecked,
                ["SQLiteConsole"] = console.IsChecked
            };
            System.IO.File.WriteAllText(Consts.SettingsPath, root.ToString());
            Close();
        }

        private void Obzor(object sender, RoutedEventArgs e)
        {
            OpenFileDialog file = new OpenFileDialog()
            {
                Filter = "Exe files (*.exe)|*.exe",
                Title = "Укажіть плеєр",
            };
            file.ShowDialog();
            if(file.FileName != "")
            {
                path = file.FileName;
                PPath.Text = file.FileName;
            }

        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
        }
        private void Update()
        {
            Language.ItemsSource = new[] { "ua", "ru", "en" };
            dynamic json = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(Consts.SettingsPath));
            PPath.Text = json.PlayerPath;
            path = json.PlayerPath;
            Language.SelectedItem = GetIndex(new[] { "ua", "ru", "en" },(string)json.Lang);
            Tips.IsChecked = json.Tips;
            console.IsChecked = json.SQLiteConsole;
        }
        string GetIndex(string[] arr,string item)
        {
            for (int i = 0; i < arr.Length; i++)
            {
                if(arr[i] == item)
                {
                    return arr[i];
                }
            }
            return "";
        }
        void UpdateNames()
        {
            PlayerPath.Text = Settings.GetWord(Words.Player);
            Tip.Text = Settings.GetWord(Words.Tip);
            Lang.Text = Settings.GetWord(Words.Language);
            SaveB.Content = Settings.GetWord(Words.Save);
        }
    }
}
