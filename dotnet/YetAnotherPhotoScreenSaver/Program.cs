using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Windows.Forms;
using System.Drawing;
using System.Diagnostics;
using System.Threading;
using System.Runtime.InteropServices;

namespace Org.Kuhn.Yapss {
    class Program {
        [STAThread()]
        static void Main(string[] args) {
            try {
                foreach (Process process in Process.GetProcesses())
                    if (process.Id != Process.GetCurrentProcess().Id && process.ProcessName.Equals("YetAnotherPhotoScreenSaver"))
                        return;
                if (args.Length > 0)
                    if (args[0].ToLower().Contains("/p"))
                        return;
                    else if (args[0].ToLower().Contains("/c")) {
                        Application.Run(new ConfigWindow());
                        return;
                    }

                Config config = new Config();
                Program program = new Program(config);
                program.End += (obj, e) => {
                    Log.Instance.Write("Stopping screen saver");
                    program.Stop();
                    Application.Exit();
                };
                program.Run();
                Application.Run();
            }
            catch (Exception ex) {
                Log.Instance.Write("Unhandled exception on main thread", ex);
            }
            finally {
                ShowTaskbar(); // just in case of abnormal termination
            }
        }

        public Program(Config config) {
            this.config = config;
        }

        public void Run() {
            HideTaskbar();
            Log.Instance.IsEnabled = config.IsLoggingEnabled;
            Log.Instance.Write("Starting screen saver");

            Theme theme = config.Theme == Theme.Random ?
                (DateTime.Now.Second % 2 == 0 ? Theme.Dark : Theme.Light)
                : config.Theme;

            List<IImageSource> imageSources = new List<IImageSource>();
            if (config.IsEnabledFileImageSource)
                imageSources.Add(new FileImageSource(config.FileImageSourcePath));
            if (config.IsEnabledFlickrImageSource)
                imageSources.Add(new FlickrImageSource(config.FlickrImageSourceTags, config.IsFlickrImageSourceTagAndLogic, config.FlickrImageSourceUserName, config.FlickrImageSourceText, theme == Theme.Dark));
            IImageSource imageSource = new RoundRobinImageSource(imageSources, new ColorSquareImageSource());

            // base x size on width of primary screen
            int xSize = Screen.PrimaryScreen.Bounds.Width / config.XCount;

            // build window for each screen
            foreach (Screen screen in Screen.AllScreens) {
                Window wnd = new Window(screen.Bounds, xSize, theme, imageSource);
                windows.Add(wnd);
                wnd.End += DisplayWindowEndEventHandler;
                wnd.Show();
            }

            // start the background drawing thread
            thread = new Thread(ThreadProc);
            thread.IsBackground = true;
            thread.Start();
        }

        public void Stop() {
            ShowTaskbar();
            thread.Abort();
            foreach (Window wnd in windows)
                wnd.Close();
        }

        private Config config;
        private IList<Window> windows = new List<Window>();
        private Thread thread;

        private void ThreadProc() {
            Log.Instance.Write("Drawing thread started");

            // build the multicontroller
            IList<IController> controllers = new List<IController>();
            for (int i = 0; i < windows.Count; ++i) {
                controllers.Add(windows[i].Controller);
            }

            // begin the controller loop, aborted by thread termination only
            using (AsyncMultiController controller = new AsyncMultiController(new MultiController(controllers), 20)) {
                while (true) {
                    try {
                        using (MultiControllerInstruction instruction = controller.GetInstruction()) {
                            Thread.Sleep(instruction.longPause ? config.LongInterval : config.ShortInterval);
                            windows[instruction.controllerIndex].Draw(instruction);
                        }
                    }
                    catch (ThreadAbortException) {
                        // ignore
                    }
                    catch (Exception ex) {
                        Log.Instance.Write("Exception on drawing thread", ex);
                    }
                }
            }
        }

        private void DisplayWindowEndEventHandler(object sender, EventArgs args) {
            End(this, EventArgs.Empty);
        }

        public event EventHandler End;

        private const int SW_HIDE = 0;
        private const int SW_SHOW = 1;
        private const string TASKBAR_WINDOW = "Shell_TrayWnd";

        [DllImport("user32.dll")]
        private static extern int FindWindow(string className, string windowText);
        
        [DllImport("user32.dll")]
        private static extern int ShowWindow(int hwnd, int command);

        private static void HideTaskbar() {
            try {
                ShowWindow(FindWindow(TASKBAR_WINDOW, ""), SW_HIDE);
            }
            catch (Exception ex) {
                Log.Instance.Write("Failed hiding taskbar", ex);
            }
        }

        private static void ShowTaskbar() {
            try {
                ShowWindow(FindWindow(TASKBAR_WINDOW, ""), SW_SHOW);
            }
            catch (Exception ex) {
                Log.Instance.Write("Failed showing taskbar", ex);
            }
        }
    }
}