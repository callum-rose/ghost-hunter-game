using UnityEngine;

[System.Serializable]
public struct MapPoint
{
    public Vector3 PositionXYZ { get; private set; }

    public Vector2 Position { get { return new Vector2(PositionXYZ.x, PositionXYZ.z); } }
    public float Height { get { return PositionXYZ.y; } }

    public MapPoint(Vector3 position)
    {
        PositionXYZ = position;
    }

    public MapPoint(Vector2 position, float height)
    {
        PositionXYZ = new Vector3(position.x, height, position.y);
    }

    public override string ToString()
    {
        return string.Format("[x:{0:0.#}, y:{1:0.#}, h:{2:0.#}]", Position.x, Position.y, Height);
    }
}