using RPG2D.GameEngine;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RPG2D.SGame.Forms
{
    public partial class SettingsForm : Form
    {
        [DllImport("user32.dll")]
        public static extern bool EnumDisplaySettings(
              string deviceName, int modeNum, ref DEVMODE devMode);
        const int ENUM_CURRENT_SETTINGS = -1;

        const int ENUM_REGISTRY_SETTINGS = -2;

        [StructLayout(LayoutKind.Sequential)]
        public struct DEVMODE
        {

            private const int CCHDEVICENAME = 0x20;
            private const int CCHFORMNAME = 0x20;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmDeviceName;
            public short dmSpecVersion;
            public short dmDriverVersion;
            public short dmSize;
            public short dmDriverExtra;
            public int dmFields;
            public int dmPositionX;
            public int dmPositionY;
            public ScreenOrientation dmDisplayOrientation;
            public int dmDisplayFixedOutput;
            public short dmColor;
            public short dmDuplex;
            public short dmYResolution;
            public short dmTTOption;
            public short dmCollate;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 0x20)]
            public string dmFormName;
            public short dmLogPixels;
            public int dmBitsPerPel;
            public int dmPelsWidth;
            public int dmPelsHeight;
            public int dmDisplayFlags;
            public int dmDisplayFrequency;
            public int dmICMMethod;
            public int dmICMIntent;
            public int dmMediaType;
            public int dmDitherType;
            public int dmReserved1;
            public int dmReserved2;
            public int dmPanningWidth;
            public int dmPanningHeight;

        }

        public SettingsForm()
        {
            InitializeComponent();

            this.Load += (e, o) =>
            {
                Location = new Point(GameManager.Game.ThisGame.Window.Position.X, GameManager.Game.ThisGame.Window.Position.Y);
                this.Size = new Size(RPG2D.Properties.Settings.Default.resWidth + 4, RPG2D.Properties.Settings.Default.resHeight + 50);

                Image bmp = Bitmap.FromFile("title.png");

                pictureBox.Image = bmp;
                pictureBox.Width = bmp.Width;
                pictureBox.Height = bmp.Height;
                int titleTexX = (int)GameEngine.GameManager.Game.ScreenSize.X / 2 - pictureBox.Width / 2;
                pictureBox.Location = new Point(titleTexX, 10);
                panelUI.Location = new Point(panelUI.Location.X, pictureBox.Height + 10);
            };
            this.buttonApply.Click += ButtonApply_Click;
            this.buttonBack.Click += ButtonBack_Click;

            PopulateResSelector();

            comboBoxResWidth.Text = RPG2D.Properties.Settings.Default.resWidth.ToString();
            comboBoxResHeight.Text = RPG2D.Properties.Settings.Default.resHeight.ToString();
            textBoxName.Text = RPG2D.Properties.Settings.Default.name;
        }

        private void ButtonBack_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void ButtonApply_Click(object sender, EventArgs e)
        {
            RPG2D.Properties.Settings.Default.resWidth = int.Parse(comboBoxResWidth.Text);
            RPG2D.Properties.Settings.Default.resHeight = int.Parse(comboBoxResHeight.Text);

            RPG2D.Properties.Settings.Default.name = textBoxName.Text;

            RPG2D.Properties.Settings.Default.Save();
            Application.Restart();
        }

        private void PopulateResSelector()
        {
            DEVMODE vDevMode = new DEVMODE();
            int i = 0;
            while (EnumDisplaySettings(null, i, ref vDevMode))
            {
                if (!comboBoxResWidth.Items.Contains(vDevMode.dmPelsWidth))
                {
                    comboBoxResWidth.Items.Add(vDevMode.dmPelsWidth);
                }
                if (!comboBoxResHeight.Items.Contains(vDevMode.dmPelsHeight))
                {
                    comboBoxResHeight.Items.Add(vDevMode.dmPelsHeight);
                }

                i++;
            }
        }
    }
}
