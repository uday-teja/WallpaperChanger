using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;

namespace WallpaperChangeApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        internal sealed class Win32
        {
            [DllImport("user32.dll", CharSet = CharSet.Auto)]
            internal static extern int SystemParametersInfo(
                int uAction,
                int uParam,
                String lpvParam,
                int fuWinIni);
        }

        const int SET_DESKTOP_BACKGROUND = 20;
        const int UPDATE_INI_FILE = 1;
        const int SEND_WINDOWS_INI_CHANGE = 2;
        public string selectedImagePath { get; set; }

        public MainWindow()
        {
            InitializeComponent();
        }

        private void SetDesktopBackground(string imagePath)
        {
            Win32.SystemParametersInfo(SET_DESKTOP_BACKGROUND, 0, imagePath, UPDATE_INI_FILE | SEND_WINDOWS_INI_CHANGE);
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                selectedImagePath = openFileDialog.FileName;
                FileName.Text = selectedImagePath;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(selectedImagePath);
                bitmap.EndInit();
                ImageViewer.Source = bitmap;
            }
            ApplyUserImage.Visibility = ImageViewer.Source != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ApplyUserImage_Click(object sender, RoutedEventArgs e)
        {
            SetDesktopBackground(selectedImagePath);
        }

        private void DefaultSettings(object sender, RoutedEventArgs e)
        {
            string imgPath;
            if (DateTime.Now.Hour < 12)
            {
                imgPath = "Images\\1.jpg";
            }
            else if (DateTime.Now.Hour < 17)
            {
                imgPath = "Images\\2.png";
            }
            else
            {
                imgPath = "Images\\3.jpg";
            }
            SetDesktopBackground(AppDomain.CurrentDomain.BaseDirectory + imgPath);
        }
    }
}