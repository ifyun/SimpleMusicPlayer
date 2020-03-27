using Id3Lib;
using Mp3Lib;
using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;

namespace SimpleMusicPlayer
{
    /// <summary>
    /// MainWindow 业务逻辑
    /// </summary>
    partial class MainWindow
    {
        private BitmapImage BitmapToBitmapImage(Bitmap bitmap)
        {
            var bitmapImage = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Bmp);
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = ms;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
                bitmapImage.Freeze();
            }
            bitmap.Dispose();
            return bitmapImage;
        }

        /// <summary>
        /// Read musics from json file.
        /// </summary>
        private void ReadMusics()
        {
            if (File.Exists(SavePath))
            {
                try
                {
                    using (var file = new StreamReader(SavePath))
                    {
                        string musics = file.ReadToEnd();
                        Musics = JsonConvert.DeserializeObject<ObservableCollection<Music>>(musics);
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }

                for (var i = 0; i < Musics.Count; i++)
                {
                    if (File.Exists(Musics[i].Path))
                    {
                        try
                        {
                            var tag = new Mp3File(Musics[i].Path).TagHandler;
                            Musics[i].AlbumImage = BitmapToBitmapImage(new Bitmap(tag.Picture));
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                            Musics[i].AlbumImage = null;
                        }
                    }
                    else
                    {
                        Musics.RemoveAt(i);
                        i--;
                    }
                }

                MusicList.DataContext = Musics;
                SaveMusicList();
            }
        }

        /// <summary>
        /// Save musics to json file.
        /// </summary>
        private void SaveMusicList()
        {
            var musics = JsonConvert.SerializeObject(Musics);
            using (var file = new StreamWriter(SavePath))
            {
                file.Write(musics);
            }
        }

        /// <summary>
        /// Add music to Collection.
        /// </summary>
        /// <param name="path">Path of music file</param>
        private void AddMusic(string path)
        {
            Music music;

            try
            {
                var tag = new Mp3File(path).TagHandler;
                music = new Music(tag.Title, tag.Artist, tag.Album, BitmapToBitmapImage(new Bitmap(tag.Picture)), path);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                // No ID3 Tag
                music = new Music(path.Substring(path.LastIndexOf('\\') + 1), "None", "None", null, path);
            }

            if (!Musics.Contains(music))
            {             
                Musics.Add(music);
                MusicList.Items.Refresh();
                SaveMusicList();
            }
        }

        /// <summary>
        /// Synchronized to ProgressBar.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Timer_Tick(object sender, EventArgs e)
        {
            Progress.Value = Player.Position.TotalSeconds;
            CurrentTime.Text = TimeSpan.FromSeconds(Progress.Value).ToString(@"mm\:ss");
        }

        /// <summary>
        /// Play selected music.
        /// </summary>
        /// <param name="index">The index of selected music</param>
        private void PlaySelected(int index)
        {
            if (Play(Musics[index].Path))
            {

                foreach (Music music in Musics)
                {
                    music.IsSelected = false;
                }

                Musics[index].IsSelected = true;
                MusicList.Items.Refresh();

                AlbumImage.Source = Musics[index].AlbumImage != null ?
                    Musics[index].AlbumImage : new BitmapImage(new Uri("./Images/Start.jpg", UriKind.Relative));
                this.Title = TITLE_STR + " - " + Musics[index].Title;
                CurrentTitle.Text = Musics[index].Title;
                CurrentArtist.Text = Musics[index].Artist;
            }
        }

        /// <summary>
        /// Play from the file path.
        /// </summary>
        /// <param name="path">The path of music file</param>
        private bool Play(string path)
        {
            Player.Stop();
            if (File.Exists(path))
            { 
                Player.Source = new Uri(path);
                Player.Play();
                State = PlayState.PLAY;

                timer.Start();
                PlayIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;

                return true;
            }
            else
            {
                var index = Musics.ToList().FindIndex(a => a.Path.Equals(path));

                if (index != -1)
                {
                    Musics.RemoveAt(index);
                    SaveMusicList();
                }

                return false;
            }
        }
    }
}
