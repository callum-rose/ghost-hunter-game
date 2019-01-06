using UnityEngine;

[System.Serializable]
public struct Coordinate 
{
    [SerializeField] public float latitude, longitude, altitude;

    public Coordinate (float latitude, float longitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
        this.altitude = 0;
    }

    public Coordinate(float latitude, float longitude, float altitude)
    {
        this.latitude = latitude;
        this.longitude = longitude;
        this.altitude = altitude;
    }
}
