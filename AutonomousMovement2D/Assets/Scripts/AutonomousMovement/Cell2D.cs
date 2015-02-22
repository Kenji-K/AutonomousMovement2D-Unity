using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kensai.AutonomousMovement {
    public struct Cell2D {
        public List<SteeringAgent2D> Members { get; set; }
        public Rect Rect { get; set; }
        public Cell2D(Rect Bounds) {
            this.Rect = Bounds;
        }

        public Cell2D(Vector2 topLeft, Vector2 botRight) {
            this.Rect = new Rect(topLeft.x, topLeft.y, Mathf.Abs(botRight.x - topLeft.x), Mathf.Abs(botRight.y - topLeft.y));
            Members = new List<SteeringAgent2D>();
        }
    }
}
