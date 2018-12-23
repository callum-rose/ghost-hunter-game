using UnityEngine;
using Utils;

public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
{
	static T _instance;
	static object _lock = new object();

    static bool isBeingDestroyed;

	public static T Instance
	{
		get
		{
			lock(_lock)
			{
                if (isBeingDestroyed)
                {
                    LogUtil.WriteWarning("[Singleton] Attempted to access singleton");
                    return null;
                }
				if (_instance == null)
				{
					_instance = (T) FindObjectOfType(typeof(T));

					if ( FindObjectsOfType(typeof(T)).Length > 1 )
					{
						LogUtil.WriteError("[Singleton] Something went really wrong " +
							" - there should never be more than 1 singleton!" +
							" Reopening the scene might fix it.");
						return _instance;
					}

					if (_instance == null)
					{
						GameObject singleton = new GameObject();
						_instance = singleton.AddComponent<T>();
						singleton.name = "(singleton) "+ typeof(T).ToString();

						DontDestroyOnLoad(singleton);

						LogUtil.Write("[Singleton] An instance of " + typeof(T) + 
							" is needed in the scene, so '" + singleton +
							"' was created with DontDestroyOnLoad.");
					}
				}

				return _instance;
			}
		}
	}

	//private static bool applicationIsQuitting = false;
	/// <summary>
	/// When Unity quits, it destroys objects in a random order.
	/// In principle, a Singleton is only destroyed when application quits.
	/// If any script calls Instance after it have been destroyed, 
	///   it will create a buggy ghost object that will stay on the Editor scene
	///   even after stopping playing the Application. Really bad!
	/// So, this was made to be sure we're not creating that buggy ghost object.
	/// </summary>
    protected virtual void OnDestroy () 
	{
		isBeingDestroyed = true;
		_instance = null;
	}
}