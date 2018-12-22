using UnityEngine;
using CustomExtensions;

public class PlayerLocation : MonoBehaviour 
{
    [SerializeField] bool doSpoofLocation;
    [SerializeField] Vector2 spoofLocationVector;

    public Vector2 Location { get { return transform.position.XZ2XY(); } }

    void OnEnable()
    {
        DeviceLocation.OnLocationDataIn += OnLocationUpdate;
    }

    void OnDisable()
    {
        DeviceLocation.OnLocationDataIn -= OnLocationUpdate;
    }

    void OnDestroy()
    {
        DeviceLocation.OnLocationDataIn -= OnLocationUpdate;
    }

    void Update()
    {
        if (doSpoofLocation)
        {
            Move(new Vector3(spoofLocationVector.x, 0, spoofLocationVector.y));
        }
    }

    void Move(Vector3 position)
    {
        transform.position = position;
    }

    void OnLocationUpdate(LocationInfo locationInfo)
    {
        print(locationInfo.latitude + ", " + locationInfo.longitude);
    }


}
