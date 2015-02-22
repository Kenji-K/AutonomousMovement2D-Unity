using System;
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace Kensai.Util { 
    [Serializable]
    public class IntVector2 : IComparable<IntVector2> {
        public int x;
        public int y;

        public static IntVector2 Zero {
            get {
                return new IntVector2(0, 0);
            }
        }

        public IntVector2(int x, int y) {
            this.x = x;
            this.y = y;
        }

        public override string ToString() {
            return string.Format("({0}, {1})", x, y);
        }

        public Vector2 ToVector2() {
            return new Vector2(x, y);
        }

        public static IntVector2 operator +(IntVector2 a, IntVector2 b) {
            return new IntVector2(a.x + b.x, a.y + b.y);
        }

        public static IntVector2 operator -(IntVector2 a, IntVector2 b) {
            return new IntVector2(a.x - b.x, a.y - b.y);
        }

        public float Magnitude() {
            return Mathf.Sqrt((float) SquaredMagnitude());
        }

        public int SquaredMagnitude() {
            return x * x + y * y;
        }

        public int CompareTo(IntVector2 other) {
            var thisMagnitude = SquaredMagnitude();
            var otherMagnitude = other.SquaredMagnitude();

            if (thisMagnitude > otherMagnitude) return 1;
            else if (thisMagnitude < otherMagnitude) return -1;
            else return 0;
        }

        public IntVector2 Rotate(SquareAngle angle) {
            switch (angle) {
                case SquareAngle.Degrees90:
                case SquareAngle.DegreesMinus270:
                    return new IntVector2(y, -x);
                case SquareAngle.Degrees180:
                case SquareAngle.DegreesMinus180:
                    return new IntVector2(-x, -y);
                case SquareAngle.Degrees270:
                case SquareAngle.DegreesMinus90:
                    return new IntVector2(-y, x);
                default:
                    return this;
            }
        }

        #region Equality comparison implementation
        public override bool Equals(object other) {
            if (!(other is IntVector2)) return false;

            var vector = other as IntVector2;
            return vector.x == this.x && vector.y == this.y;
        }

        public override int GetHashCode() {
            return ShiftAndWrap(x.GetHashCode(), 2) ^ y.GetHashCode();
        }

        private int ShiftAndWrap(int value, int positions) {
            positions = positions & 0x1F;

            // Save the existing bit pattern, but interpret it as an unsigned integer. 
            uint number = BitConverter.ToUInt32(BitConverter.GetBytes(value), 0);
            // Preserve the bits to be discarded. 
            uint wrapped = number >> (32 - positions);
            // Shift and wrap the discarded bits. 
            return BitConverter.ToInt32(BitConverter.GetBytes((number << positions) | wrapped), 0);
        }
        #endregion
    }

    public enum SquareAngle {
        Degrees90, Degrees180, Degrees270,
        DegreesMinus90, DegreesMinus180, DegreesMinus270
    }
}