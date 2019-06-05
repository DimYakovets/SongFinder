using System;
using System.Windows;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Data.SQLite;
using System.Threading.Tasks;

namespace SongPlayer
{
    static class DataBaseHelper
    {
        public static List<Song> Get()
        {
            List<Song> songs = new List<Song>();
            using (SQLiteConnection DB = new SQLiteConnection($"Data source={Consts.DBPath}; Version=3"))
            {
                DB.Open();
                SQLiteCommand cmd =new SQLiteCommand("Select Name,Lang,Number,Category from Songs", DB);
                SQLiteDataReader SQL = cmd.ExecuteReader();
                using (SQL)
                {
                    if (SQL.HasRows)
                    {
                        while (SQL.Read())
                        {
                            try
                            {
                                string name = (string)SQL["Name"];
                                var number = Convert.ToInt16(SQL["Number"]);
                                string lang = (string)SQL["Lang"];
                                string category = (string)SQL["Category"];
                                songs.Add(new Song(name, number, category, lang));
                                GC.Collect();

                            }
                            catch (Exception e)
                            {
                                Log.Error(e.StackTrace + " => " + e.Message);
                            }
                        }
                    }
                }
                DB.Close();
            }
            Log.Message($"{songs.Count} songs loaded.");
            return songs;
        }
        public static void Delete(string name)
        {
            using (SQLiteConnection DB = new SQLiteConnection($"Data source={Consts.DBPath}; Version=3"))
            {
                DB.Open();
                SQLiteCommand cmd = new SQLiteCommand($"DELETE FROM Songs WHERE Name='{name}'", DB);
                cmd.ExecuteNonQuery();
                DB.Close();
            }
            Log.Message($"Song '{name}' delted.");
        }
        public static void Add(string Name, string Lang, int Number, string Category, byte[] MP3)
        {
            using (SQLiteConnection DB = new SQLiteConnection($"Data source={Consts.DBPath}; Version=3"))
            {
                DB.Open();
                SQLiteCommand cmd = new SQLiteCommand($"insert into Songs(Name,Lang,Number,Category,MP3) values('{Name}','{Lang}','{Number}','{Category}',@mp3)", DB);
                cmd.Parameters.Add("@mp3", System.Data.DbType.Binary, MP3.Length);
                cmd.Parameters["@mp3"].Value = MP3;
                cmd.ExecuteNonQuery();
                DB.Close();
            }
            Log.Message($"Song '{Name}' with number '{Number}' language '{Lang}' category '{Category}' added.");
        }
        public static bool NameExist(string name, bool invert = false)
        {
            using (SQLiteConnection DB = new SQLiteConnection($"Data source={Consts.DBPath}; Version=3"))
            {
                DB.Open();
                SQLiteCommand cmd = new SQLiteCommand($"Select Name from Songs where Name = '{name}'", DB);
                using (SQLiteDataReader SQL = cmd.ExecuteReader())
                {
                    if (SQL.HasRows)
                        return true;
                }
                DB.Close();
            }
            return false;
        }
        public static byte[] GetMp3(string name)
        {
            byte[] arr = null;
            using (SQLiteConnection DB = new SQLiteConnection($"Data source={Consts.DBPath}; Version=3"))
            {
                DB.Open();
                SQLiteCommand cmd = new SQLiteCommand($"Select * from Songs where Name = '{name}'", DB);
                using (SQLiteDataReader SQL = cmd.ExecuteReader())
                {
                    if (SQL.HasRows)
                    {
                        while (SQL.Read())
                        {
                            try
                            {
                                arr = (byte[])SQL["MP3"];
                            }
                            catch (Exception e)
                            {
                                Log.Error(e.StackTrace + " => " + e.Message);
                            }
                        }
                    }
                }
                DB.Close();
            }
            Log.Message($"Loaded '{name}' from Data Base.");
            return arr;
        }
        public static void Edit(string name,string column,object value)
        {
            if (value is byte[] && column == "MP3")
            {
                using (SQLiteConnection DB = new SQLiteConnection($"Data source={Consts.DBPath}; Version=3"))
                {
                    DB.Open();
                    SQLiteCommand cmd = new SQLiteCommand($"update Songs set MP3 = @MP3 where Name ='{name}'", DB);
                    cmd.Parameters.Add("@mp3", System.Data.DbType.Binary, (value as byte[]).Length);
                    cmd.Parameters["@mp3"].Value = (byte[])value;
                    cmd.ExecuteNonQuery();
                    DB.Close();
                }
            }
            else
            {
                using (SQLiteConnection DB = new SQLiteConnection($"Data source={Consts.DBPath}; Version=3"))
                {
                    DB.Open();
                    SQLiteCommand cmd = new SQLiteCommand($"update Songs set {column} ='{value}' where Name ='{name}'", DB);
                    cmd.ExecuteNonQuery();
                    DB.Close();
                }
            }
            if(column != "Popularity")
                Log.Message($"{column} updated in row '{name}' on '{value}'.");
        }
    }
}
