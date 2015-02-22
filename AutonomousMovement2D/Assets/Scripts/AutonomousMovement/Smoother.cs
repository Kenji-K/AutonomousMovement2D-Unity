using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MiscUtil;

namespace Kensai.AutonomousMovement {
    internal class Smoother<T> {
        public List<T> history;
        private int nextUpdateSlot = 0;

        public Smoother(int sampleSize) {
            history = new List<T>(sampleSize);
        }

        public T Update(T mostRecentValue) {
            if (history.Count < history.Capacity) {
                history.Add(mostRecentValue);
                nextUpdateSlot++;
            } else { 
                history[nextUpdateSlot++] = mostRecentValue;
            }

            if (nextUpdateSlot >= history.Capacity) nextUpdateSlot = 0;
            var sum = Operator<T>.Zero;

            foreach (var value in history) {
                sum = Operator.Add(sum, value);
            }

            if (Operator.NotEqual(sum, Operator<T>.Zero)) {
                return Operator.DivideAlternative(sum, (float)history.Count);
            } else {
                return sum;
            }
        }
    }
}
