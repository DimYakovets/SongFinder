using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;
using Microsoft.VisualBasic;
using Microsoft.Win32;

namespace SongPlayer
{
    /// <summary>
    /// Логика взаимодействия для AddSongDialog.xaml
    /// </summary>
    public partial class AddSongDialog : Window
    {
        string name;
        string category;
        string path;
        short number = -1;
        string lang;
        byte[] MP3;
        bool ok = false;
        public AddSongDialog()
        {
            InitializeComponent();
            SongLang.ItemsSource = new[] { "Російська", "Українська" };
            SongLang.SelectedItem = "Українська";
            var Categories = new List<string>();
            foreach (var item in MainWindow.Songs)
            {
                if (!Categories.Exists((c) => c == item.Category))
                    Categories.Add(item.Category);
            }
            SongCategoryList.ItemsSource = Categories;
        }
        private void AddSong_Click(object sender, RoutedEventArgs args)
        {
            string error_text = "";
            name = SongName.Text;

            if((SongNumber.Text != "" && SongNumber.Text != null && SongNumber.Text.Length < 5 ) || IsHasNumber.IsChecked.Value)
            {
                if (!IsHasNumber.IsChecked.Value)
                    number = Convert.ToInt16(SongNumber.Text);
                else
                    number = 0;
            }

            if ((SongCategory.Text != null && SongCategory.Text != "" ) || (string)SongCategoryList.SelectedItem != "")
            {
                if ((bool)NewCategory.IsChecked)
                {
                    category = SongCategory.Text;
                }
                else
                {
                    category = (string)SongCategoryList.SelectedItem;
                }
            }
            if(path != null)
            {
                if (path.EndsWith(".mp3"))
                {
                    MP3 = System.IO.File.ReadAllBytes(path);
                }
                else if (path.EndsWith(".mp4"))
                {
                    try
                    {
                        string lpath = Environment.CurrentDirectory + "\\file.mp3";

                        Interaction.Shell($"ffmpeg.exe -i \"{path}\" \"{lpath}\"", AppWinStyle.Hide, true, -1);
                        MP3 = System.IO.File.ReadAllBytes(lpath);
                        System.IO.File.Delete(lpath);
                        MessageBox.Show("Конвертація завершена");
                    }
                    catch(Exception e)
                    {
                        Log.Error(e.StackTrace + " => " + e.Message);
                    }
                }
                else if (path.EndsWith(".m4a"))
                {
                    try
                    {
                        string temp = Environment.CurrentDirectory + @"\Data\Temp\file.wav";
                        string mp3 = Environment.CurrentDirectory + @"\Data\Temp\file.mp3";
                        Interaction.Shell($"faad.exe -o \"{temp}\" \"{path}\"", AppWinStyle.Hide, true, -1);
                        Interaction.Shell($"lame.exe --preset standard \"{temp}\" \"{mp3}\"", AppWinStyle.Hide, true, -1);
                        MP3 = System.IO.File.ReadAllBytes(mp3);
                        System.IO.File.Delete(temp);
                        System.IO.File.Delete(mp3);
                        MessageBox.Show("Конвертація завершена");
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.StackTrace + " => " + e.Message);
                    }
                }
            }
            lang = (string)SongLang.SelectedValue;
            if(name != "" && name != null && !DataBaseHelper.NameExist(name) && category != "" && category != null && ((SongNumber.Text != "" && SongNumber.Text != null) || IsHasNumber.IsChecked.Value) && lang != "" && MP3 != null)
            {
                DataBaseHelper.Add(name, lang, number, category, MP3);
                ok = !ok;
                Close();
            }
            else
            {
                if(name == "")
                {
                    error_text += "Назву не вказано" + "\n";
                }
                if (DataBaseHelper.NameExist(name) && name != "")
                {
                    error_text += "Назва використана" + "\n";
                }
                if (category == null || category == "")
                {
                    error_text += "Категорію не вказано" + "\n";
                }
                if ((number == 0 || number < 0) && !IsHasNumber.IsChecked.Value)
                {
                    error_text += "Номер не вказано" + "\n";
                }
                if (path == null || path == "")
                {
                    error_text += "Файл не вказано" + "\n";
                }
                MessageBox.Show(error_text, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public bool DialogStatus()
        {
            return ok;
        }
        private void OBZOR_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Music files(*.mp3;*.m4a;*.mp4)|*.mp3;*.m4a;*.mp4"
            };
            fileDialog.ShowDialog();
            if(fileDialog.FileName != "")
            {
                path = fileDialog.FileName;
            }
            FileName.Text = fileDialog.FileName;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
