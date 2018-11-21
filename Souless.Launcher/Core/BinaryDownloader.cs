using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Souless.Launcher.Core
{
    public class BinaryDownloader
    {
        public event DownloadStateChanged OnProgressChanges;
        public delegate void DownloadStateChanged(DownloadProgressChangedEventArgs e);

        public event Complete OnDownloadComplete;
        public delegate void Complete(string path);

        private string tempFile = "temp.zip";

        public string BinaryUrl
        {
            get
            {
                if (VersionChecker.IsOnline)
                {
                    using (var client = new WebClient())
                    using (StreamReader streamReader = new StreamReader(client.OpenRead(VersionChecker.VersionServer + "/binary")))
                        return streamReader.ReadToEnd();
                }
                else
                {
                    return null;
                }
            }
        }

        public void DownloadOrUpdate()
        {
            startDownload();
        }
        private void startDownload()
        {
            Thread thread = new Thread(() =>
            {
                WebClient client = new WebClient();
                client.DownloadProgressChanged += new DownloadProgressChangedEventHandler(client_DownloadProgressChanged);
                client.DownloadFileCompleted += new AsyncCompletedEventHandler(client_DownloadFileCompleted);
                client.DownloadFileAsync(new Uri(BinaryUrl), Environment.CurrentDirectory + @"\" + tempFile);
            });
            thread.Start();
        }
        void client_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
        {
            OnProgressChanges?.Invoke(e);

        }
        void client_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
        {
            OnDownloadComplete?.Invoke(tempFile);
        }
    }
}
