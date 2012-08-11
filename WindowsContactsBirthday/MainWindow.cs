using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace WindowsContactsBirthday
{
    using Microsoft.Communications.Contacts;

    /// <summary>
    /// Main window.
    /// </summary>
    public partial class MainWindow : Form
    {

        /// <summary>
        /// Load main wondow.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            // Setup application context menu
            notifyIcon.Icon = Properties.Resources.NotificationIcon;
                 
            notifyIcon.ContextMenu = new ContextMenu();
            notifyIcon.ContextMenu.MenuItems.Add(Properties.Resources.ExitMenuItem, exitMenuItem_Click);
            // Setup background worker
            birthdayCheckWorker.DoWork += birthdayCheckWorker_DoWork;
            birthdayCheckWorker.RunWorkerCompleted += birthdayCheckWorker_Completed;
            // Prepare list of contact
            Photo.SupportNonLocalUrls = true;
            ContactUtility.RefreshContactListCompleted += ContactUtility_RefreshContactListCompleted;
            ContactUtility.RefreshContactList();

        }

        /// <summary>
        /// Manage refresh contact list completed.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event</param>
        private void ContactUtility_RefreshContactListCompleted(object sender, EventArgs e)
        {
            ShowNotification();
            // Relaunch worker
            birthdayCheckWorker.RunWorkerAsync();
        }

        /// <summary>
        /// Show notification.
        /// </summary>
        private void ShowNotification()
        {
            String message = null;
            String resource = null;
            // Today's birthday
            foreach (ContactBirthday tmp in  ContactUtility.getTodayBirthdayList())
            {
                if (tmp.age == 1)
                {
                    resource = Properties.Resources.TodayOneYearNotificationText;
                }
                else if (tmp.age >= 1)
                {
                    resource = Properties.Resources.TodayBirthdayNotificationText;
                }
                message += String.Format(resource, tmp.displayName, tmp.age, tmp.birthDate.Day, tmp.birthDate.Month) + "\n";
            }
            // Week birthday
            foreach (ContactBirthday tmp in ContactUtility.getWeekBirthdayList())
            {
                if (tmp.age == 1)
                {
                    resource = Properties.Resources.WeekOneYearNotificationText;
                }
                else if (tmp.age >= 1)
                {
                    resource = Properties.Resources.WeekBirthdayNotificationText;
                }
                message += String.Format(resource, tmp.displayName, tmp.age, tmp.birthDate.Day, tmp.birthDate.Month) + "\n";
            }

            if (message == null)
            {
                message = Properties.Resources.NoBirthdayNotificationText;
            }
            notifyIcon.ShowBalloonTip(Properties.Settings.Default.NotificationTimeoutInS * 1000, Properties.Resources.BirthdayNotificationTitle, message, ToolTipIcon.Info);
        }

        private void birthdayCheckWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(Properties.Settings.Default.WorkerInMn*60*1000);
        }

        private void birthdayCheckWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            ContactUtility.RefreshContactList();
        }
        
        /// <summary>
        /// Click on exit 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        /// <summary>
        /// Load event.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainWindow_Load(object sender, EventArgs e)
        {

        }
    }
}
