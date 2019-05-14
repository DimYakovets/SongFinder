using System;
using System.Linq;
using System.IO;

namespace SongPlayer
{
    public static class Log
    {
        private static int I = 0;
        public static void Init()
        {
            DirectoryInfo DataLog = new DirectoryInfo(Consts.LogPath);
            if (!DataLog.Exists)
            {
                DataLog.Create();
            }
            DirectoryInfo info = new DirectoryInfo(Consts.LogPath);
            var number = info.GetFiles("*.log").Select((e) => Int32.Parse(e.Name.Replace(".log",""))).ToList();
            number.Sort();
            foreach (var item in number)
            {
                I = item + 1;
            }
            File.WriteAllText(Consts.LogPath + $"\\{I}.log","");
            Message($"File {I}.log created");
        }
        public static void Message(string message)
        {
            string log = File.ReadAllText($"{Consts.LogPath}\\{I}.log");
            log += $"[{DateTime.Now.ToString()}] [Info] {message}\n";
            File.WriteAllText($"{Consts.LogPath}\\{I}.log", log);
        }
        public static void Error(string message)
        {
            string log = File.ReadAllText($"{Consts.LogPath}\\{I}.log");
            log += $"[{DateTime.Now.ToString()}] [Error] {message}\n";
            File.WriteAllText($"{Consts.LogPath}\\{I}.log", log);
        }
    }
}
