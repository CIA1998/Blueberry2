using System;
using System.Diagnostics;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using BlueberryPie.statics;
using BlueberryPie.Statics;

namespace Blueberry2
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Settings : Page
    {
        //public ElementTheme theme;
        private String name;
        private String version;
        private String localIP;
        private String directory;
        //public bool IsDarkTheme { get { return (bool)Application.Current.Resources["IsDarkTheme"]; } }

        public Settings()
        {
            this.InitializeComponent();
            PageCore.RequestedTheme = GlobalSettings.appTheme;
            SetRadioButtons();          // Setting what radio button is checked depending on the system theme
            SetStrings();               // Setting the version, description, and other strings on the setting page
        }

        /// <summary>
        ///     Display the version and the description to the user
        /// </summary>
        private void SetStrings()
        {
            versionTextBlock.Text += Misc.version;
            descriptionTextBlock.Text = Misc.description;
        }

        /// <summary>
        ///     Check the selected radion button
        /// </summary>
        private void SetRadioButtons()
        {
            ElementTheme et = GlobalSettings.appTheme;
            if (et.Equals(ElementTheme.Light))
                themeLightRadio.IsChecked = true;
            if (et.Equals(ElementTheme.Dark))
                themeDarkRadio.IsChecked = true;
            if (et.Equals(ElementTheme.Default))
                themeDefaultRadio.IsChecked = true;
        }

        public String GetName()
        {
            return this.name;
        }

        public String GetVersion()
        {
            return this.version;
        }

        public String GetLocalIP()
        {
            return this.localIP;
        }

        public String GetDirectory()
        {
            return this.directory;
        }

        /// <summary>
        ///     Updates theme in the Settings class (globally accessible)
        /// </summary>
        public void SetTheme(ElementTheme theme)
        {
            GlobalSettings.appTheme = theme;
            PageCore.RequestedTheme = GlobalSettings.appTheme;
        }

        /// <summary>
        ///     Check wat is the current system theme and set it to the opposite
        /// </summary>
        private void SystemTheme()
        {
            var DefaultTheme = new Windows.UI.ViewManagement.UISettings();
            var uiTheme = DefaultTheme.GetColorValue(Windows.UI.ViewManagement.UIColorType.Background).ToString();
            ElementTheme systemTheme = (uiTheme != "FF000000") ? ElementTheme.Light : ElementTheme.Dark;
            GlobalSettings.appTheme = systemTheme;
            themeDefaultRadio.IsChecked = true;
            SetTheme(GlobalSettings.appTheme);
        }

        /// <summary>
        ///     Set the theme based on which selection was made
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ThemeChecked(object sender, RoutedEventArgs e)
        {
            RadioButton rb = sender as RadioButton;

            if (rb != null)
            {
                string themeName = rb.Tag.ToString();
                switch (themeName)
                {
                    case "Light":
                        GlobalSettings.appTheme = ElementTheme.Light;
                        break;
                    case "Dark":
                        GlobalSettings.appTheme = ElementTheme.Dark;
                        break;
                    case "Default":
                        SystemTheme();  // Easier to call a method that will set the default windows theme to a variable
                        break;
                }
                SetTheme(GlobalSettings.appTheme);
            }
        }

        /// <summary>
        ///     Show the menu options when menu button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ShowMenu(object sender, RoutedEventArgs e)
        {
            sideMenu.IsPaneOpen = !sideMenu.IsPaneOpen;
            Debug.WriteLine("Event");
        }

        /// <summary>
        ///     Open the song list page
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenSongsPage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(UIChooseSong));
        }

        /// <summary>
        ///     Open the MainPage which contains the queue with the songs that have to be played.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OpenHomePage(object sender, RoutedEventArgs e)
        {
            this.Frame.Navigate(typeof(MainPage));
        }
    }
}
