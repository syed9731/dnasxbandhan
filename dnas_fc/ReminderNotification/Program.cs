using ReminderNotification.BAL;
using ReminderNotification.Common;

namespace Scheduler.ReminderNotification
{
    public class Program()
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("Notification Scheduler Start");
            SchedulerLogger.LogwriteInfo("Notification Scheduler Start", "Notification");
            NotifyBL notifyBL = new NotifyBL();
            await notifyBL.SendNotificationMail();
            Console.WriteLine("Notification Scheduler End");
            SchedulerLogger.LogwriteInfo("Notification Scheduler End", "Notification");
            Environment.Exit(0);
        }
    }
}
