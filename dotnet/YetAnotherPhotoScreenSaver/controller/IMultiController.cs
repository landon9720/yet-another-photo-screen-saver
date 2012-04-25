using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Kuhn.Yapss {
    interface IMultiController {
        MultiControllerInstruction GetInstruction();
    }
    class MultiControllerInstruction : Instruction {
        public int controllerIndex;
        public MultiControllerInstruction(int controllerIndex, Instruction instruction) {
            this.controllerIndex = controllerIndex;
            this.x = instruction.x;
            this.y = instruction.y;
            this.w = instruction.w;
            this.h = instruction.h;
            this.image = instruction.image;
            this.longPause = instruction.longPause;
        }
    }
}
