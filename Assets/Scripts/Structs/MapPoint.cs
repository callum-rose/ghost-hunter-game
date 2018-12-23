using UnityEngine;

[System.Serializable]
public struct MapPoint
{
    [SerializeField] Vector3 m_positionXYZ;
    [SerializeField] Color m_colour;

    public Vector3 PositionXYZ 
    { 
        get { return m_positionXYZ; }
        set { m_positionXYZ = value; }
    }

    public Vector2 Position { get { return new Vector2(m_positionXYZ.x, m_positionXYZ.z); } }
    public float Height { get { return m_positionXYZ.y; } }

    public Color Colour
    {
        get { return m_colour; }
        set { m_colour = value; }
    }

    public MapPoint(Vector2 position, float height, Color colour)
    {
        m_positionXYZ = new Vector3(position.x, height, position.y);
        m_colour = colour;
    }

    public override string ToString()
    {
        return string.Format("[x:{0:0.#}, y:{1:0.#}; h:{2:0.#}; c:{3}]", Position.x, Position.y, Height, Colour);
    }
}