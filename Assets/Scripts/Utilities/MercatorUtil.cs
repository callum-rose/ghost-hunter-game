using UnityEngine;
using CustomExtensions;

namespace Utils
{
    public static class MercatorUtil
    {
        /// <summary>
        /// Projects from globe coordinates to mercator projection XY.
        /// </summary>
        /// <param name="latitude">Latitude in degrees.</param>
        /// <param name="longitude">Longitude in degrees.</param>
        public static Vector2 FromGlobeToXY(float latitude, float longitude)
        {
            // convert to radians
            latitude = latitude * Mathf.PI / 180;
            longitude = longitude * Mathf.PI / 180;

            // in meters
            const float worldRadius = 6367000;
            const float worldCircumference = 2 * Mathf.PI * worldRadius;
            // the reference meridian in radians i.e. Greenwich
            const float longitude0 = 0;
            const float a = Mathf.PI / 4;

            // mercator projection formula
            float x = worldRadius * (longitude - longitude0);
            float y = worldRadius * Mathf.Log(Mathf.Tan(a + latitude / 2));

            return new Vector2(x, y);
        }

        /// <summary>
        /// Projects from globe coordinates to mercator projection XY.
        /// </summary>
        /// <param name="latitude">Latitude in degrees.</param>
        /// <param name="longitude">Longitude in degrees.</param>
        public static Vector2 FromGlobeToXY(Coordinate coordinate)
        {
            return FromGlobeToXY(coordinate.latitude, coordinate.longitude);
        }
    }
}