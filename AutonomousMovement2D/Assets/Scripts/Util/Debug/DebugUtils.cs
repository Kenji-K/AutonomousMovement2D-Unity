using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace Kensai.Util.Debug {
    public class DebugUtils {
        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition) {
            Assert(condition, string.Empty);
        }

        [Conditional("UNITY_EDITOR")]
        public static void Assert(bool condition, string customMessage) {
            if (!condition) {
                StackFrame callStack = new StackFrame(1, true);
                var message = "Assert failed in: " + callStack.GetFileName() + ", Line: " + callStack.GetFileLineNumber();
                customMessage = customMessage.Trim();
                if (customMessage != string.Empty) { 
                    message += "\r\n" + customMessage;
                }
                var ex = new AssertFailedException(message);
                throw ex;
            }
        }

    }
}
