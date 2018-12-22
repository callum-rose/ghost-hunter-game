using System.Collections;
using UnityEngine;

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
            Logger.Write("Location services are not enabled by the user.");
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
                Logger.Write("Firing location data event");
            }
            else
                Logger.Write("Nothing subscribed to location data event");

            yield return new WaitForSeconds(5);
        }

        Input.location.Stop();
        Logger.Write("Stopping location service");
    }

    IEnumerator InitialiseLocationServiceRoutine()
    {
        Logger.Write("Initialising location service.");

        Input.location.Start();

        long maxWaitSeconds = 20;
        long startSeconds = TimeManager.Seconds;

        while (Input.location.status == LocationServiceStatus.Initializing
               && TimeManager.GetSecondsSince(startSeconds) < maxWaitSeconds)
        {
            // wait for initialisation
            yield return new WaitForSeconds(1);
        }

        Logger.Write("LocationServiceStatus: " + Input.location.status);
        locationServiceActive = Input.location.status == LocationServiceStatus.Running;
    }

    IEnumerator RequestPermissions()
    {
        yield return null;
    }
}