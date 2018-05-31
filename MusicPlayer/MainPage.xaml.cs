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
            Frame.Navigate(typeof(NewPage), "");
            //MusicItem item = e.ClickedItem as MusicItem;
            //var stream = await item.File.OpenAsync(FileAccessMode.Read);
            //player.SetSource(stream, "");
        }

        private void deleteMusicLibFolder(object sender, RoutedEventArgs e)
        {
            LibFolder lib = ShowFolder.SelectedItem as LibFolder;
            musicItemViewModel.deleteMusicLibFolder(lib);
        }
    }
}
