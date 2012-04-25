using System;
using System.IO;
using System.Text;

namespace Org.Kuhn.Yapss {
    class Log {
        private Log() {

        }

        public static Log Instance {
            get { return instance; }
        }

        public bool IsEnabled {
            get { return enabled; }
            set { enabled = value; }
        }

        public void Write(String message) {
            Write(message, null);
        }

        public void Write(String message, Exception ex) {
            if (enabled) {
                lock (this) {
                    StreamWriter writer = null;
                    try {
                        writer = File.AppendText("yapss.log");
                        writer.Write(DateTime.Now.ToString());
                        writer.Write(" ");
                        writer.WriteLine(message);
                        if (ex != null) {
                            writer.WriteLine(ex.ToString());
                        }
                        writer.Flush();
                    }
                    catch {
                        // ignore
                    }
                    finally {
                        if (writer != null) {
                            writer.Dispose();
                        }
                    }
                }

            }
        }

        private static Log instance = new Log();
        private bool enabled = false;
    }
}
