using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using CustomExtensions;

public class TerrainMeshGenerator : Singleton<TerrainMeshGenerator>, IInitialisable
{   
    void Start()
    {
        Init();
    }

    public void Init()
    {
        Generate(Map.Instance.MapPointsArr);
    }

    List<MapPointLink> linksList;

    public Mesh Generate(MapPoint[] mapData)
    {
        Mesh output = new Mesh();

        float mapWorldWidth = mapData.Max(m => m.Position.x) - mapData.Min(m => m.Position.x);
        float mapWorldHeight = mapData.Max(m => m.Position.y) - mapData.Min(m => m.Position.y);

        // split map data into groups of y i.e. rows
        MapPoint[][] mapPointsByRow = mapData.GroupBy(m => m.Position.y).Select(g => g.ToArray()).ToArray();

        linksList = GetLinks(mapPointsByRow);

        return output;
    }

    void OnDrawGizmos()
    {
        if (Application.isPlaying)
        {
            foreach (var l in linksList)
            {
                Gizmos.DrawLine(l.Point0.PositionXYZ, l.Point1.PositionXYZ);
            }
        }
    }

    List<MapPointLink> GetLinks(MapPoint[][] mapData)
    {
        List<MapPointLink> tempList = new List<MapPointLink>();

        // loop through rows
        for (int r = 0; r < mapData.Length - 1; r++)
        {
            MapPoint[] rowCurrent = mapData[r];
            MapPoint[] rowAbove = mapData[r + 1];

            // loop through points in row
            for (int p = 0; p < rowCurrent.Length; p++)
            {
                MapPoint pointCurrent = rowCurrent[p];

                MapPoint? pointLeftOfCurrent = null;
                if (rowCurrent.HasIndex(p - 1))
                    pointLeftOfCurrent = rowCurrent[p - 1];

                MapPoint? pointRightOfCurrent = null;
                if (rowCurrent.HasIndex(p + 1))
                    pointRightOfCurrent = rowCurrent[p + 1];

                // loop through points in row above
                for (int q = 0; q < rowAbove.Length; q++)
                {
                    MapPoint pointAboveCurrent = rowAbove[q];

                    MapPoint? pointAboveLeftOfCurrent = null;
                    if (rowAbove.HasIndex(q - 1))
                        pointAboveLeftOfCurrent = rowAbove[q - 1];

                    MapPoint? pointAboveRightOfCurrent = null;
                    if (rowAbove.HasIndex(q + 1))
                        pointAboveRightOfCurrent = rowAbove[q + 1];

                    // check if x values are the same
                    if (pointCurrent.Position.x.RoughlyEquals(pointAboveCurrent.Position.x, 2))
                    {
                        // There are the only 2 cases where a left leaning link will happen
                        if (pointAboveLeftOfCurrent != null)
                        {
                            if (pointLeftOfCurrent == null)
                            {
                                // Left leaning link: Case 1
                                // o-o 
                                //  \| 
                                // x c 
                                MapPointLink linkLeft = new MapPointLink(pointCurrent, pointAboveLeftOfCurrent.Value);
                                tempList.Add(linkLeft);
                            }
                        }

                        // create link
                        MapPointLink link = new MapPointLink(pointCurrent, pointAboveCurrent);
                        tempList.Add(link);

                        if (pointAboveRightOfCurrent != null)
                        {
                            MapPointLink linkRight = new MapPointLink(pointCurrent, pointAboveRightOfCurrent.Value);
                            tempList.Add(linkRight);
                        }
                    }
                    else if (pointAboveRightOfCurrent != null)
                    {
                        if (pointRightOfCurrent != null)
                        {
                            if (pointRightOfCurrent.Value.Position.x.RoughlyEquals(pointAboveRightOfCurrent.Value.Position.x, 2))
                            {
                                // in this case no point exists above the current point but does to the above right
                                //
                                // x o
                                //  /|
                                // c-o

                                MapPointLink linkRight = new MapPointLink(pointCurrent, pointAboveRightOfCurrent.Value);
                                tempList.Add(linkRight);
                            }
                        }
                    }
                    else if (pointAboveLeftOfCurrent != null)
                    {
                        if (pointLeftOfCurrent != null)
                        {
                            if (pointLeftOfCurrent.Value.Position.x.RoughlyEquals(pointAboveLeftOfCurrent.Value.Position.x, 2))
                            {
                                // Left leaning link: Case 2
                                // o x
                                // |\
                                // o-c 

                                MapPointLink linkRight = new MapPointLink(pointCurrent, pointAboveRightOfCurrent.Value);
                                tempList.Add(linkRight);
                            }
                        }
                    }
                }
            }
        }

        return tempList;
    }

    public struct MapPointLink
    {
        public MapPoint Point0 { get; private set; }
        public MapPoint Point1 { get; private set; }

        public MapPointLink(MapPoint point0, MapPoint point1)
        {
            Point0 = point0;
            Point1 = point1;
        }
    }
}