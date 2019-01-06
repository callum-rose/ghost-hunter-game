using UnityEngine;

public class TerrainMeshData : IJsonable {

    public Vector3[] vertices;
    public int[] triangles;
    public Vector3[] normals;
    public Color[] colours;

    public bool CheckPropertiesValid()
    {
        if (vertices.Length == 0)
            return false;

        if (triangles.Length == 0)
            return false;

        if (normals.Length == 0)
            return false;

        if (colours.Length == 0)
            return false;

        return true;
    }
}
