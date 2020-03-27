using Newtonsoft.Json;
using System.Windows.Media;

namespace SimpleMusicPlayer
{
    public class Music
    {
        public string Title { get; set; }
        public string Artist { get; set; }
        public string Album { get; set; }

        [JsonIgnore]
        public ImageSource AlbumImage { get; set; }
        public string Path { get; set; }

        [JsonIgnore]
        public bool IsSelected { set; get; }

        public Music() { }

        public Music(string title, string artist, string album, ImageSource albumImage, string path)
        {
            Title = title;
            Artist = artist;
            Album = album;
            AlbumImage = albumImage;
            Path = path;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Music))
                return false;
            return Path == (obj as Music).Path;
        }

        public override int GetHashCode()
        {
            return Path.GetHashCode();
        }
    }
}
