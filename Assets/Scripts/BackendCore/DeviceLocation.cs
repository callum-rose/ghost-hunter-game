using System.Collections;
using UnityEngine;
using Utils;
using CustomExtensions;

public class DeviceLocation : MonoBehaviour
{
    [SerializeField] bool doSpoofCoordinates;
    [SerializeField] [Range(0, 1e-6f)] float spoofCoordinateX, spoofCoordinateY;

    bool locationServiceActive;
    const float locationUpdateWait = 1;

    public delegate void LocationEvent(Vector2 locationInfo);
    public static event LocationEvent OnLocationDataIn;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (doSpoofCoordinates || Input.location.isEnabledByUser)
            StartCoroutine(CheckLocationRoutine());
        else
            LogUtil.Write("Location services are not enabled by the user.");
    }

    IEnumerator CheckLocationRoutine()
    {
        if (!doSpoofCoordinates)
            yield return StartCoroutine(InitialiseLocationServiceRoutine());
        else
            LogUtil.Write("Spoofing coordinates");

        while (doSpoofCoordinates || (locationServiceActive && Input.location.isEnabledByUser))
        {
            if (OnLocationDataIn != null)
            {
                float lat, lon;
                if (doSpoofCoordinates)
                {
                    lat = spoofCoordinateY;
                    lon = spoofCoordinateX;
                }
                else
                {
                    lat = Input.location.lastData.latitude * Mathf.PI / 180;
                    lon = Input.location.lastData.longitude * Mathf.PI / 180;
                }

                Vector2 locationXY = MercatorUtil.FromGlobeToXY(lat, lon);

                OnLocationDataIn(locationXY);

                LogUtil.Write("Location data event");
            }
            else
                LogUtil.Write("Nothing subscribed to location data event");

            yield return new WaitForSeconds(locationUpdateWait);
        }

        Input.location.Stop();
        LogUtil.Write("Stopping location service");
    }

    IEnumerator InitialiseLocationServiceRoutine()
    {
        LogUtil.Write("Initialising location service.");

        Input.location.Start();

        long maxWaitSeconds = 20;
        long startSeconds = TimeUtil.Seconds;

        while (Input.location.status == LocationServiceStatus.Initializing
               && TimeUtil.GetSecondsSince(startSeconds) < maxWaitSeconds)
        {
            // wait for initialisation
            yield return new WaitForSeconds(1);
        }

        LogUtil.Write("LocationServiceStatus: " + Input.location.status);
        locationServiceActive = Input.location.status == LocationServiceStatus.Running;
    }

    IEnumerator RequestPermissions()
    {
        yield return null;
    }
}