using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace SimpleMusicPlayer
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private string SavePath = "./Data/musics.json";
        private readonly DispatcherTimer timer = new DispatcherTimer();
        private PlayState State = PlayState.NONE;
        private bool IsListOpen = false;
        private int CurrentIndex = -1;

        private static string TITLE_STR;

        public ObservableCollection<Music> Musics { get; set; } = new ObservableCollection<Music>();

        public MainWindow()
        {
            InitializeComponent();
            TITLE_STR = this.Title;

            Player.LoadedBehavior = MediaState.Manual;
            Player.UnloadedBehavior = MediaState.Manual;
            Player.Volume = 1;

            timer.Interval = TimeSpan.FromMilliseconds(500);
            timer.Tick += new EventHandler(Timer_Tick);

            ReadMusics();
        }     

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void TitleBar_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                this.DragMove();
            }
        }

        private void AlbumImage_Drop(object sender, DragEventArgs e)
        {
            var filePath = ((Array)e.Data.GetData(DataFormats.FileDrop)).GetValue(0).ToString();
            if (filePath.Split('.').Last().Equals("mp3"))
            {
                AddMusic(filePath);
                CurrentIndex = Musics.Count - 1;
                PlaySelected(CurrentIndex);             
            }
        }

        private void MusicList_Drop(object sender, DragEventArgs e)
        {
            var files = (Array)e.Data.GetData(DataFormats.FileDrop);
            foreach (string filePath in files)
            {
                if (filePath.Split('.').Last().Equals("mp3"))
                {                
                    AddMusic(filePath);
                }
            }
        }

        private void Player_MediaOpened(object sender, RoutedEventArgs e)
        {
            Progress.Maximum = Player.NaturalDuration.TimeSpan.TotalSeconds;
            TotalTime.Text = Player.NaturalDuration.TimeSpan.ToString(@"mm\:ss");
        }

        private void Player_MediaEnded(object sender, RoutedEventArgs e)
        {
            PlayIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
            Progress.Value = Player.Position.TotalSeconds;
            CurrentTime.Text = TimeSpan.FromSeconds(Progress.Value).ToString(@"mm\:ss");
            timer.Stop();
            State = PlayState.STOP;
        }

        private void PlayButton_Click(object sender, RoutedEventArgs e)
        {
            switch (State)
            {
                case PlayState.PLAY:
                    Player.Pause();
                    PlayIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Play;
                    timer.Stop();
                    State = PlayState.PAUSE;
                    break;
                case PlayState.PAUSE:
                    Player.Play();
                    PlayIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                    State = PlayState.PLAY;
                    timer.Start();
                    break;
                case PlayState.STOP:
                    PlaySelected(CurrentIndex);
                    break;
                case PlayState.NONE:
                    if (Musics.Count > 0)
                    {
                        CurrentIndex = 0;
                        PlaySelected(CurrentIndex);
                        PlayIcon.Kind = MaterialDesignThemes.Wpf.PackIconKind.Pause;
                        timer.Start();
                    }
                    break;
            }
        }

        private void ShowMusicListButton_Click(object sender, RoutedEventArgs e)
        {
            if (IsListOpen)
            {
                this.Width -= 400;
                this.TitleText.Width -= 400;
                IsListOpen = false;
            }
            else
            {
                this.Width += 400;
                this.TitleText.Width += 400;
                IsListOpen = true;
            }
        }

        private void ListViewItem_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            PlaySelected(MusicList.SelectedIndex);
            CurrentIndex = MusicList.SelectedIndex;
        }

        private void NextButton_Click(object sender, RoutedEventArgs e)
        {
            if (Musics.Count > 0)
            {
                CurrentIndex = (CurrentIndex + 1) % Musics.Count;
                PlaySelected(CurrentIndex);
            }
        }

        private void PreviousButton_Click(object sender, RoutedEventArgs e)
        {
            if (Musics.Count > 0)
            {
                CurrentIndex = --CurrentIndex < 0 ? Musics.Count - 1: CurrentIndex;
                PlaySelected(CurrentIndex);
            }
        }

        private void PlayItem_Click(object sender, RoutedEventArgs e)
        {
            PlaySelected(MusicList.SelectedIndex);
        }       

        private void DeleteItem_Click(object sender, RoutedEventArgs e)
        {
            if (CurrentIndex >= MusicList.SelectedIndex)
                CurrentIndex--;
            Musics.RemoveAt(MusicList.SelectedIndex);
            SaveMusicList();
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void Player_MediaFailed(object sender, ExceptionRoutedEventArgs e)
        {
            Console.WriteLine("Can not play.");
        }
    }
}
