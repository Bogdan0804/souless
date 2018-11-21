using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Souless.Launcher.Core
{
    public class VersionChecker
    {
        public static readonly string VersionServer = "http://server.soulless.co.za/";

        public bool IsUpdateAvailable { get; internal set; }
        public static bool IsOnline { get; private set; }

        public int InstalledVersion
        {
            get
            {
                if (File.Exists("version"))
                {
                    string versionGameInstalled = File.ReadAllText("version").Split('\n')[0];

                    int n1I = int.Parse(versionGameInstalled.Split('.')[0]);
                    int n2I = int.Parse(versionGameInstalled.Split('.')[1]);
                    int n3I = int.Parse(versionGameInstalled.Split('.')[2]);
                    return n1I * n2I * n3I;
                }
                else
                {
                    return 0;
                }
            }
        }


        public void CheckUpdates()
        {
            this.IsUpdateAvailable = false;
            IsOnline = CheckForInternetConnection();


            if (IsOnline)
            {
                using (var client = new WebClient())
                {
                    using (StreamReader streamReader = new StreamReader(client.OpenRead(VersionServer + "version")))
                    {
                        string raw_version = streamReader.ReadToEnd();

                        string versionGame = raw_version.Split('\n')[0];

                        int n1 = int.Parse(versionGame.Split('.')[0]);
                        int n2 = int.Parse(versionGame.Split('.')[1]);
                        int n3 = int.Parse(versionGame.Split('.')[2]);
                        int vNumb = n1 * n2 * n3;

                        if (vNumb > InstalledVersion)
                        {
                            this.IsUpdateAvailable = true;
                        }

                        if (!File.Exists("version"))
                        {
                            File.WriteAllText("version", raw_version);
                            this.IsUpdateAvailable = true;
                        }
                    }
                }
            }
        }

        private static bool CheckForInternetConnection()
        {
            try
            {
                using (var client = new WebClient())
                using (client.OpenRead("http://clients3.google.com/generate_204"))
                {
                    return true;
                }
            }
            catch
            {
                return false;
            }
        }
    }
}
