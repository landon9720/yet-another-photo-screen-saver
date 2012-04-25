using System;
using System.Collections.Generic;
using System.Threading;
using System.Text;

namespace Org.Kuhn.Yapss {
    class AsyncMultiController : IMultiController, IDisposable {
        public AsyncMultiController(IMultiController controller, int queueSize) {
            this.controller = controller;
            this.evt = new AutoResetEvent(false);
            this.queue = new Queue<MultiControllerInstruction>();
            this.queueSize = queueSize;
            thread = new Thread(ThreadProc);
            thread.IsBackground = true;
            thread.Start();
        }
        public MultiControllerInstruction GetInstruction() {
            while (queue.Count == 0)
                evt.WaitOne();
            MultiControllerInstruction instruction;
            lock (queue) {
                instruction = queue.Dequeue();
            }
            evt.Set();
            return instruction;
        }
        public void Dispose() {
            thread.Abort();
        }
        private void ThreadProc() {
            Log.Instance.Write("Controller thread started");

            while (true) {
                try {
                    while (queue.Count < queueSize) {
                        MultiControllerInstruction instruction = controller.GetInstruction();
                        lock (queue) {
                            queue.Enqueue(instruction);
                        }
                        evt.Set();
                        Thread.Sleep(0);
                    }
                    evt.WaitOne();
                }
                catch (ThreadAbortException) {
                    // ignore
                }
                catch (Exception ex) {
                    Log.Instance.Write("Exception on controller thread", ex);
                }
            }
        }
        private IMultiController controller;
        private Thread thread;
        private AutoResetEvent evt;
        private Queue<MultiControllerInstruction> queue;
        private int queueSize;
    }
}
