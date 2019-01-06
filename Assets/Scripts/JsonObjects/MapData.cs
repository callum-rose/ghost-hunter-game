using System.Collections.Generic;
using UnityEngine;

public class MapData : IJsonable {

    public float mapFidelity = 1;
    public List<Coordinate> boundaryCoords;
    public List<MapPoint> mapPointsList;
    public Gradient colourGradient;

    public bool CheckPropertiesValid()
    {
        if (boundaryCoords.Count == 0)
            return false;

        if (mapPointsList.Count == 0)
            return false;

        if (colourGradient == null)
            return false;

        return true;
    }
}