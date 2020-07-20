using System;
using System.Collections.Generic;
using System.Text;

namespace SpotifyTransferVkMusic
{
    public class Song
    {
        public string Author { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }

        public Song(string author, string name, string uri)
        {
            Author = author;
            Name = name;
            Uri = uri;
        }
    }
}
