using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;

namespace SongPlayer
{
    public class Song
    {
        public Song(string name, short number, string category, string lang, short popularity)
        {
            Name = name;
            Number = number;
            Category = category;
            Lang = lang;
            Popularity = popularity;
        }
        public void PopularityUp()
        {
            Popularity++;
        }
        public string Name { get; private set; }
        public short Number { get; private set; }
        public short Popularity { get; private set; }
        public string Category { get; private set; }
        public string Lang { get; private set; }

        public string LangString
        {
            get
            {
                return Settings.GetWord(Words.Language) + " : " + Lang;
            }
        }
        public string NumberString
        {

            get
            {
                var num = Number == 0 ? $"{Settings.GetWord(Words.Rewritten)}" : Number.ToString();
                return Settings.GetWord(Words.Number) + " : " + num;
            }
        }
        public string CategoryString
        {
            get
            {
                return Settings.GetWord(Words.Category) + " : " + Category;
            }
        }
    }
}
