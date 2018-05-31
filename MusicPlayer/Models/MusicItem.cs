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
    public class MusicItem
    {
        private string _title;
        private string _artist;
        private string _album;
        private StorageFile _file;
        private ImageSource _imageSource;
        public string Title { get { return _title; } set { _title = value;} }
        public string Artist { get { return _artist; } set { _artist = value;} }
        public string Album { get { return _album; } set { _album = value;} }
        public StorageFile File { get { return _file; } set { _file = value; } }
        public ImageSource ImageSource { get { return _imageSource; } set { _imageSource = value; } }
        public MusicItem(string Title, string Artist, string Album, ImageSource imageSource ,StorageFile file)
        {
            this._title = Title;
            this._artist = Artist;
            this._album = Album;
            this._imageSource = imageSource;
            this._file = file;
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
