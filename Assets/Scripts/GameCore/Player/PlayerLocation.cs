using UnityEngine;
using CustomExtensions;

public class PlayerLocation : MonoBehaviour 
{
    [SerializeField] bool doSpoofLocation;
    [SerializeField] Vector2 spoofLocationVector;

    Vector2 Location { get { return transform.position.XZ2XY(); } }

    public delegate void PlayerLocationEvent(Vector2 position);
    public static event PlayerLocationEvent OnPlayerMoved;

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

        if (OnPlayerMoved != null)
            OnPlayerMoved(Location);
    }

    void OnLocationUpdate(LocationInfo locationInfo)
    {
        print(locationInfo.latitude + ", " + locationInfo.longitude);
    }


}
