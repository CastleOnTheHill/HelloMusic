using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml.Media.Imaging;
using Windows.Media;
using Windows.Media.Core;
using Windows.Media.Playback;
using Windows.Storage;
using Windows.Storage.Pickers;
using Windows.UI;
using Windows.Storage.FileProperties;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using Windows.Data.Xml.Dom;
using System.Text.RegularExpressions;
using Newtonsoft.Json.Linq;
using MusicPlayer.Models;
using MusicPlayer.ViewModel;
using Windows.ApplicationModel.DataTransfer;
using Windows.Storage.Streams;
using Windows.UI.Notifications;

// https://go.microsoft.com/fwlink/?LinkId=234238 上介绍了“空白页”项模板

namespace MusicPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class NewPage : Page
    {

        private MusicPlayer.ViewModel.MusicItemViewModel viewModel;
        public double value = 100;
        public MediaPlayer mediaPlayer = new MediaPlayer();
        // public MediaTimelineController mediaTimelineController = new MediaTimelineController();
        public TimeSpan duration;
        //public MediaPlaybackSession mediaPlayBackSession = new MediaPlaybackSession();

        public NewPage()
        {
            this.InitializeComponent();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
            viewModel = ViewModel.MusicItemViewModel.GetInstance();
            var mediaSource = MediaSource.CreateFromStorageFile(viewModel.SelectedMusicItem.File);
            mediaPlayer.Source = mediaSource;
            mediaSource.OpenOperationCompleted += MediaSource_OpenOperationCompleted;
            mediaPlayer.CommandManager.IsEnabled = false;
            mediaplayerelement.SetMediaPlayer(mediaPlayer);
            //EllStoryboard.Stop();
            //innerEllStoryboard.Stop();
            Volume_ProcessBar.AddHandler(PointerReleasedEvent, new PointerEventHandler(VolumeButtonPointerReleased), true);
            getLyric(Title.Text, ArtistName.Text);
            UpdatePrimaryTile();
            SelectedMusicItemChanged();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.NavigationMode == NavigationMode.Forward)
            {
                if (viewModel.SelectedMusicItem.Title != Title.Text)
                {
                    SelectedMusicItemChanged();
                }
            }
        }

        public void DisplayButton_Click(object sender, RoutedEventArgs e)
        {
            if (mediaPlayer.CurrentState == MediaPlayerState.Paused || mediaPlayer.CurrentState == MediaPlayerState.Stopped)
            {
                DispatcherTimer timer = new DispatcherTimer();
                timer.Interval = TimeSpan.FromSeconds(1);
                timer.Tick += timer_Tick;
                timer.Start();
                EllStoryboard.Begin();
                innerEllStoryboard.Begin();
                switchOn.Begin();
                mediaPlayer.Play();
                DisplayButton.Label = "暂停";
                DisplayButton.Icon = new SymbolIcon(Symbol.Pause);
            }
            else if (mediaPlayer.CurrentState == MediaPlayerState.Playing)
            {
                EllStoryboard.Pause();
                innerEllStoryboard.Pause();
                switchOff.Begin();
                mediaPlayer.Pause();
                DisplayButton.Label = "播放";
                DisplayButton.Icon = new SymbolIcon(Symbol.Play);
            }
        }

        public void StopButton_Click(object sender, RoutedEventArgs e)
        {
            EllStoryboard.Stop();
            innerEllStoryboard.Stop();
            switchOn.Stop();
            mediaPlayer.Position = TimeSpan.FromSeconds(0);
            DisplayButton.Label = "播放";
            DisplayButton.Icon = new SymbolIcon(Symbol.Play);
            mediaPlayer.Pause();
        }

        public async void OpenFileButton_Click(object sender, RoutedEventArgs e)
        {
            var openPicker = new FileOpenPicker();

            openPicker.FileTypeFilter.Add(".wmv");
            openPicker.FileTypeFilter.Add(".mp4");
            openPicker.FileTypeFilter.Add(".wma");
            openPicker.FileTypeFilter.Add(".mp3");

            var file = await openPicker.PickSingleFileAsync();

            // mediaPlayer is a MediaPlayerElement defined in XAML
            if (file != null)
            {
                var mediaSource = MediaSource.CreateFromStorageFile(file);
                mediaPlayer.Source = mediaSource;
                //time = mediaSource.Duration.GetValueOrDefault().ToString();
                //Time.Text = time;
                mediaSource.OpenOperationCompleted += MediaSource_OpenOperationCompleted;
                if (file.FileType == ".mp3" || file.FileType == ".wma")
                {
                    ellipse.Visibility = Visibility.Visible;
                    innerEllipse.Visibility = Visibility.Visible;
                    image.Visibility = Visibility.Visible;
                    Message.Visibility = Visibility.Visible;
                }
                else
                {
                    ellipse.Visibility = Visibility.Collapsed;
                    innerEllipse.Visibility = Visibility.Collapsed;
                    image.Visibility = Visibility.Collapsed;
                    Message.Visibility = Visibility.Collapsed;
                }
                using (StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 300))
                {
                    if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(thumbnail);
                        innerPicture.ImageSource = bitmapImage;
                    }
                }
                try
                {
                    //StringBuilder outputText = new StringBuilder();
                    // Get music properties
                    MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                    //outputText.AppendLine("Album: " + musicProperties.Album);
                    //outputText.AppendLine("Artist: " + musicProperties.Artist);
                    //outputText.AppendLine("Title: " + musicProperties.Title);
                    // outputText.AppendLine("Duration: " + musicProperties.Duration);
                    // lyric.Text = outputText.ToString();
                    Title.Text = musicProperties.Title;
                    ArtistName.Text = musicProperties.Artist;
                    AlbumName.Text = musicProperties.Album;
                    string total = musicProperties.Duration.ToString();
                    Time.Text = total.Substring(3, 5);
                    getLyric(musicProperties.Title, musicProperties.Artist);
                }
                // Handle errors with catch blocks
                catch (FileNotFoundException)
                {
                    // For example, handle a file not found error
                }
            }
            EllStoryboard.Stop();
            innerEllStoryboard.Stop();
            switchOn.Stop();
            mediaPlayer.Position = TimeSpan.FromSeconds(0);
            DisplayButton.Label = "播放";
            DisplayButton.Icon = new SymbolIcon(Symbol.Play);
            mediaPlayer.Pause();
        }

        public void FullScreenButton_Click(object sender, RoutedEventArgs e)
        {
            ApplicationView view = ApplicationView.GetForCurrentView();
            bool isInFullScreenMode = view.IsFullScreenMode;
            if (isInFullScreenMode)
            {
                view.ExitFullScreenMode();
            }
            else
            {
                view.TryEnterFullScreenMode();
            }
        }

        public void WideScreenButton_Click(object sender, RoutedEventArgs e)
        {
            var width = Window.Current.Bounds.Width;
            var height = Window.Current.Bounds.Height;
            var prewidth = 600;
            var preheight = 400;
            if (mediaplayerelement.Width == prewidth)
            {
                mediaplayerelement.Width = width;
                mediaplayerelement.Height = height;
            }
            else
            {
                mediaplayerelement.Width = prewidth;
                mediaplayerelement.Height = preheight;
            }
        }

        public void VolumeButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)VolumeButton.Tag == "MusicInfo")
            {
                Volume_ProcessBar.Value = 0;
                VolumeButton.Icon = new SymbolIcon(Symbol.Mute);
                VolumeButton.Tag = "Mute";
            }
            else
            {
                Volume_ProcessBar.Value = value;
                VolumeButton.Icon = new SymbolIcon(Symbol.Volume);
                VolumeButton.Tag = "MusicInfo";
            }
        }

        public void Volume_ValueChanged(object sender, RoutedEventArgs e)
        {
            mediaPlayer.Volume = (double)Volume_ProcessBar.Value;
        }

        private async void MediaSource_OpenOperationCompleted(MediaSource sender, MediaSourceOpenOperationCompletedEventArgs args)
        {
            duration = sender.Duration.GetValueOrDefault();
            await Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                Display_ProcessBar.Minimum = 0;
                Display_ProcessBar.Maximum = duration.TotalSeconds;
                Display_ProcessBar.StepFrequency = 1;
            });
        }

        void timer_Tick(object sender, object e)
        {
            Display_ProcessBar.Value = ((TimeSpan)mediaPlayer.Position).TotalSeconds;
            if (Display_ProcessBar.Value == Display_ProcessBar.Maximum)
            {
                mediaPlayer.Position = TimeSpan.FromSeconds(0);
                if ((string)DisplayOrderButton.Tag == "Circle")
                {
                    EllStoryboard.Begin();
                    innerEllStoryboard.Begin();
                    switchOn.Begin();
                    mediaPlayer.Play();
                    DisplayButton.Label = "暂停";
                    DisplayButton.Icon = new SymbolIcon(Symbol.Pause);
                }
                else
                {
                    mediaPlayer.Pause();
                    EllStoryboard.Stop();
                    switchOff.Begin();
                    innerEllStoryboard.Stop();
                    DisplayButton.Label = "播放";
                    DisplayButton.Icon = new SymbolIcon(Symbol.Play);
                }
            }
            String time;
            int minute = ((int)Display_ProcessBar.Value) / 60;
            int second = ((int)Display_ProcessBar.Value) - minute * 60;
            string minutes = minute.ToString();
            string seconds = second.ToString();
            if (minute < 10)
            {
                minutes = "0" + minute.ToString();
            }
            if (second < 10)
            {
                seconds = "0" + second.ToString();
            }
            time = minutes + ":" + seconds;
            CurrentTime.Text = time;
        }

        public void DisplayOrderButton_Click(object sender, RoutedEventArgs e)
        {
            if ((string)DisplayOrderButton.Tag == "Circle")
            {
                BitmapIcon randomIcon = new BitmapIcon();
                randomIcon.UriSource = new Uri("ms-appx:///Assets/random.png");
                DisplayOrderButton.Icon = randomIcon;
                DisplayOrderButton.Tag = "Random";
            }
            else
            {
                BitmapIcon circleIcon = new BitmapIcon();
                circleIcon.UriSource = new Uri("ms-appx:///Assets/circle.png");
                DisplayOrderButton.Icon = circleIcon;
                DisplayOrderButton.Tag = "Circle";
            }
        }

        private void VolumeButtonPointerReleased(object sender, RoutedEventArgs e)
        {
            value = Volume_ProcessBar.Value;
        }

        public async void getLyric(string title, string artist)
        {
            HttpClient http = new HttpClient();
            String url = "http://geci.me/api/lyric/" + title + "/" + artist;
            try
            {
                lyric.Text = "";
                HttpResponseMessage response = await http.GetAsync(url);
                String responseJson = await response.Content.ReadAsStringAsync();
                JObject jobject = JObject.Parse(responseJson);
                string lyr = jobject["result"][0]["lrc"].ToString();
                //lyric.Text = lyr;
                HttpResponseMessage Response = await http.GetAsync(lyr);
                String lrc = await Response.Content.ReadAsStringAsync();
                //lyric.Text = lrc;
                var array = lrc.Split('\n');
                //for (int i = 0; array[i] != ""; i++)
                //lyric.Text += array[i];
                Lrc lrcinit = InitLrc(array);
                //lyric.Text += lrcinit.Title;
                Dictionary<double, string>.ValueCollection valueCol = lrcinit.LrcWord.Values;

                foreach (string value in valueCol)
                {
                    lyric.Text += value;
                }

            }
            catch
            {
                lyric.Text = "暂无歌词";
            }
        }

        static string SplitInfo(string line)
        {
            return line.Substring(line.IndexOf(":") + 1).TrimEnd(']');
        }

        public static Lrc InitLrc(string[] array)
        {
            Lrc lrc = new Lrc();
            Dictionary<double, string> dicword = new Dictionary<double, string>();
            for (int i = 0; array[i] != ""; i++)
            {
                if (array[i].StartsWith("[ti:"))
                {
                    lrc.Title = SplitInfo(array[i]);
                }
                else if (array[i].StartsWith("[ar:"))
                {
                    lrc.Artist = SplitInfo(array[i]);
                }
                else if (array[i].StartsWith("[al:"))
                {
                    lrc.Album = SplitInfo(array[i]);
                }
                else if (array[i].StartsWith("[by:"))
                {
                    lrc.LrcBy = SplitInfo(array[i]);
                }
                else if (array[i].StartsWith("[offset:"))
                {
                    lrc.Offset = SplitInfo(array[i]);
                }
                else
                {
                    try
                    {
                        Regex regexword = new Regex(@".*\](.*)");
                        Match mcw = regexword.Match(array[i]);
                        string word = mcw.Groups[1].Value;
                        Regex regextime = new Regex(@"\[([0-9.:]*)\]", RegexOptions.Compiled);
                        MatchCollection mct = regextime.Matches(array[i]);
                        foreach (Match item in mct)
                        {
                            double time = TimeSpan.Parse("00:" + item.Groups[1].Value).TotalSeconds;
                            dicword.Add(time, word);
                        }
                    }
                    catch
                    {
                        continue;
                    }
                }
            }
            lrc.LrcWord = dicword.OrderBy(t => t.Key).ToDictionary(t => t.Key, p => p.Value);
            return lrc;
        }

        public class Lrc
        {
            /// <summary>
            /// 歌曲
            /// </summary>
            public string Title { get; set; }
            /// <summary>
            /// 艺术家
            /// </summary>
            public string Artist { get; set; }
            /// <summary>
            /// 专辑
            /// </summary>
            public string Album { get; set; }
            /// <summary>
            /// 歌词作者
            /// </summary>
            public string LrcBy { get; set; }
            /// <summary>
            /// 偏移量
            /// </summary>
            public string Offset { get; set; }

            /// <summary>
            /// 歌词
            /// </summary>
            public Dictionary<double, string> LrcWord = new Dictionary<double, string>();
        }

        private void Share_click(object sender, RoutedEventArgs e)
        {
            DataTransferManager dataTransferManager = DataTransferManager.GetForCurrentView();
            dataTransferManager.DataRequested += DataTransferManager_DataRequested;
            DataTransferManager.ShowShareUI();
        }
        private void DataTransferManager_DataRequested(DataTransferManager sender, DataRequestedEventArgs args)
        {
            DataRequest request = args.Request;
            MusicItem item = viewModel.SelectedMusicItem;
            request.Data.SetText(item.Title);
            List<StorageFile> files = new List<StorageFile>();
            files.Add(item.File);
            request.Data.SetStorageItems(files);
            request.Data.Properties.Title = item.Title;
            request.Data.Properties.Description = item.Album + " " + item.Artist;
        }
        public void UpdatePrimaryTile()
        {
            XmlDocument document = new XmlDocument();
            document.LoadXml(System.IO.File.ReadAllText("tile.xml"));
            XmlNodeList textElements = document.GetElementsByTagName("text");
            textElements[2].InnerText = viewModel.SelectedMusicItem.Title;
            textElements[3].InnerText = viewModel.SelectedMusicItem.Artist;
            textElements[4].InnerText = viewModel.SelectedMusicItem.Title;
            textElements[5].InnerText = viewModel.SelectedMusicItem.Artist;
            textElements[6].InnerText = viewModel.SelectedMusicItem.Title;
            textElements[7].InnerText = viewModel.SelectedMusicItem.Artist;
            var tileNotification = new TileNotification(document);
            TileUpdateManager.CreateTileUpdaterForApplication().Update(tileNotification);
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueue(true);
            TileUpdateManager.CreateTileUpdaterForApplication().EnableNotificationQueueForSquare310x310(true);
        }

        private void ChangeMusicToNext(object sender, RoutedEventArgs e)
        {
            int CurrPos = viewModel.MusicItems.IndexOf(viewModel.SelectedMusicItem);
            int ModelSize = viewModel.MusicItems.Count();
            int NextPos = (CurrPos + 1) % ModelSize;
            if(DisplayOrderButton.Tag as string == "Random")
            {
                Random random = new Random();
                NextPos = random.Next(ModelSize);
            }
            viewModel.SelectedMusicItem = viewModel.MusicItems.ElementAt(NextPos);
            SelectedMusicItemChanged();
        }

        private void ChangeMusicToLast(object sender, RoutedEventArgs e)
        {
            int CurrPos = viewModel.MusicItems.IndexOf(viewModel.SelectedMusicItem);
            int ModelSize = viewModel.MusicItems.Count();
            int NextPos = (CurrPos - 1 + ModelSize) % ModelSize;
            viewModel.SelectedMusicItem = viewModel.MusicItems.ElementAt(NextPos);
            SelectedMusicItemChanged();
        }

        private void SelectedMusicItemChanged()
        {
            EllStoryboard.Stop();
            innerEllStoryboard.Stop();
            switchOff.Begin();
            mediaPlayer.Pause();
            DisplayButton.Label = "播放";
            DisplayButton.Icon = new SymbolIcon(Symbol.Play);
            mediaPlayer.Source = MediaSource.CreateFromStorageFile(viewModel.SelectedMusicItem.File);
            getLyric(Title.Text, ArtistName.Text);
            Bindings.Update();
        }
    }
}
