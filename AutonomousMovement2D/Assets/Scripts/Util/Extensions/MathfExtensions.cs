using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kensai.Util.Extensions {
    public static class MathfExtensions {
        private static float Signed2DTriArea(Vector2 a, Vector2 b, Vector2 c)
        {
            return (a.x - c.x) * (b.y - c.y) - (a.y - c.y) * (b.x - c.x);
        }

        public static bool SegmentIntersection2D(Vector2 a, Vector2 b, Vector2 c, Vector2 d, out float distanceAlongFirstVector, out Vector2 intersectionPoint)
        {
            distanceAlongFirstVector = 0;
            intersectionPoint = Vector2.zero;
            // signs of areas correspond to which side of ab points c and d are
            float a1 = Signed2DTriArea(a,b,d); // Compute winding of abd (+ or -)
            float a2 = Signed2DTriArea(a,b,c); // To intersect, must have sign opposite of a1

            // If c and d are on different sides of ab, areas have different signs
            if( a1 * a2 < 0.0f ) // require unsigned x & y values.
            {
                float a3 = Signed2DTriArea(c,d,a); // Compute winding of cda (+ or -)
                float a4 = a3 + a2 - a1; // Since area is constant a1 - a2 = a3 - a4, or a4 = a3 + a2 - a1

                // Points a and b on different sides of cd if areas have different signs
                if( a3 * a4 < 0.0f )
                {
                    // Segments intersect. Find intersection point along L(t) = a + t * (b - a).
                    distanceAlongFirstVector = a3 / (a3 - a4);
                    intersectionPoint = a + distanceAlongFirstVector * (b - a); // the point of intersection
                    return true;
                }
            }

            // Segments not intersecting or collinear
            return false;
        }

        public static Vector2 LineIntersectionVector2(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2) {
            // Get A,B,C of first line - points : ps1 to pe1
            float A1 = pe1.y - ps1.y;
            float B1 = ps1.x - pe1.x;
            float C1 = A1 * ps1.x + B1 * ps1.y;

            // Get A,B,C of second line - points : ps2 to pe2
            float A2 = pe2.y - ps2.y;
            float B2 = ps2.x - pe2.x;
            float C2 = A2 * ps2.x + B2 * ps2.y;

            // Get delta and check if the lines are parallel
            float delta = A1 * B2 - A2 * B1;
            if (delta == 0)
                throw new System.Exception("Lines are parallel");

            // now return the Vector2 intersection point
            return new Vector2(
                (B2 * C1 - B1 * C2) / delta,
                (A1 * C2 - A2 * C1) / delta
            );
        }

        public static bool LinesIntersect(Vector2 ps1, Vector2 pe1, Vector2 ps2, Vector2 pe2) {
            try {
                LineIntersectionVector2(ps1, pe1, ps2, pe2);
                return true;
            } catch {
                return false;
            }
        }
    }
}
