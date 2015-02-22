using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.Util.Debug {
    public class AssertFailedException : Exception {
        public AssertFailedException(string message)
            : base(message) {
        }
    }
}
