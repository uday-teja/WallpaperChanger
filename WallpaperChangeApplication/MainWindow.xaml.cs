using Microsoft.Win32;
using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media.Imaging;
using Quartz.Impl;
using Quartz;
using System.Threading.Tasks;
using Quartz.Impl.Triggers;

namespace WallpaperChangeApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public ChangeWallpapaer changeBackground;
        public ISchedulerFactory SchedulerFactory { get; set; }
        public IScheduler Scheduler { get; set; }
        public string SelectedImagePath { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            changeBackground = new ChangeWallpapaer();
            changeBackground.SetBackground();
            SetScheduler();
        }

        private async void SetScheduler()
        {
            SchedulerFactory = new StdSchedulerFactory();
            Scheduler = await SchedulerFactory.GetScheduler();

            IJobDetail job = JobBuilder.Create<ChangeWallpapaer>()
                .WithIdentity("myJob", "group1")
                .Build();

            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                //.WithSimpleSchedule(s => s.WithIntervalInSeconds(5).WithRepeatCount(2))
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(this.changeBackground.TriggerHour, 00))
                .Build();

            await Scheduler.ScheduleJob(job, trigger);
            await Scheduler.Start();
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
                SelectedImagePath = openFileDialog.FileName;
                FileName.Text = SelectedImagePath;
                BitmapImage bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(SelectedImagePath);
                bitmap.EndInit();
                ImageViewer.Source = bitmap;
            }
            ApplyUserImage.Visibility = ImageViewer.Source != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ApplyUserImage_Click(object sender, RoutedEventArgs e)
        {
            this.changeBackground.SetDesktopBackground(SelectedImagePath);
        }

        private void DefaultSettings(object sender, RoutedEventArgs e)
        {
            this.changeBackground.SetBackground();
        }
    }

    internal sealed class Win32
    {
        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        internal static extern int SystemParametersInfo(
            int uAction,
            int uParam,
            String lpvParam,
            int fuWinIni);
    }

    [DisallowConcurrentExecution]
    public class ChangeWallpapaer : IJob
    {
        const int SET_DESKTOP_BACKGROUND = 20;
        const int UPDATE_INI_FILE = 1;
        const int SEND_WINDOWS_INI_CHANGE = 2;
        public int TriggerHour { get; set; }

        public void SetDesktopBackground(string imagePath)
        {
            Win32.SystemParametersInfo(SET_DESKTOP_BACKGROUND, 0, imagePath, UPDATE_INI_FILE | SEND_WINDOWS_INI_CHANGE);
        }

        public void SetBackground()
        {
            string imgPath = string.Empty;
            var timeOfDay = DateTime.Now.Hour;
            if (timeOfDay >= 0 && timeOfDay < 12)
            {
                this.TriggerHour = 12;
                imgPath = "Images\\1.jpg";
            }
            else if (timeOfDay >= 12 && timeOfDay < 16)
            {
                this.TriggerHour = 16;
                imgPath = "Images\\2.png";
            }
            else if (timeOfDay >= 16 && timeOfDay < 21)
            {
                this.TriggerHour = 21;
                imgPath = "Images\\3.jpg";
            }
            else if (timeOfDay >= 21 && timeOfDay < 24)
            {
                this.TriggerHour = 12;
                imgPath = "Images\\4.jpg";
            }
            SetDesktopBackground(AppDomain.CurrentDomain.BaseDirectory + imgPath);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => SetBackground());
            ITrigger trigger = TriggerBuilder.Create()
                .WithIdentity(context.Trigger.Key.Name)
                .StartNow()
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(this.TriggerHour, 00))
                //.WithSimpleSchedule(s => s.WithIntervalInSeconds(100).WithRepeatCount(5))
                .Build();
            await context.Scheduler.RescheduleJob(new TriggerKey(context.Trigger.Key.Name), trigger);
        }
    }
}