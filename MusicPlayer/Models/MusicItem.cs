using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Xaml.Media;

namespace MusicPlayer.Models
{
    public class MusicItem : INotifyPropertyChanged
    {
        private string _title;
        private string _artist;
        private string _album;
        private string _totalTime;
        private StorageFile _file;
        private ImageSource _imageSource;
        public string Title { get { return _title; } set { _title = value; OnPropertyChanged(); } }
        public string Artist { get { return _artist; } set { _artist = value; OnPropertyChanged(); } }
        public string Album { get { return _album; } set { _album = value; OnPropertyChanged(); } }
        public string TotalTime { get { return _totalTime; } set { _totalTime = value; OnPropertyChanged(); } }
        public StorageFile File { get { return _file; } set { _file = value; OnPropertyChanged(); } }
        public ImageSource ImageSource { get { return _imageSource; } set { _imageSource = value; OnPropertyChanged(); } }
        public MusicItem(string Title, string Artist, string Album, string TotalTime, ImageSource imageSource ,StorageFile file)
        {
            this._title = Title;
            this._artist = Artist;
            this._album = Album;
            this._totalTime = TotalTime;
            this._imageSource = imageSource;
            this._file = file;
        }
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class LibFolder : IEquatable<LibFolder>
    {
        private string _folderName;
        private StorageFolder _storageFolder;
        public string FolderName { get { return _folderName; } set { _folderName = value; } }
        public StorageFolder StorageFolder { get { return _storageFolder; } set { _storageFolder = value; } }
        public LibFolder(string name, StorageFolder folder)
        {
            this._folderName = name;
            this._storageFolder = folder;
        }
        public bool Equals(LibFolder other)
        {
            return _folderName == other._folderName && _storageFolder.Path == other._storageFolder.Path;
        }
    }
}
