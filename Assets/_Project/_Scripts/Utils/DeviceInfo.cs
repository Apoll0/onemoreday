using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;

#if UNITY_IOS
using UnityEngine.iOS;
#endif

public class DeviceInfo : MonoBehaviour
{
#if !UNITY_EDITOR && UNITY_IPHONE
	[DllImport ("__Internal")] private static extern float _screenScaleFactor();
	[DllImport ("__Internal")] private static extern bool _isTablet();
	[DllImport ("__Internal")] private static extern float _openAppSettings();
#endif
	
	public static Vector2 screenSize => new Vector2(Screen.width, Screen.height);
	
	public static float screenScaleFactor
	{
		get
		{
#if UNITY_EDITOR
			if (Mathf.RoundToInt(100.0f * Screen.height / Screen.width) == Mathf.RoundToInt(400.0f / 3))//iPad
				return (float)Screen.height / 1024;
			if (Screen.width == 1080)//iPhone 8 Plus
				return 2.6f;
			return Screen.width < 640 ? 1.0f : Screen.width >= 960 ? 3.0f : 2.0f;
#elif UNITY_IPHONE
			return _screenScaleFactor();
#elif UNITY_ANDROID
			return density;
#else
			return 1.0f;
#endif
		}
	}

	public static bool isTablet
	{
		get
		{
#if UNITY_EDITOR
			return Mathf.RoundToInt(100.0f * Screen.height / Screen.width) == Mathf.RoundToInt(400.0f / 3); //iPad
#elif UNITY_IPHONE
			return _isTablet();
#elif UNITY_ANDROID
			return isLargeDiagonal;
#else
			return false;
#endif
		}
	}

	public static Rect screenSafeRect
	{
		get
		{
/*#if UNITY_EDITOR
			if ((float)Screen.height / Screen.width > 2.0f)
				return new Rect(new Vector2(0.0f, screenScaleFactor * 34.0f / Screen.height),
					new Vector2(1.0f, (Screen.safeArea.height - screenScaleFactor * 81.0f) / Screen.height));
#endif*/
			return new Rect(new Vector2(Screen.safeArea.x / Screen.width, Screen.safeArea.y / Screen.height),
				new Vector2(Screen.safeArea.width / Screen.width, Screen.safeArea.height / Screen.height));
		}
	}

	private static float _swipeOffsetThreshold;
	public static float swipeOffsetThreshold
	{
		get
		{
			if (_swipeOffsetThreshold <= 0.0f)
				_swipeOffsetThreshold = (dpi > 100.0f && dpi < 1000.0f) ? 0.16f * dpi : 0.03f * Screen.height;
			return _swipeOffsetThreshold;
		}
	}
	
#if !UNITY_EDITOR && UNITY_ANDROID
	private static float _density = -1.0f;
	private static float density
	{
		get
		{
			if (_density >= 0.0f)
				return _density;
			
			_density = 1.0f;
			
			using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"), metricsClass = new AndroidJavaClass("android.util.DisplayMetrics"))
			{
				using (
					AndroidJavaObject metricsInstance = new AndroidJavaObject("android.util.DisplayMetrics"),
					activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"),
					windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager"),
					displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay"))
				{
					displayInstance.Call("getMetrics", metricsInstance);
					_density = metricsInstance.Get<float>("density");
				}
			}
			return _density;
		}
	}

	private static int _isLargeDiagonal = -1;
	private static bool isLargeDiagonal
	{
		get
		{
			if (_isLargeDiagonal >= 0)
				return _isLargeDiagonal > 0;

			_isLargeDiagonal = 0;
			
			using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"), metricsClass = new AndroidJavaClass("android.util.DisplayMetrics"))
			{
				using (
					AndroidJavaObject metricsInstance = new AndroidJavaObject("android.util.DisplayMetrics"),
					activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"),
					windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager"),
					displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay"))
				{
					displayInstance.Call("getMetrics", metricsInstance);
					float dpiX = metricsInstance.Get<float>("xdpi");
					float dpiY = metricsInstance.Get<float>("ydpi");
					float xInches = Screen.width/dpiX;
					float yInches = Screen.height/dpiY;
					float diagonalInches = Mathf.Sqrt(xInches * xInches + yInches * yInches);
					_isLargeDiagonal = diagonalInches >= 6.5f ? 1 : 0;
				}
			}
			return _isLargeDiagonal > 0;
		}
	}
#endif
	
	
	private static float _dpi = -1.0f;
	private static float dpi
	{
		get
		{
			if (_dpi >= 0.0f)
				return _dpi;
			
			_dpi = 0.0f;
			
#if !UNITY_EDITOR && UNITY_IPHONE
			_dpi = Screen.dpi;
#endif

#if !UNITY_EDITOR && UNITY_ANDROID
			using (AndroidJavaClass unityPlayerClass = new AndroidJavaClass("com.unity3d.player.UnityPlayer"), metricsClass = new AndroidJavaClass("android.util.DisplayMetrics"))
			{
				using (
					AndroidJavaObject metricsInstance = new AndroidJavaObject("android.util.DisplayMetrics"),
					activityInstance = unityPlayerClass.GetStatic<AndroidJavaObject>("currentActivity"),
					windowManagerInstance = activityInstance.Call<AndroidJavaObject>("getWindowManager"),
					displayInstance = windowManagerInstance.Call<AndroidJavaObject>("getDefaultDisplay"))
				{
					displayInstance.Call("getMetrics", metricsInstance);
					float dpiX = metricsInstance.Get<float>("xdpi");
					float dpiY = metricsInstance.Get<float>("ydpi");
					_dpi = (dpiX + dpiY) * 0.5f;
				}
			}
#endif
			return _dpi;
		}
	}
	
	public static void OpenAppSettings()
	{
#if !UNITY_EDITOR && UNITY_IPHONE
		_openAppSettings();
#endif
	}
}
