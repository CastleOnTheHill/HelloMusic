using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using Windows.Storage.Search;
using MusicPlayer.Models;
using MusicPlayer.ViewModel;

// https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x804 上介绍了“空白页”项模板

namespace MusicPlayer
{
    /// <summary>
    /// 可用于自身或导航至 Frame 内部的空白页。
    /// </summary>
    public sealed partial class MainPage : Page
    {
        private MusicItemViewModel musicItemViewModel = MusicItemViewModel.GetInstance();
        public MainPage()
        {
            this.InitializeComponent();
            musicItemViewModel.Init();
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }

        private void addMusicLibFolder(object sender, RoutedEventArgs e)
        {
            musicItemViewModel.addMusicLibFolder();
        }

        private void OpenFolder(object sender, ItemClickEventArgs e)
        {
            musicItemViewModel.showMusicInFolder(((LibFolder)e.ClickedItem).StorageFolder);
        }

        private void PlayMusic(object sender, ItemClickEventArgs e)
        {
            musicItemViewModel.SelectedMusicItem = (Models.MusicItem)(e.ClickedItem);
            if(Frame.CanGoForward)
            {
                Frame.GoForward();
            }
            else
            {
                Frame.Navigate(typeof(NewPage));
            }
        }

        private void deleteMusicLibFolder(object sender, RoutedEventArgs e)
        {
            LibFolder lib = ShowFolder.SelectedItem as LibFolder;
            musicItemViewModel.deleteMusicLibFolder(lib);
        }

        private void AutoSuggestBox_TextChanged(AutoSuggestBox sender, AutoSuggestBoxTextChangedEventArgs args)
        {
            if(args.Reason == AutoSuggestionBoxTextChangeReason.UserInput)
            {
                List<MusicItem> showItems = new List<MusicItem>();
                foreach (var item in musicItemViewModel.MusicItems)
                {
                    if (item.Title.Contains(sender.Text) || item.Artist.Contains(sender.Text) || item.Album.Contains(sender.Text))
                    {
                        showItems.Add(item);
                    }
                }
                if(showItems.Count == 0)
                {
                    MusicItem item = new MusicItem("没有找到QAQ", "", "", null, null);
                    showItems.Add(item);
                }
                sender.ItemsSource = showItems;
            }
        }

        private void AutoSuggestBox_QuerySubmitted(AutoSuggestBox sender, AutoSuggestBoxQuerySubmittedEventArgs args)
        {
            if (args.ChosenSuggestion != null && (args.ChosenSuggestion as MusicItem).File != null)
            {
                musicItemViewModel.selectedMusicItem = args.ChosenSuggestion as MusicItem;
                if(Frame.CanGoForward)
                {
                    Frame.GoForward();
                }
                else
                {
                    Frame.Navigate(typeof(NewPage));
                }
            }
        }
    }
}
