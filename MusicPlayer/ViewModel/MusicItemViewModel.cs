using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MusicPlayer.Models;
using Windows.Foundation.Collections;
using Windows.Storage;
using Windows.Storage.Search;
using Windows.UI.Xaml.Media;
using Windows.Storage.FileProperties;
using System.IO;
using Windows.UI.Xaml.Media.Imaging;

namespace MusicPlayer.ViewModel
{
    public class MusicItemViewModel
    {

        public Models.MusicItem selectedMusicItem = default(Models.MusicItem);
        public Models.MusicItem SelectedMusicItem { get { return selectedMusicItem; } set { this.selectedMusicItem = value; } }

        public Models.LibFolder selectedFolderItem = default(Models.LibFolder);
        public Models.LibFolder SelectedFolderItem { get { return selectedFolderItem; } set { this.selectedFolderItem = value; } }

        private ObservableCollection<MusicItem> musicItems;
        private ObservableCollection<LibFolder> libFolders;
        private static MusicItemViewModel Instance;
        private MusicItemViewModel()
        {
            musicItems = new ObservableCollection<MusicItem>();
            libFolders = new ObservableCollection<LibFolder>();
        }
        private static async void InitLib()
        {
            musicLib = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
        }
        public static StorageLibrary musicLib;
        public ObservableCollection<MusicItem> MusicItems { get { return musicItems; } }
        public ObservableCollection<LibFolder> LibFolders { get { return libFolders; } }
        public static MusicItemViewModel GetInstance()
        {
            if(Instance == null)
            {
                InitLib();
                Instance = new MusicItemViewModel();
            }
            return Instance;
        }
        private void addMusicItem(string Title, string Artist, string Album, ImageSource imageSource, StorageFile file)
        {
            MusicItem musicItem = new MusicItem(Title, Artist, Album, imageSource, file);
            musicItems.Add(musicItem);
        }
        private void addLibFolder(string name, StorageFolder folder)
        {
            LibFolder libfo = new LibFolder(name, folder);
            if(!libFolders.Contains(libfo))
            {
                libFolders.Add(libfo);
            }
        }

        public void Init()
        {
            Update();
        }
        
        public async void showMusicInFolder(StorageFolder folder)
        {
            musicItems.Clear();
            QueryOptions queryOptions = new QueryOptions(CommonFileQuery.OrderByTitle, new string[] { ".mp3", ".wma" });
            queryOptions.FolderDepth = FolderDepth.Deep;
            var files = await folder.CreateFileQueryWithOptions(queryOptions).GetFilesAsync();
            foreach (var file in files)
            {
                string title = "", artist = "", album = "";
                ImageSource image = null;
                using (StorageItemThumbnail thumbnail = await file.GetThumbnailAsync(ThumbnailMode.MusicView, 300))
                {
                    if (thumbnail != null && thumbnail.Type == ThumbnailType.Image)
                    {
                        var bitmapImage = new BitmapImage();
                        bitmapImage.SetSource(thumbnail);
                        image = bitmapImage;
                    }
                }
                try
                {
                    MusicProperties musicProperties = await file.Properties.GetMusicPropertiesAsync();
                    title = musicProperties.Title;
                    artist = musicProperties.Artist;
                    album = musicProperties.Album;
                    addMusicItem(title, artist, album, image, file);
                }
                catch (FileNotFoundException)
                {
                    // For example, handle a file not found error
                }
            }
        }

        public async void addMusicLibFolder()
        {
            var myMusic = await Windows.Storage.StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            if (myMusic != null)
            {
                Windows.Storage.StorageFolder storageFolder = await myMusic.RequestAddFolderAsync();
                Update();
            }
        }

        public async void deleteMusicLibFolder(LibFolder folder)
        {
            if (await musicLib.RequestRemoveFolderAsync(folder.StorageFolder))
            {
                LibFolders.Remove(folder);
                MusicItems.Clear();
            }
        }

        private async void Update()
        {
            var myMusic = await Windows.Storage.StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            IObservableVector<StorageFolder> MusicFolder = myMusic.Folders;
            foreach (var item in MusicFolder)
            {
                string name = item.DisplayName;
                addLibFolder(name, item);
            }
        }
    }
}
