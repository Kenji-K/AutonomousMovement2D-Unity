using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.Util.Extensions {
    public static class VectorExtensions {
        public static Vector2 Truncate(this Vector2 vector, float max) {
            if (vector.sqrMagnitude < max * max) {
                return vector;
            } else {
                return vector.normalized * max;
            }
        }
    }
}
