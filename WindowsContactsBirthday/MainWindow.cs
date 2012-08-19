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
        /// Close from menu.
        /// </summary>
        Boolean closeFromMenu = false;

        /// <summary>
        /// Load main wondow.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            // Setup application icon
            notifyIcon.Icon = Properties.Resources.NotificationIcon;
            // Setup application context menu     
            notifyIcon.ContextMenu = new ContextMenu();
            notifyIcon.ContextMenu.MenuItems.Add(Properties.Resources.ApplicationMenuItemShowNotification, showNotificationMenuItem_Click);
            notifyIcon.ContextMenu.MenuItems.Add(Properties.Resources.ApplicationMenuItemShowBirthday, showBirthdateMenuItem_Click);
            notifyIcon.ContextMenu.MenuItems.Add("Import note->anniversaire", importNoteBirthdateMenuItem_Click);
            notifyIcon.ContextMenu.MenuItems.Add(Properties.Resources.ApplicationMenuItemExit, exitMenuItem_Click);
            // Setup background worker
            birthdayCheckWorker.DoWork += birthdayCheckWorker_DoWork;
            birthdayCheckWorker.RunWorkerCompleted += birthdayCheckWorker_Completed;
            // Prepare list of contact
            Photo.SupportNonLocalUrls = true;
            ContactControler.RefreshContactListCompleted += ContactUtility_RefreshContactListCompleted;
            ContactControler.RefreshContactList();

            grdView.AutoGenerateColumns = false;

        }

        /// <summary>
        /// Import note to birthdate
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void importNoteBirthdateMenuItem_Click(object sender, EventArgs e)
        {
            ContactControler.importNoteToBirthdate();
        }

        /// <summary>
        /// Show notification.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void showNotificationMenuItem_Click(object sender, EventArgs e)
        {
            ShowNotification();
        }

        /// <summary>
        /// Show birthday list.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void showBirthdateMenuItem_Click(object sender, EventArgs e)
        {
            this.WindowState = FormWindowState.Normal;
            this.Visible = true;
        }

        /// <summary>
        /// Manage refresh contact list completed.
        /// </summary>
        /// 
        /// 
        private void ContactUtility_RefreshContactListCompleted()
        {
            grdView.DataSource = ContactControler.getBirthdayList();
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
            foreach (ContactBirthday tmp in  ContactControler.getTodayBirthdayList())
            {
                if (tmp.age == 1)
                {
                    resource = Properties.Resources.ApplicationNotificationTextTodayOneYear;
                }
                else if (tmp.age >= 1)
                {
                    resource = Properties.Resources.ApplicationNotificationTextTodayBirthday;
                }
                message += String.Format(resource, tmp.displayName, tmp.age, tmp.birthDate.Day, tmp.birthDate.Month) + "\n";
            }
            // Week birthday
            foreach (ContactBirthday tmp in ContactControler.getWeekBirthdayList())
            {
                if (tmp.age == 1)
                {
                    resource = Properties.Resources.ApplicationNotificationTextWeekOneYear;
                }
                else if (tmp.age >= 1)
                {
                    resource = Properties.Resources.ApplicationNotificationTextWeekBirthday;
                }
                message += String.Format(resource, tmp.displayName, tmp.age, tmp.birthDate.Day, tmp.birthDate.Month) + "\n";
            }

            if (message == null)
            {
                message = Properties.Resources.ApplicationNotificationTextNoBirthday;
            }
            notifyIcon.ShowBalloonTip(Properties.Settings.Default.NotificationTimeoutInS * 1000, Properties.Resources.ApplicationNotificationTitle, message, ToolTipIcon.Info);
        }

        private void birthdayCheckWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            System.Threading.Thread.Sleep(Properties.Settings.Default.WorkerInMn*60*1000);
        }

        private void birthdayCheckWorker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            ContactControler.RefreshContactList();
        }
        
        /// <summary>
        /// Click on exit 
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void exitMenuItem_Click(object sender, EventArgs e)
        {
            closeFromMenu = true;
            this.Close();
        }

        /// <summary>
        /// Load application event.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void MainWindow_Load(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// Closing application event.
        /// </summary>
        /// <param name="sender">Sender</param>
        /// <param name="e">Event arguments</param>
        private void MainWindow_FormClosing(object sender, FormClosingEventArgs e)
        {
            this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            e.Cancel = true && !closeFromMenu;
        }
    }
}
