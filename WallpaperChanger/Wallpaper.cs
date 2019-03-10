using System;
using System.Threading.Tasks;
using Quartz;

namespace WallpaperChangeApplication
{
    [DisallowConcurrentExecution]
    public class Wallpapaer : IJob
    {
        const int SET_DESKTOP_BACKGROUND = 20;
        const int UPDATE_INI_FILE = 1;
        const int SEND_WINDOWS_INI_CHANGE = 2;
        public int TriggerHour { get; set; }

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

        public void SetDesktopBackground(string imagePath)
        {
            Helper.Win32.SystemParametersInfo(SET_DESKTOP_BACKGROUND, 0, imagePath, UPDATE_INI_FILE | SEND_WINDOWS_INI_CHANGE);
        }

        public async Task Execute(IJobExecutionContext context)
        {
            await Task.Run(() => SetBackground());
            var trigger = TriggerBuilder.Create()
                .WithIdentity(context.Trigger.Key.Name)
                .StartNow()
                .WithSchedule(CronScheduleBuilder.DailyAtHourAndMinute(this.TriggerHour, 00))
                //.WithSimpleSchedule(s => s.WithIntervalInSeconds(100).WithRepeatCount(5))
                .Build();
            await context.Scheduler.RescheduleJob(new TriggerKey(context.Trigger.Key.Name), trigger);
        }
    }
}
