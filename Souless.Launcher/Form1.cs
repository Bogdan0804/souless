using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

using System.Windows.Forms;
using System.IO;
using System.Diagnostics;
using System.Deployment.Application;
using System.Threading;
using System.Net;
using System.IO.Compression;
using Souless.Launcher.Core;

namespace Souless.Launcher
{
    public partial class Form1 : Form
    {
        [DllImportAttribute("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, int Msg, int wParam, int LPAR);
        [DllImportAttribute("user32.dll")]
        public static extern bool ReleaseCapture();

        const int WM_NCLBUTTONDOWN = 0xA1;
        const int HT_CAPTION = 0x2;  //this indicates that the action takes place on the title bar

        VersionChecker VersionChecker = new VersionChecker();
        BinaryDownloader BinaryDownloader = new BinaryDownloader();

        public Form1()
        {
            InitializeComponent();
            this.topbar.MouseDown += new MouseEventHandler(move_window);
            this.titleLabel.MouseDown += new MouseEventHandler(move_window);

            this.closeButton.Click += CloseButton_Click;
            this.Load += Form1_Load;
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            dockPanel.BackColor = Color.FromArgb(76, 63, 62);
            topbar.BackColor = Color.FromArgb(27, 17, 23);

            playButton.ForeColor = Color.White;
            playButton.BackColor = Color.FromArgb(27, 17, 23);
        }

        private void CloseButton_Click(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void move_window(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Left)
            {
                ReleaseCapture();
                SendMessage(this.Handle, WM_NCLBUTTONDOWN, HT_CAPTION, 0);
            }
        }

        private void playButton_Click(object sender, EventArgs e)
        {
            statusLabel.Text = "Checking updates...";
            VersionChecker.CheckUpdates();
            if (VersionChecker.IsOnline)
            {
                statusLabel.Text = "Downloading...";
                BinaryDownloader.OnProgressChanges += BinaryDownloader_OnProgressChanges;
                BinaryDownloader.OnDownloadComplete += BinaryDownloader_OnDownloadComplete;

                BinaryDownloader.DownloadOrUpdate();
            }
        }

        private void BinaryDownloader_OnProgressChanges(System.Net.DownloadProgressChangedEventArgs e)
        {

            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                statusLabel.Text = "Downloaded " + e.BytesReceived + " of " + e.TotalBytesToReceive;
                if ((Math.Truncate(percentage) > progressBarDownlaod.Minimum && (Math.Truncate(percentage) < progressBarDownlaod.Maximum)))
                    progressBarDownlaod.Value = int.Parse(Math.Truncate(percentage).ToString());
            });

        }

        private void BinaryDownloader_OnDownloadComplete(string path)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                statusLabel.Text = "Extracting...";
                ZipFile.ExtractToDirectory(path, "bin");
                statusLabel.Text = "Done!";
                playButton.Enabled = true;
                File.Delete("temp.zip");
            });
        }


    }
}
