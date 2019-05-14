using Newtonsoft.Json;

namespace SongPlayer
{
    enum Words
    {
        Name,
        Number,
        Language,
        Category,
        NewCategory, 
        Rewritten,
        Delete,
        Cancel,
        File,
        Add,
        Save,
        Info,
        Settings,
        Play,
        Edit,
        Player,
        Tip,
        All,
        ConvertingEnded,
        NameIsBusy,
        CategoryNotSpecified,
        NameNotSpecified,
        NumberNotSpecified,
        MP3FileNotSpecified
    }
    static class Settings
    {
        public static string Path;
        public static string Lang;
        public static bool Console;
        public static bool Tips; 
        public static void Update()
        {
            dynamic json = JsonConvert.DeserializeObject(System.IO.File.ReadAllText(Consts.SettingsPath));
            Path = json.PlayerPath;
            Lang = json.Lang;
            Tips = json.Tips;
            Console= json.SQLiteConsole;
        }
        public static string GetWord(Words word)
        {
            switch (Lang)
            {
                case "ua":
                    switch (word)
                    {
                        case Words.Name:
                            return "Назва";
                        case Words.Number:
                            return "Номер";
                        case Words.Language:
                            return "Мова";
                        case Words.Category:
                            return "Категорія";
                        case Words.NewCategory:
                            return "Нова Категорія";
                        case Words.Rewritten:
                            return "Переписаний";
                        case Words.Delete:
                            return "Видалити";
                        case Words.Cancel:
                            return "Відміна";
                        case Words.File:
                            return "Файл";
                        case Words.Add:
                            return "Додати";
                        case Words.Save:
                            return "Зберегти";
                        case Words.Info:
                            return "Інформація";
                        case Words.Settings:
                            return "Налаштування";
                        case Words.Play:
                            return "Відтворити";
                        case Words.Edit:
                            return "Редагувати";
                        case Words.Player:
                            return "Плеєр";
                        case Words.Tip:
                            return "Підказки";
                        case Words.NameIsBusy:
                            return "Така пісня вже існує";
                        case Words.ConvertingEnded:
                            return "Конвертація завершена";
                        case Words.CategoryNotSpecified:
                            return "Категорія не вказана";
                        case Words.NameNotSpecified:
                            return "Ім'я не вказано";
                        case Words.NumberNotSpecified:
                            return "Номер не вказано";
                        case Words.MP3FileNotSpecified:
                            return "mp3 файл не вказано";
                        case Words.All:
                            return "Все";
                    }
                    break;

                case "ru":
                    switch (word)
                    {
                        case Words.Name:
                            return "Название";
                        case Words.Number:
                            return "Номер";
                        case Words.Language:
                            return "Язык";
                        case Words.Category:
                            return "Категория";
                        case Words.NewCategory:
                            return "Новая категория";
                        case Words.Rewritten:
                            return "Переписан";
                        case Words.Delete:
                            return "Удалить";
                        case Words.Cancel:
                            return "Отмена";
                        case Words.File:
                            return "Файл";
                        case Words.Add:
                            return "Добавить";
                        case Words.Save:
                            return "Сохранить";
                        case Words.Info:
                            return "Информация";
                        case Words.Settings:
                            return "Настройки";
                        case Words.Play:
                            return "Воспроизвести";
                        case Words.Edit:
                            return "Редактировать";
                        case Words.Player:
                            return "Плеер";
                        case Words.Tip:
                            return "Подсказки";
                        case Words.NameIsBusy:
                            return "Такая песня уже существует";
                        case Words.ConvertingEnded:
                            return "Конвертация закончена";
                        case Words.CategoryNotSpecified:
                            return "Категория не указана";
                        case Words.NameNotSpecified:
                            return "Имя не указано";
                        case Words.NumberNotSpecified:
                            return "Номер не указан";
                        case Words.MP3FileNotSpecified:
                            return "mp3 файл не указан";
                        case Words.All:
                            return "Все";
                    }
                    break;

                case "en":
                    switch (word)
                    {
                        case Words.Name:
                            return "Name";
                        case Words.Number:
                            return "Number";
                        case Words.Language:
                            return "Language";
                        case Words.Category:
                            return "Category";
                        case Words.NewCategory:
                            return "New сategory";
                        case Words.Rewritten:
                            return "Rewritten";
                        case Words.Delete:
                            return "Delete";
                        case Words.Cancel:
                            return "Cancel";
                        case Words.File:
                            return "File";
                        case Words.Add:
                            return "Add";
                        case Words.Save:
                            return "Save";
                        case Words.Info:
                            return "Info";
                        case Words.Settings:
                            return "Settings";
                        case Words.Play:
                            return "Play";
                        case Words.Edit:
                            return "Edit";
                        case Words.Player:
                            return "Player";
                        case Words.Tip:
                            return "Tips";
                        case Words.NameIsBusy:
                            return "Name is busy";
                        case Words.ConvertingEnded:
                            return "Converting ended";
                        case Words.CategoryNotSpecified:
                            return "Category not Specified";
                        case Words.NameNotSpecified:
                            return "Name not specified";
                        case Words.NumberNotSpecified:
                            return "Number not specified";
                        case Words.MP3FileNotSpecified:
                            return "mp3 file not specified";
                        case Words.All:
                            return "All";
                    }
                    break;
            }
            return "NaN";
        }
    }
}
