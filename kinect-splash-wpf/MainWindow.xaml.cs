using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace kinect_splash_wpf
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        int screenWidth;
        int screenHeight;
        bool isClosing = false;

        System.Windows.Threading.DispatcherTimer timer1 = new System.Windows.Threading.DispatcherTimer();
        System.Windows.Threading.DispatcherTimer timer2 = new System.Windows.Threading.DispatcherTimer();

        const string ImageDirJPG = "C:\\HTech\\KinectTmpFile\\UserColorMap.jpg";
        const string ImageDirPNG = "C:\\HTech\\KinectTmpFile\\UserColorMap.png";
        string appPath = System.AppDomain.CurrentDomain.BaseDirectory;

        string ExeDir = String.Empty;
        string SettingsDir = String.Empty;

        // Set Window Position
        [System.Runtime.InteropServices.DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int Y, int cx, int cy, int wFlags);

        // Window Z Order
        const int
        HWND_TOP = 0,
        HWND_BOTTOM = 1,
        HWND_TOPMOST = -1,
        HWND_NOTTOPMOST = -2;

        // Window Position Flags
        const int
        SWP_NOSIZE = 0x0001,
        SWP_NOMOVE = 0x0002,
        SWP_NOZORDER = 0x0004,
        SWP_NOREDRAW = 0x0008,
        SWP_NOACTIVATE = 0x0010,
        SWP_DRAWFRAME = 0x0020,
        SWP_FRAMECHANGED = 0x0020,
        SWP_SHOWWINDOW = 0x0040,
        SWP_HIDEWINDOW = 0x0080,
        SWP_NOCOPYBITS = 0x0100,
        SWP_NOOWNERZORDER = 0x0200,
        SWP_NOREPOSITION = 0x0200,
        SWP_NOSENDCHANGING = 0x0400,
        SWP_DEFERERASE = 0x2000,
        SWP_ASYNCWINDOWPOS = 0x4000;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        static extern IntPtr FindWindow(string strClassName, string strWindowName);

        public MainWindow()
        {
            InitializeComponent();

            this.Loaded += new RoutedEventHandler(AppLoaded);

            screenWidth = Convert.ToInt32(SystemParameters.PrimaryScreenWidth);
            screenHeight= Convert.ToInt32(SystemParameters.PrimaryScreenHeight);

            this.Width = screenWidth;

            SplashHolder.Width = screenWidth;
        }

        private void AppLoaded(object sender, RoutedEventArgs e)
        {
            ExeDir = appPath;
            SettingsDir = ExeDir + "/Splash Resources";

            this.Opacity = 0;

            this.Left = 0;
            this.Top = (screenHeight - this.Height) + (screenHeight * 0.10);

            timer1.Interval = new TimeSpan(0, 0, 0, 0, 20);
            timer2.Interval = new TimeSpan(0, 0, 0, 0, 100);

            timer1.Tick += new EventHandler(timer1_Tick);
            timer2.Tick += new EventHandler(timer2_Tick);

            timer1.Start();
            timer2.Start();

            CheckSettings();
            //throw new NotImplementedException();
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            this.isClosing = true;
            if (this.Opacity > 0)
            {
                e.Cancel = true;
            }
            //base.OnClosing(e);
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (this.isClosing == false && this.Opacity < 1)
                this.Opacity += 0.05;
            else if (this.isClosing == true && this.Opacity > 0)
                this.Opacity -= 0.05;
            else if (this.isClosing == true && this.Opacity <= 0)
                Application.Current.Shutdown();

            //throw new NotImplementedException();
        }

        private void timer2_Tick(object sender, EventArgs e)
        {
            if (this.isClosing == false)
            {
                ShowOnTop();
            }

            //throw new NotImplementedException();
        }

        private void ShowOnTop()
        {
            IntPtr handle = IntPtr.Zero;

            try // Try to keep CMS on top
            {
                handle = FindWindow(null, "kinect_splash_wpf");
                //handle = FindWindow("WindowsForms10.Window.8.app.0.378734a", null);
                SetWindowPos(handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
            }
            catch (Exception e)
            {
                // Do nothing
                Console.WriteLine(e.ToString());
            }

            handle = System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle;
            SetWindowPos(handle, (IntPtr)HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        private void CheckSettings()
        {
            if(!Directory.Exists(SettingsDir))
            {
                Directory.CreateDirectory(SettingsDir);
            }

            try
            {
                BitmapImage splashImg = new BitmapImage();
                splashImg.BeginInit();

                string fileName = string.Empty;
                // Check first if Splash JPG is present
                if (File.Exists(SettingsDir + "/splash.jpg"))
                {
                    fileName = SettingsDir + "/splash.jpg";
                }
                // If Splash JPG is not present, look for PNG
                else if (File.Exists(SettingsDir + "/splash.png"))
                {
                    fileName = SettingsDir + "/splash.png";
                }

                splashImg.UriSource = new Uri(fileName);
                splashImg.EndInit();

                SplashHolder.Source = splashImg;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }
        }
    }
}
