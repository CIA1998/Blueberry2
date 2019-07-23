using BlueberryPie.statics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Windows.Foundation;
using Windows.Storage;
using Windows.Storage.FileProperties;
using Windows.Storage.Search;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Blueberry2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class UIChooseSong : Page
    {
        StorageLibrary musicLibrary;
        String pathToMusic;
        private IAsyncOperation<StorageFolder> files;
        private List<Song> _songsList = new List<Song>();

        //internal List<Song> SongsList { get => _songsList; set => _songsList = value; }
        private Frame rootFrame;
        private MainPage page;
        public UIChooseSong()
        {
            this.InitializeComponent();
            PageCore.RequestedTheme = GlobalSettings.appTheme;
            SetDefaultPath();
            this.DataContext = _songsList;
            rootFrame = Window.Current.Content as Frame;
            page = rootFrame.Content as MainPage;
        }

        /// <summary>
        ///     Set default path to the MusicLibrary
        /// </summary>
        private async void SetDefaultPath()
        {
            musicLibrary = await StorageLibrary.GetLibraryAsync(KnownLibraryId.Music);
            pathToMusic = musicLibrary.SaveFolder.Path;
            Debug.WriteLine("PATH TO MUSIC " + pathToMusic);

            GetFilesInFolder();
        }

        //public IAsyncOperation<StorageFolder> UpdateQueueList()
        //{
        //    files = Windows.Storage.StorageFolder.GetFolderFromPathAsync(pathToMusic);
        //    return files;

        //    // To avoid permission errors for now the 15063 is the suggested lowest version
        //}

        public async void GetFilesInFolder()
        {
            List<IStorageItem> filesInFolder = new List<IStorageItem>();

            // Get the app's installation folder.
            StorageFolder appFolder = Windows.ApplicationModel.Package.Current.InstalledLocation;

            // Get the items in the current folder.
            StorageItemQueryResult itemsInFolder = KnownFolders.MusicLibrary.CreateItemQuery();

            // Iterate over the results and print the list of items
            // to the Visual Studio Output window.
            foreach (IStorageItem item in await itemsInFolder.GetItemsAsync())
            {
                // Adding all the files to the variable
                if (item.IsOfType(StorageItemTypes.File))
                {
                    filesInFolder.Add(item);
                }
            }
            FillQueueDataGrid(filesInFolder);
        }

        //Add a song from the song list to the song queue on the MainPage
        public void AddSongToQueue(object sender, RoutedEventArgs e)
        {
            page.MakePlayAllVisible();
            // Add this song to queue
            //Retrieve the specific row from the DataGrid
            DataGridRow row = DataGridRow.GetRowContainingElement((DataGridCell)((Button)sender).Parent);
            Song chosenSong = (Song)row.DataContext;
            Debug.WriteLine(chosenSong.songName);
            //Debug.WriteLine("SONG ARTIST " + chosenSong.ToString());
            page.GetPlayer().AddSongToQueue(chosenSong);
            Debug.WriteLine(page.GetPlayer().GetSongs().Count);
            page.UpdateQueueList();
        }

        /// <summary>
        ///     Populate the Song list datagrid with the files from the given folder. Additionally create  a song object for every song in the folder.
        /// </summary>
        /// <param name="filesInFolder"></param>
        public async void FillQueueDataGrid(List<IStorageItem> filesInFolder)
        {
            int i = 1;

            foreach (StorageFile selectedFile in filesInFolder)
            {

                // Selecting all the audio files
                if (selectedFile.ContentType.Contains("audio/"))
                {
                    // Putting the file name in a variable for conveniece
                    string fileName = selectedFile.DisplayName;

                    // Getting the data of the selected audio file
                    MusicProperties musicProperties = await selectedFile.Properties.GetMusicPropertiesAsync();
                    string songArtist = musicProperties.Artist;
                    string songName = musicProperties.Title;
                    string songLength = musicProperties.Duration.ToString();
                    string songPath = pathToMusic + "\\" + selectedFile.Name;
                    Debug.WriteLine(songPath);

                    // Checking if the file has Artist and Title and if not generating them from the name of the file
                    if (songArtist.Equals("") || songName.Equals(""))
                    {
                        // Playing safe if the file name doesn't have dash (-) separation for artist and title
                        try
                        {
                            songArtist = fileName.Substring(0, fileName.IndexOf("-")).Trim();
                            songName = fileName.Substring(0, fileName.LastIndexOf("-") + 1);
                        }
                        catch
                        {
                            songArtist = "Blueberry Pie";
                            songName = fileName;
                        }
                    }

                    //Cretae the song object for current song file
                    Song song = new Song { songArtist = songArtist, songName = songName, songLenght = songLength, songPath = songPath };
                    _songsList.Add(song);
                    i++;
                }
                // Forcing the update of the DataGrid
                UpdateMusicList();
            }
        }

        /// <summary>
        ///     
        /// </summary>
        private void UpdateMusicList()
        {
            // Setting item source to null and then to _songsList to force refresh the DataGrid
            songGrid.ItemsSource = null;
            songGrid.ItemsSource = _songsList;
        }

        private void ShowMenu(object sender, RoutedEventArgs e)
        {
            sideMenu.IsPaneOpen = !sideMenu.IsPaneOpen;
            Debug.WriteLine("Event");
        }

        private void OpenHomePage(object sender, RoutedEventArgs e)
        {

            this.Frame.Navigate(typeof(MainPage));
        }

        private void OpenSettingsPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(Settings));
        }
    }
}
