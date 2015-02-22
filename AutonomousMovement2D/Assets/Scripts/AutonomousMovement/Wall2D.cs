using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kensai.AutonomousMovement {
    public struct Wall2D {
        public Vector2 To;
        public Vector2 From;

        public Vector2 Normal {
            get {
                return new Vector2(From.y - To.y, To.x - From.x).normalized;
            }
        }

        public Vector2 InverseNormal {
            get {
                return new Vector2(To.y - From.y, From.x - To.x).normalized;
            }
        }

        public Wall2D(Vector2 from, Vector2 to) {
            From = from;
            To = to;
        }

        public Wall2D(float x1, float y1, float x2, float y2) {
            From = new Vector2(x1, y1);
            To = new Vector2(x2, y2);
        }
    }
}
