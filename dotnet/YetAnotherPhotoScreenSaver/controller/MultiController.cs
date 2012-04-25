using System;
using System.Collections.Generic;
using System.Text;

namespace Org.Kuhn.Yapss {
    class MultiController : IMultiController {
        public MultiController(IList<IController> controllers) {
            this.controllers = controllers;
            this.peeks = new Instruction[controllers.Count];
        }
        public MultiControllerInstruction GetInstruction() {
            // this is the weirdest f****** code i've ever written.
            MultiControllerInstruction instruction;
            if (peeks[index] != null) {
                instruction = new MultiControllerInstruction(index, peeks[index]);
                peeks[index] = null;
            }
            else {
                peeks[index] = controllers[index].GetInstruction();
                if (peeks[index].longPause) {
                    index = ++index % controllers.Count;
                    instruction = GetInstruction();
                }
                else {
                    instruction = new MultiControllerInstruction(index, peeks[index]);
                    peeks[index] = null;
                }
            }
            return instruction;
        }
        private IList<IController> controllers;
        private Instruction[] peeks;
        private int index = 0;
    }
}