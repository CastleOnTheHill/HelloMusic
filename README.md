# HelloMusic

## 1. 类

1. `MusicItem` ：描述音乐文件，

   ```c#
   public string Title { get { return _title; } set { _title = value;} } // 歌曲名
   public string Artist { get { return _artist; } set { _artist = value;} } // 演唱者
   public string Album { get { return _album; } set { _album = value;} } // 专辑
   public StorageFile File { get { return _file; } set { _file = value; } } // 歌曲文件
   public ImageSource ImageSource { get { return _imageSource; } set { _imageSource = value; } } // 歌曲封面
   // 唯一构造函数
   public MusicItem(string Title, string Artist, string Album, ImageSource imageSource ,StorageFile file)
   ```

   ​

2.  `LibFolder` ： 描述音乐库文件夹，文件夹名和文件夹对象。

   ```c#

   public string FolderName { get { return _folderName; } set { _folderName = value; } }
   public StorageFolder StorageFolder { get { return _storageFolder; } set { _storageFolder = value; } }
   // 构造函数
    public LibFolder(string name, StorageFolder folder)
   ```

3.  `MusicItemViewModel` : 音乐文件和音乐库文件夹的容器，提供音乐库文件夹的增删操作。

   ```c#
   public static StorageLibrary musicLib; //音乐库文件
   // 当前显示的歌曲文件
   public ObservableCollection<MusicItem> MusicItems { get { return musicItems; } } 
   // 当前音乐库中的文件夹
   public ObservableCollection<LibFolder> LibFolders { get { return libFolders; } }
   ```

## 2. 接口

1. `MusicItem` 类，`LibFolder` 类，直接通过`Public` 属性赋值和获取。
2. `MusicItemViewModel` 类，`Public` 属性获取音乐库文件， 当前显示的歌曲文件，当前音乐库中的文件夹。
   - `GetInstance(void)`: 获取`MusicItemViewModel` 的单例对象
   - `showMusicInFolder(StorageFolder folder)` :  将`folder`文件夹中的音乐文件放入`MusicItems`容器中以绑定显示。`showMusicInFolder(musicLib.folder)` 可获得全部音乐库中的所有音乐文件。
   - `addMusicLibFolder(void)` : 使用类似于`FilePicker` 的方式让用户选择文件夹，并添加到音乐库中。
   - `deleteMusicLibFolder(LibFolder folder)` ：删除音乐库中的`folder`文件夹(并不会删除本地文件)。
   - `Init()` 用来加载用户上次启动已存在的音乐库文件夹到容器中，**只在页面初始化的时候调用**。"# HelloMusic" 
