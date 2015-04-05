using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Kensai.Util.Extensions {
    public static class VectorExtensions {
        public static float Cross(this Vector2 v, Vector2 w) {
            return v.x * w.y - v.y * w.x;
        }

        public static float SimpleMult(this Vector2 v, Vector2 w) {
            return v.x * w.x + v.y * w.y;
        }

        public static Vector2 Truncate(this Vector2 vector, float max) {
            if (vector.sqrMagnitude < max * max) {
                return vector;
            } else {
                return vector.normalized * max;
            }
        }
    }
}
