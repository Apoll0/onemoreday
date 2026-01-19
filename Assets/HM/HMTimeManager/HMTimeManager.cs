using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

/*

Use method "OnTimeManagerReady() {}" (or any other in your scripts as callbacks)
TimeManager.Instance.OnTimeManagerUpdated += OnTimeManagerReady;
TimeManager.Instance.Initialize();

*/

public class HMTimeManager : HMSingleton <HMTimeManager>
{
	#region Events
	
	public static event Action OnTimeManagerUpdated;

	#endregion

	#region Constatns
	
#if UNITY_IOS
	private const string NTP_LIST = "time.apple.com time.euro.apple.com time.asia.apple.com time.windows.com";
#else
	private const string NTP_LIST = "time.google.com time1.google.com time2.google.com time3.google.com time4.google.com time.windows.com";
#endif
		
	private const int SERVER_TIMEOUT = 200; //milliseconds
	
	private const string kRealNtpServerTime = "totm.tm.kdt";
	private const string kDeviceTimestamp 	= "totm.tm.kts";

	#endregion
	
	private bool _initialized;
	private bool _updated;
	private bool _updating;

	private int _serverTimestamp;
	private int _deviceTimestamp;
	private int  _serverTimeDelta;
	
	private readonly List<string> _delayedResetKeys = new ();

	#region Properties
	
	public static DateTime LocalTime => DateTime.Now.AddSeconds(Instance._serverTimeDelta);
	public static DateTime UtcNow => DateTime.UtcNow.AddSeconds(Instance._serverTimeDelta);
	public static int UnixUtcNow => DateTimeToUnix(UtcNow); // Returns seconds elapsed since 1970.1.1
	public static int UnixLocalNow => DateTimeToUnix(LocalTime); // Returns local seconds elapsed since 1970.1.1
	public static int Days => DateTimeToUnix(LocalTime) / oneDay; // Returns local days elapsed since 1970.1.1
	
	public static DateTime DateTimeFromUnix(int seconds) { return new DateTime(1970, 1, 1).AddSeconds(seconds); }
	public static int DateTimeToUnix(DateTime date) { return (int)date.Subtract(new DateTime(1970, 1, 1)).TotalSeconds; }
	
	public static int GmtTimeOffset => (int)TimeZoneInfo.Local.GetUtcOffset(DateTime.Now).TotalSeconds;
	
	#endregion

	public const int oneMinute = 60;
	public const int oneHour = 3600;
	public const int oneDay = 86400;
	public const int oneWeek = 604800;

	public static void SeparateTimeToHMS(int unixTime, out int w, out int d, out int h, out int m, out int s)
	{
		w = unixTime / oneWeek;
		unixTime -= w * oneWeek;
		d = unixTime / oneDay;
		unixTime -= d * oneDay;
		h = unixTime / oneHour;
		unixTime -= h * oneHour;
		m = unixTime / oneMinute;
		unixTime -= m * oneMinute;
		s = unixTime;
	}

	#region Static public methods

	public static string TimeToStringShort(int unixTime, bool hoursOnly = false)
	{
		SeparateTimeToHMS(unixTime, out int w, out int d, out int h, out int m, out int s);

		if (hoursOnly)
		{
			h += w * 7*24 + d * 24;
		}
		
		return h > 0 ? (h.ToString("00") + ":" + m.ToString("00")/* + ":" + s.ToString("00")*/) : (m.ToString("00") + ":" + s.ToString("00"));
	}

	public static string TimeToString(int unixTime)
	{
		SeparateTimeToHMS(unixTime, out int w, out int d, out int h, out int m, out int s);
		return w + " W, " + d + " D, " + h + " H, " + m + " Min, " + s + " Sec";
	}
	
	public static void SeparateTimeToShortLocalizedString(int unixTime, out string localizableKey, out object[] parameters)
	{
		SeparateTimeToHMS(unixTime, out int w, out int d, out int h, out int m, out int s);
		if (w > 0)
		{
			localizableKey = "str.time.short.w";
			parameters = new object[]{ w, d };
		}
		else if (d > 0)
		{
			localizableKey = "str.time.short.d";
			parameters = new object[]{ d, h };
		}
		else
		{
			localizableKey = "str.time.next.hms";
			parameters = new object[]{ h.ToString("00"), m.ToString("00"), s.ToString("00")};
		}
	}

	public static string GetToShortTimeNotation(int unixTime)
	{
		SeparateTimeToHMS(unixTime, out int w, out int d, out int h, out int m, out int s);
		if (w > 0)
			return $"{w}w {d}d";

		if (d > 0)
			return $"{d}d {h}h";

		if (h > 0)
			return $"{h}h {m}m";

		if (m > 0)
			return $"{m}m {s}s";

		return $"{s}s";
	}

