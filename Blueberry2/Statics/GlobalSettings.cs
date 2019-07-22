using Windows.UI.Xaml;

namespace BlueberryPie.statics
{
    class GlobalSettings
    {
        /// <summary>
        ///     Keeping the global settings of the app in a static class as to make them globally accessible
        /// </summary>
        /// <param name="appTheme">The current theme of the app</param>
        public static ElementTheme appTheme = ElementTheme.Default;
    }
}
