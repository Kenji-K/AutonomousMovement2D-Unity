using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Kensai.Util.Extensions {
    public static class MathfExtensions {
        /// <summary>
        /// Test whether two line segments intersect. If so, calculate the intersection point.
        /// <see cref="http://stackoverflow.com/a/14143738/292237"/>
        /// </summary>
        /// <param name="p">Vector to the start point of p.</param>
        /// <param name="p2">Vector to the end point of p.</param>
        /// <param name="q">Vector to the start point of q.</param>
        /// <param name="q2">Vector to the end point of q.</param>
        /// <param name="intersection">The point of intersection, if any.</param>
        /// <param name="considerOverlapAsIntersect">Do we consider overlapping lines as intersecting?
        /// </param>
        /// <returns>True if an intersection point was found.</returns>
        public static bool LineSegementsIntersect(Vector2 p, Vector2 p2, Vector2 q, Vector2 q2,
            out Vector2 intersection, bool considerCollinearOverlapAsIntersect = false) {
            intersection = new Vector2();

            var r = p2 - p;
            var s = q2 - q;
            var rxs = r.Cross(s);
            var qpxr = (q - p).Cross(r);

            // If r x s = 0 and (q - p) x r = 0, then the two lines are collinear.
            if (Mathf.Approximately(rxs, 0) && Mathf.Approximately(qpxr, 0)) {
                // 1. If either  0 <= (q - p) * r <= r * r or 0 <= (p - q) * s <= * s
                // then the two lines are overlapping,
                if (considerCollinearOverlapAsIntersect)
                    if ((0 <= (q - p).SimpleMult(r) && (q - p).SimpleMult(r) <= r.SimpleMult(r)) || (0 <= (p - q).SimpleMult(s) && (p - q).SimpleMult(s) <= s.SimpleMult(s)))
                        return true;

                // 2. If neither 0 <= (q - p) * r = r * r nor 0 <= (p - q) * s <= s * s
                // then the two lines are collinear but disjoint.
                // No need to implement this expression, as it follows from the expression above.
                return false;
            }

            // 3. If r x s = 0 and (q - p) x r != 0, then the two lines are parallel and non-intersecting.
            if (Mathf.Approximately(rxs, 0) && !Mathf.Approximately(qpxr, 0))
                return false;

            // t = (q - p) x s / (r x s)
            var t = (q - p).Cross(s) / rxs;

            // u = (q - p) x r / (r x s)

            var u = (q - p).Cross(r) / rxs;

            // 4. If r x s != 0 and 0 <= t <= 1 and 0 <= u <= 1
            // the two line segments meet at the point p + t r = q + u s.
            if (!Mathf.Approximately(rxs, 0) && (0 <= t && t <= 1) && (0 <= u && u <= 1)) {
                // We can calculate the intersection point using either t or u.
                intersection = p + t * r;

                // An intersection was found.
                return true;
            }

            // 5. Otherwise, the two line segments are not parallel but do not intersect.
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