	#endregion

	private void OnApplicationFocus(bool hasFocus)
	{
		if (_initialized && hasFocus)
			UpdateServerTimeStamp();
	}

	#region Public methods

	/// <summary>
	/// Initialize HMTimeManager as soon as possible
	/// </summary>
	public void Initialize()
	{
		if (_initialized)
			return;
		
		DebugLog("Initialize");
		DebugLog( "Now: " + UnixUtcNow + ", gmtTimeOffset: " + GmtTimeOffset);
		
		_initialized = true;
		UpdateServerTimeStamp();
	}

	/// <summary>
	/// Returns time manager status
	/// </summary>
	public bool IsReady() { return _updated; }

	/// <summary>
	/// Resets time stamp
	/// </summary>
	public void ResetTimeStampForKey(string key)
	{
		if(_updated)
		{
			SecurePlayerPrefs.SetInt(GetKeyForTimeStamp(key), UnixUtcNow);
			SecurePlayerPrefs.Save();
		}
		else
		{
			_delayedResetKeys.Add(key);
		}
	}

	/// <summary>
	/// Returns time elapsed from the time stamp
	/// </summary>
	public int GetElapsedTimeForKey(string key)
	{
		//DebugLog("GetElapsedTimeForKey: " + key);
		if (!_updated)
		{
			DebugLog(@"TimeManager is not updated yet");
			return 0;
		}

		string timeKey = GetKeyForTimeStamp(key);
		if (!SecurePlayerPrefs.HasKey(timeKey))
			return DateTimeToUnix(DateTime.UtcNow);

		int elapsedTime = UnixUtcNow - SecurePlayerPrefs.GetInt(timeKey);
		//DebugLog(@"Elapsed time: " + elapsedTime);
		return elapsedTime;
	}

	/// <summary>
	/// Shift timestamp
	/// </summary>
	public void ShiftTimestampForKey(string key, int timeInterval)
	{
		string timeKey = GetKeyForTimeStamp(key);
		if (SecurePlayerPrefs.HasKey(timeKey))
		{
			int deviceTimestamp = SecurePlayerPrefs.GetInt(timeKey);
			SecurePlayerPrefs.SetInt(timeKey, deviceTimestamp + timeInterval);
		}
	}

	/// <summary>
	/// Returns time to the end of timeInterval
	/// </summary>
	public int FireTimeForKey(string key, int timeInterval)
	{
		int elapsedTime = GetElapsedTimeForKey(key);
		if (elapsedTime < 0)
			return timeInterval;
		if (elapsedTime >= timeInterval)
			return 0;
		return timeInterval - elapsedTime;
	}

	public bool TimeSavedForKey(string key)
	{
		string timeKey = GetKeyForTimeStamp(key);
		return SecurePlayerPrefs.HasKey(timeKey);
	}
	
	public void RemoveTimeStampForKey(string key)
	{
		string timeKey = GetKeyForTimeStamp(key);
		if (SecurePlayerPrefs.HasKey(timeKey))
			SecurePlayerPrefs.DeleteKey(timeKey);
	}
	/// <summary>
	/// Устанавливает временную метку для ключа так, чтобы до истечения таймера timeInterval оставалось remainingTime секунд
	/// </summary>
	/// <param name="key">Ключ события</param>
	/// <param name="timeInterval">Общая продолжительность события в секундах</param>
	/// <param name="remainingTime">Желаемое оставшееся время в секундах</param>
	public void SetFireTimeForKey(string key, int timeInterval, int remainingTime)
	{
	    if (!_updated)
	    {
	        DebugLog($"TimeManager еще не обновлен. Невозможно установить время для ключа {key}");
	        return;
	    }
	
	    // Рассчитываем, когда должно было начаться событие, чтобы до его окончания осталось remainingTime секунд
	    int currentTime = UnixUtcNow;
	    int elapsedTime = timeInterval - remainingTime;
	    int startTime = currentTime - elapsedTime;
	    
	    // Устанавливаем метку времени начала события
	    string timeKey = GetKeyForTimeStamp(key);
	    SecurePlayerPrefs.SetInt(timeKey, startTime);
	    SecurePlayerPrefs.Save();
	    
	    DebugLog($"Установлено оставшееся время {remainingTime} сек для ключа {key} (общая продолжительность {timeInterval} сек)");
	}
	#endregion

	private void ProcessDelayedResetKeys()
	{
		foreach(string key in _delayedResetKeys)
			ResetTimeStampForKey(key);
		_delayedResetKeys.Clear();
	}

