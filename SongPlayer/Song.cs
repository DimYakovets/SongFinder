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
        public Song(string name, short number, string category, string lang)
        {
            Name = name;
            Number = number;
            Category = category;
            Lang = lang;
        }
        public string Name { get; private set; }
        public short Number { get; private set; }
        public string Category { get; private set; }
        public string Lang { get; private set; }
    }
}
