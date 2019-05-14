using System.Windows;

namespace SongPlayer
{
    /// <summary>
    /// Логика взаимодействия для DeleteDialog.xaml
    /// </summary>
    public partial class DeleteDialog : Window
    {
        private bool delete = false;
        public DeleteDialog(string SongName)
        {
            InitializeComponent();
            Label.Text = "Видалити " + SongName + " ?";
            UpdateNames();
        }
        private void Delete(object sender, RoutedEventArgs e)
        {
            delete = !delete;
            Close();
        }
        private void Cancel(object sender, RoutedEventArgs e)
        {
            Close();
        }
        public bool GetDelete()
        {
            return delete;
        }
        void UpdateNames()
        {
            DeleteB.Content = Settings.GetWord(Words.Delete);
            CancelB.Content = Settings.GetWord(Words.Cancel);
        }
    }
}