	private void UpdateServerTimeStamp()
	{
		if (_updating)
			return;
	
		DebugLog("UpdateServerTimeStamp");
		
		_updating = true;
		GetNtpTime().ContinueWith(task =>
		{
			if (task.IsFaulted)
				Debug.LogException(task.Exception);
			ProcessNewServerTimestamp();
		}, TaskScheduler.FromCurrentSynchronizationContext());
	}
	
	private async Task GetNtpTime()
	{
		DebugLog("GetNtpTime");

#if DEBUG
		_serverTimestamp = DateTimeToUnix(DateTime.UtcNow);
		_deviceTimestamp = DateTimeToUnix(DateTime.UtcNow);
		return;
#endif
		_serverTimestamp = -1;
		_deviceTimestamp = -1;

		bool done = false;
		string[] ntpServers = NTP_LIST.Split(' ');
		int i = 0;
		while (i < ntpServers.Length && !done)
		{
			try
			{
				string ntpServer = ntpServers[i++];
				DebugLog("Try to get ntp time from server: " + ntpServer);

				// NTP message size - 16 bytes of the digest (RFC 2030)
				var ntpData = new byte[48];

				//Setting the Leap Indicator, Version Number and Mode values
				ntpData[0] = 0x1B; //LI = 0 (no warning), VN = 3 (IPv4 only), Mode = 3 (Client Mode)

				var addresses = (await Dns.GetHostEntryAsync(ntpServer)).AddressList;

				//The UDP port number assigned to NTP is 123
				var ipEndPoint = new IPEndPoint(addresses[0], 123);

				//NTP uses UDP
				using(var socket = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp))
				{
					socket.Connect(ipEndPoint);

					//Stops code hang if NTP is blocked
					socket.ReceiveTimeout = SERVER_TIMEOUT;

					socket.Send(ntpData);
					socket.Receive(ntpData);
					socket.Close();
				}

				//Offset to get to the "Transmit Timestamp" field (time at which the reply 
				//departed the server for the client, in 64-bit timestamp format."
				const byte serverReplyTime = 40;

				//Get the seconds part
				ulong intPart = BitConverter.ToUInt32(ntpData, serverReplyTime);

				//Get the seconds fraction
				ulong fractionPart = BitConverter.ToUInt32(ntpData, serverReplyTime + 4);

				//Convert From big-endian to little-endian
				intPart = SwapEndianness(intPart);
				fractionPart = SwapEndianness(fractionPart);

				var milliseconds = (intPart * 1000) + ((fractionPart * 1000) / 0x100000000L);

				//**UTC** time
				DateTime networkDateTime = (new DateTime(1900, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds((long)milliseconds);
				
				_serverTimestamp = DateTimeToUnix(networkDateTime);
				_deviceTimestamp = DateTimeToUnix(DateTime.UtcNow);

				DebugLog("Ntp time is received: " + _serverTimestamp);

				done = true;
			}
			catch (Exception exception)
			{
				DebugLog("GetNtpTime exception: " + exception.Message);
			}
		}

		if (!done)
			DebugLog("Ntp time is not received: " + _serverTimestamp);

		if (_serverTimestamp <= 0)
			_serverTimestamp = DateTimeToUnix(DateTime.UtcNow);
		
		if (_deviceTimestamp <= 0)
			_deviceTimestamp = DateTimeToUnix(DateTime.UtcNow);
	}

	private void ProcessNewServerTimestamp()
	{
		DebugLog("ProcessNewServerTimestamp: " + _serverTimestamp + ", deviceTimestamp: " + _deviceTimestamp);
		
		if (_serverTimestamp > SecurePlayerPrefs.GetInt(kRealNtpServerTime))
		{
			SecurePlayerPrefs.SetInt(kRealNtpServerTime, _serverTimestamp);
			SecurePlayerPrefs.Save();
		}
		
		_serverTimeDelta = _serverTimestamp - _deviceTimestamp;
		
		DebugLog("server time delta: " + _serverTimeDelta);
		
		_updating = false;
		_updated = true;

		ProcessDelayedResetKeys();

		OnTimeManagerUpdated?.Invoke();
	}

	#region Utils

	private static string GetKeyForTimeStamp(string key)
	{
		return kDeviceTimestamp + key;
	}

	// stackoverflow.com/a/3294698/162671

	private static uint SwapEndianness(ulong x)
	{
		return (uint) (
			((x & 0x000000ff) << 24) +
			((x & 0x0000ff00) << 8) +
			((x & 0x00ff0000) >> 8) +
			((x & 0xff000000) >> 24));
	}

	private static void DebugLog(string text)
	{
#if UNITY_EDITOR
		Debug.Log("<color=green>[TimeManager] " + text + "</color>");
#elif DEBUG
		Debug.Log("[TimeManager] " + text);
#endif
	}
	
	#endregion
}
