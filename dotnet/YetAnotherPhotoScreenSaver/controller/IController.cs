using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;

namespace Org.Kuhn.Yapss {
    interface IController {
        Instruction GetInstruction();
    }
    class Instruction : IDisposable {
        public Image image;
        public int x;
        public int y;
        public int w;
        public int h;
        public bool longPause;

        public void Dispose() {
            image.Dispose();
        }
    }
}