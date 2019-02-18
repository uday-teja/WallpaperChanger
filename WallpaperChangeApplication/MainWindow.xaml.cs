using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WallpaperChangeApplication.Properties;

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

        int iBackground = 0;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            iBackground++;
            if (iBackground > 3) iBackground = 1;

            string imagePath = "C:\\Users\\udayt\\Desktop\\WallpaperImages\\" + iBackground + ".jpg";
            Set_Desktop_Background(imagePath);

        }

        private void Set_Desktop_Background(string imagePath)
        {
            const int SET_DESKTOP_BACKGROUND = 20;
            const int UPDATE_INI_FILE = 1;
            const int SEND_WINDOWS_INI_CHANGE = 2;

            string theDirectory = AppDomain.CurrentDomain.BaseDirectory;
            Win32.SystemParametersInfo(SET_DESKTOP_BACKGROUND, 0, imagePath, UPDATE_INI_FILE | SEND_WINDOWS_INI_CHANGE);
        }

        private string GetImage()
        {
            if (DateTime.Now.Hour < 12)
            {
                return "..\\Images\\1.jpg";
            }
            else if (DateTime.Now.Hour < 17)
            {
                return @"Images\2.jpg";
            }
            else
            {
                return @"Images\3.jpg";
            }
        }
    }
}