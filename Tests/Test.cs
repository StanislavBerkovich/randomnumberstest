using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace RandomNumbers.Tests {
    public abstract class Test {  
        protected Model model;

        public Test(ref Model model) {
            this.model = model;
        }

        abstract public double[] run(bool printResults);
        
        abstract public override string ToString();
    }
}
