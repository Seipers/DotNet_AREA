using System;

namespace Area.Models
{
    public class WoWCharacterModel
    {
        public int id { get; set;}
        public string username { get; set;}
        public string name { get; set;}
        public string realm { get; set;}
        public int level { get; set;}
        public int achievementPoints { get; set;}
        public int fishCaught { get; set;}
        public int deaths { get; set;}
        public int questsDone { get; set;}
        public int facepalmed { get; set;}
        public string guild { get; set;}
        public long lastModified { get; set;}
    }
}