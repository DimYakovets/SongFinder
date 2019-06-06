using Microsoft.VisualBasic;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Input;

namespace SongPlayer
{
    /// <summary>
    /// Логика взаимодействия для EditDialog.xaml
    /// </summary>
    public partial class EditDialog : Window
    {
        string name = "";
        string category;
        string path;
        short number = -1;
        string lang;
        byte[] MP3;
        #region buffer
        string B_name;
        string B_category;
        short B_number;
        string B_lang;
        bool ok = false;
        #endregion
        public EditDialog(string songname)
        {
            InitializeComponent();

            SongLang.ItemsSource = new[] { "Російська", "Українська" };
            var Categories = new List<string>();
            foreach (var item in MainWindow.Songs)
            {
                if (!Categories.Exists((c) => c == item.Category))
                    Categories.Add(item.Category);
            }
            SongCategoryList.ItemsSource = Categories;

            SongBName.Text = songname;
            SongBCategory.Text = MainWindow.Songs.Find((e) => e.Name == songname).Category;
            SongBLang.Text = MainWindow.Songs.Find((e) => e.Name == songname).Lang;
            SongBNumber.Text = MainWindow.Songs.Find((e) => e.Name == songname).Number.ToString() == "0" ? "Переписаний" : $"{MainWindow.Songs.Find((e) => e.Name == songname).Number}";

            B_name = songname;
            B_lang = MainWindow.Songs.Find((e) => e.Name == songname).Lang;
            B_category = MainWindow.Songs.Find((e) => e.Name == songname).Category;
            B_number = MainWindow.Songs.Find((e) => e.Name == songname).Number;
        }
        private void EditSong_Click(object sender, RoutedEventArgs args)
        {
            if (SongName.Text != "" && SongName.Text != null && !MainWindow.Songs.Exists((f) => f.Name == SongName.Text) )
            {
                name = SongName.Text;
            }
            if(MainWindow.Songs.Exists((f) => f.Name == SongName.Text))
            {
                MessageBox.Show("Назва використана", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if ((SongNumber.Text != "" && SongNumber.Text != null) || IsHasNumber.IsChecked.Value)
            {
                if (!IsHasNumber.IsChecked.Value)
                    number = Convert.ToInt16(SongNumber.Text);
                else
                    number = 0;
            }
            if ((SongCategory.Text != "" || SongCategory.Text != null) || (string)SongCategoryList.SelectedItem != "")
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
            if (path != null)
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
                    catch (Exception e)
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

            if(name != null && name != "" && !DataBaseHelper.NameExist(name))
            {
                DataBaseHelper.Edit(B_name, "Name", name);
                ok = true;
            }
            if(number != -1)
            {
                DataBaseHelper.Edit(B_name, "Number", number);
                ok = true;
            }
            if (category != null && category != "")
            {
                DataBaseHelper.Edit(B_name, "Category", category);
                ok = true;
            }
            if(lang != null && lang != B_lang)
            {
                DataBaseHelper.Edit(B_name, "Lang", lang);
                ok = true;
            }
            if(MP3 != null)
            {
                DataBaseHelper.Edit(B_name, "MP3", MP3);
                ok = true;
            }
            Close();
        }
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
        private void OBZOR_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog fileDialog = new OpenFileDialog
            {
                Filter = "Music files(*.mp3;*.m4a;*.mp4)|*.mp3;*.m4a;*.mp4"
            };
            fileDialog.ShowDialog();
            if (fileDialog.FileName != "")
            {
                path = fileDialog.FileName;
            }
            FileName.Text = fileDialog.FileName;
        }

        public bool DialogStatus()
        {
            return ok;
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }
    }
}
