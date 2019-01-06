using UnityEngine;
using CustomExtensions;
using Utils;
using DG.Tweening;

public class PlayerLocation : MonoBehaviour 
{
    //[SerializeField] bool doSpoofLocation;
    //[SerializeField] Vector2 spoofLocationVector;

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

    void Move(Vector2 position)
    {
        transform.DOMove(position.XY2XZ(), 1f).SetEase(Ease.InOutSine).OnUpdate(() =>
        {
            // fire every frame of the tween
            if (OnPlayerMoved != null)
                OnPlayerMoved(Location);
        });
    }

    void OnLocationUpdate(Vector2 position)
    {
        Move(position);
    }
}
