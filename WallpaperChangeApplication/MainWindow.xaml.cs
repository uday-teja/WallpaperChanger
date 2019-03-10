using Microsoft.Win32;
using System;
using System.Windows;
using System.Windows.Media.Imaging;
using Quartz.Impl;
using Quartz;

namespace WallpaperChangeApplication
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Wallpapaer ChangeBackground;
        private ISchedulerFactory SchedulerFactory { get; set; }
        private IScheduler Scheduler { get; set; }
        private string SelectedImagePath { get; set; }

        public MainWindow()
        {
            InitializeComponent();
            ChangeBackground = new Wallpapaer();
            ChangeBackground.SetBackground();
            SetScheduler();
        }

        private async void SetScheduler()
        {
            SchedulerFactory = new StdSchedulerFactory();
            Scheduler = await SchedulerFactory.GetScheduler();

            var job = JobBuilder.Create<Wallpapaer>()
                .WithIdentity("myJob", "group1")
                .Build();

            var trigger = TriggerBuilder.Create()
                .WithIdentity("myTrigger", "group1")
                .StartNow()
                //.WithSimpleSchedule(s => s.WithIntervalInSeconds(5).WithRepeatCount(2))
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(this.ChangeBackground.TriggerHour, 00))
                .Build();

            await Scheduler.ScheduleJob(job, trigger);
            await Scheduler.Start();
        }

        private void BrowseButton_Click(object sender, RoutedEventArgs e)
        {
            var openFileDialog = new OpenFileDialog
            {
                Filter = "Image files (*.jpg)|*.jpg|All Files (*.*)|*.*",
                InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
            };
            if (openFileDialog.ShowDialog() == true)
            {
                SelectedImagePath = openFileDialog.FileName;
                FileName.Text = SelectedImagePath;
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(SelectedImagePath);
                bitmap.EndInit();
                ImageViewer.Source = bitmap;
            }
            ApplyUserImage.Visibility = ImageViewer.Source != null ? Visibility.Visible : Visibility.Collapsed;
        }

        private void ApplyUserImage_Click(object sender, RoutedEventArgs e)
        {
            this.ChangeBackground.SetDesktopBackground(SelectedImagePath);
        }

        private void DefaultSettings(object sender, RoutedEventArgs e)
        {
            this.ChangeBackground.SetBackground();
        }
    }
}