using System.Collections;
using UnityEngine;
using Utils;

public class DeviceLocation : MonoBehaviour
{
    bool locationServiceActive;

    public delegate void LocationEvent(LocationInfo locationInfo);
    public static event LocationEvent OnLocationDataIn;

    void Start()
    {
        Init();
    }

    public void Init()
    {
        if (Input.location.isEnabledByUser)
            StartCoroutine(CheckLocationRoutine());
        else
            LogUtil.Write("Location services are not enabled by the user.");
    }

    IEnumerator CheckLocationRoutine()
    {
        yield return StartCoroutine(InitialiseLocationServiceRoutine());

        while (locationServiceActive && Input.location.isEnabledByUser)
        {
            if (OnLocationDataIn != null)
            {
                // fire data in event
                OnLocationDataIn.Invoke(Input.location.lastData);
                LogUtil.Write("Firing location data event");
            }
            else
                LogUtil.Write("Nothing subscribed to location data event");

            yield return new WaitForSeconds(5);
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