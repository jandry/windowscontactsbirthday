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

    public partial class Form1 : Form
    {

        private ContactManager _contactManager;

        private NotifyIcon mNotify;
        private BackgroundWorker mWorker;

        public Form1()
        {
            InitializeComponent();

            this.ShowInTaskbar = false;
            //this.WindowState = FormWindowState.Minimized;
            this.Visible = false;
            // Setup notification icon
            mNotify = new NotifyIcon();
            mNotify.Visible = true;
            mNotify.Text = "nobugz waz here";
            //mNotify.Icon = Properties.Resources.;
            // Setup context menu
            mNotify.ContextMenu = new ContextMenu();
            mNotify.ContextMenu.MenuItems.Add("Show", showToolStripMenuItem_Click);
            mNotify.ContextMenu.MenuItems.Add("Run", runToolStripMenuItem_Click);
            mNotify.ContextMenu.MenuItems.Add("Exit", exitToolStripMenuItem_Click);
            // Setup background worker
            mWorker = new BackgroundWorker();
            mWorker.DoWork += worker_DoWork;
            mWorker.RunWorkerCompleted += worker_Completed;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Photo.SupportNonLocalUrls = true;
            _contactManager = new ContactManager();
            // _contactManager.CollectionChanged += (sender, e) => _RefreshList();
            foreach(Contact contact in _contactManager.GetContactCollection())
            {
                Name contactNames = contact.Names.Default;
                foreach(DateTime? anniversaryDate in  contact.Dates)
                if (anniversaryDate.HasValue)
                {
                    int age = DateTime.Now.Year - anniversaryDate.Value.Year;
                    String message = "{0} aura {1} le {2}/{3}";
                    notifyIcon.ShowBalloonTip(10000, "Anniversaire", String.Format(message, contactNames.FormattedName, age, anniversaryDate.Value.Day, anniversaryDate.Value.Month), ToolTipIcon.Info);
                    label1.Text = contactNames.FormattedName;
                }
            }
        }

        private void worker_DoWork(object sender, DoWorkEventArgs e)
        {
            // Simulate worker thread taking time
            System.Threading.Thread.Sleep(3000);
        }
        private void worker_Completed(object sender, RunWorkerCompletedEventArgs e)
        {
            // Display balloon tip
            mNotify.BalloonTipText = "Your job iz done";
            mNotify.ShowBalloonTip(10000);
        }
        private void runToolStripMenuItem_Click(object sender, EventArgs e)
        {
            mWorker.RunWorkerAsync();
        }
        private void showToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Visible = true;
            this.WindowState = FormWindowState.Normal;
        }
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Form1_Load(object sender, EventArgs e)
        {

        }
    }
}
