using UnityEngine;

public class HMSingleton<T> : MonoBehaviour where T : MonoBehaviour
{
	private static T _instance;

	private static object _lock = new object();
	private static bool _applicationIsQuitting = false;

	public static T Instance
	{
		get
		{
			if (_applicationIsQuitting)
				return null;

			lock(_lock)
			{
				if (_instance == null)
				{
					_instance = (T) FindObjectOfType(typeof(T));
					if (_instance == null)
					{
						GameObject singleton = new GameObject(typeof(T).ToString());
						_instance = singleton.AddComponent<T>();
						DontDestroyOnLoad(singleton);
					}
				}

				return _instance;
			}
		}
	}
	
	void OnDestroy ()
	{
		_applicationIsQuitting = true;
	}
}