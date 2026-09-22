using System;
using System.Collections.Generic;
using System.Text;

namespace CS_09_01_API_VideoPlayer.Model
{
    public class Video
    {

        // ATTRIBUTES
        public string Title { get; set; }
        public string Creator { get; set; }
        public string Description { get; set; }
        public string Identifier { get; set; }
        public string ThumbnailUrl => $"https://archive.org/services/img/{Identifier}";

        //---- json attributes:
        // string title
        // string idenetifier
        // string creator
        // string description
        // string thumbnailUrl: "https://archive.org/services/img/" + identifier

        // METHODS
        public override string ToString()
        {
            return Title + " | " + Creator;
        }
        


    // END CLASS
    }
}
