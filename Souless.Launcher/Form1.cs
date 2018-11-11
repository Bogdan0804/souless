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
            dockPanel.BackColor = Color.FromArgb(76, 63,62);
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
            if (Directory.Exists("bin"))
            {
                DownloadGame();

                playButton.Enabled = false;
                Process p = new Process();
                p.StartInfo.WorkingDirectory = Environment.CurrentDirectory + @"\bin\SideScroller\bin\Release";
                p.StartInfo.FileName = Environment.CurrentDirectory + @"\bin\SideScroller\bin\Release\Souless.exe";
                p.Start();
                Application.Exit();
            }
            else
            {
                Directory.CreateDirectory("bin");

                playButton.Enabled = false;
                DownloadGame();
            }
        }

        private void DownloadGame()
        {
            progressBarDownlaod.Visible = true;
            Thread thread = new Thread(() =>
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += Client_DownloadProgressChanged;
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri("https://github.com/Bogdan0804/souless/raw/master/game.zip"), "temp.zip");
            });
            thread.Start();
        }

        private void Client_DownloadProgressChanged(object sender, System.Net.DownloadProgressChangedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                double bytesIn = double.Parse(e.BytesReceived.ToString());
                double totalBytes = double.Parse(e.TotalBytesToReceive.ToString());
                double percentage = bytesIn / totalBytes * 100;
                statusLabel.Text = "Downloaded " + (e.BytesReceived / 2048) + " of " + (e.TotalBytesToReceive / 2048);
                progressBarDownlaod.Value = int.Parse(Math.Truncate(percentage).ToString());
            });
        }

        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            this.BeginInvoke((MethodInvoker)delegate
            {
                statusLabel.Text = "Extracting...";
                ZipFile.ExtractToDirectory("temp.zip", "bin");
                statusLabel.Text = "Done!";
                playButton.Enabled = true;
                File.Delete("temp.zip");
            });
        }
    }
}
