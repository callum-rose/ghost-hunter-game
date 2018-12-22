using UnityEngine;
using System;

namespace CustomExtensions
{

    public static class VectorExtensions
    {

        public static Vector3 SetX(this Vector3 vector, float x)
        {
            return new Vector3(x, vector.y, vector.z);
        }

        public static Vector3 SetY(this Vector3 vector, float y)
        {
            return new Vector3(vector.x, y, vector.z);
        }

        public static Vector2 SetY(this Vector2 vector, float y)
        {
            return new Vector2(vector.x, y);
        }

        public static Vector3 SetZ(this Vector3 vector, float z)
        {
            return new Vector3(vector.x, vector.y, z);
        }

        public static Vector3 AddX(this Vector3 vector, float dx)
        {
            return vector + Vector3.right * dx;
        }

        public static Vector3 AddY(this Vector3 vector, float dy)
        {
            return vector + Vector3.up * dy;
        }

        public static Vector3 AddZ(this Vector3 vector, float dz)
        {
            return vector + Vector3.forward * dz;
        }

        public static Vector3 FlipY(this Vector3 vector)
        {
            return new Vector3(vector.x, -vector.y, vector.z);
        }

        /// <summary>
        /// Converts a Vector2 to a Vector3 by placing it on the XZ plane
        /// </summary>
        public static Vector3 XY2XZ(this Vector2 vector2)
        {
            return new Vector3(vector2.x, 0, vector2.y);
        }

        /// <summary>
        /// Converts a Vector3 to a Vector2 by converting its x and z components into x and y components
        /// </summary>
        public static Vector2 XZ2XY(this Vector3 vector)
        {
            return new Vector2(vector.x, vector.z);
        }

        public static Vector3 SetMagnitude(this Vector3 vector, float magnitude)
        {
            float oldMagnitude = vector.magnitude;
            float magRatio = magnitude / oldMagnitude;
            return vector * magRatio;
        }
    }
}