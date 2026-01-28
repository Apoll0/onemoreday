using System;
using NaughtyAttributes;
using UnityEngine;

public class EnergyManager : HMSingleton<EnergyManager>
{
    private const string kPrefsEnergyAddTimestamp = "EnergyAddTimeStamp";
    private const string kPrefsEnergyCount = "EnergyCount";
    private const string kPrefsInfiniteEnergySeconds = "InfiniteEnergySeconds";
    private const string kPrefsInfiniteEnergyTimeStamp = "InfiniteEnergyTimeStamp";

    public static event Action OnEnergyCountChanged;
    public static event Action OnTimerToNextEnergyUpdated;

    #region Private vars
    
    private bool _initialized;
    private bool _isMaxEnergy;
    private int _lastTimeToNexEnergy;
    private Vector3 _topHeartPosition;
    private bool _isInfiniteEnergy;
    private float _timeToNextInfiniteEnergyCheck;
    
    #endregion    

    private int EnergyAddTimeStamp
    {
        get => SecurePlayerPrefs.GetInt(kPrefsEnergyAddTimestamp);
        set => SecurePlayerPrefs.SetInt(kPrefsEnergyAddTimestamp, value);
    }

    public int EnergyCount
    {
        get => SecurePlayerPrefs.GetInt(kPrefsEnergyCount);
        private set => SecurePlayerPrefs.SetInt(kPrefsEnergyCount, value);
    }
    
    public int TimeToNextEnergy => GameConstants.EnergyAddCooldown - HMTimeManager.UnixUtcNow + EnergyAddTimeStamp;

    #region Public methods

    public void Initialize()
    {
        if (_initialized)
            return;
        
        UpdateEnergyCount();
        UpdateIsEnergyFullCachedStatus();
        _initialized = true;
        _isInfiniteEnergy = IsInfiniteEnergy();
        
        MyDebug.Log("[LivesManager] initialized. Energy count: " + EnergyCount);
    }

    public void SetOneEnergy()
    {
        EnergyCount = 0;
        IncrementEnergy(1);
    }
    
    public bool IsFullEnergy()
    {
        return EnergyCount >= GameConstants.EnergyStartQuantity;
    }

    public bool IsInfiniteEnergy()
    {
        //var timeForInfinite = SecurePlayerPrefs.GetInt(kPrefsInfiniteEnergySeconds);
        //var timeLeft = HMTimeManager.Instance.FireTimeForKey(kPrefsInfiniteEnergyTimeStamp, timeForInfinite);
        //return timeLeft > 0;
        return InifiniteEnergyTimeLeft() > 0;
    }

    public int InifiniteEnergyTimeLeft()
    {
        var timeForInfinite = SecurePlayerPrefs.GetInt(kPrefsInfiniteEnergySeconds);
        var timeLeft = HMTimeManager.Instance.FireTimeForKey(kPrefsInfiniteEnergyTimeStamp, timeForInfinite);
        return timeLeft;
    }
    
    public void IncrementEnergy(int value)
    {
        if(value <= 0 && IsInfiniteEnergy())
            return;

        if (EnergyCount >= GameConstants.EnergyStartQuantity && value > 0)
            return;
                
        int newValue = EnergyCount + value;
        if (newValue >= 0)
        {
            bool resetTimestamp = EnergyCount >= GameConstants.EnergyStartQuantity && newValue < GameConstants.EnergyStartQuantity;
            EnergyCount = newValue;
            if (resetTimestamp)
                EnergyAddTimeStamp = HMTimeManager.UnixUtcNow;
            UpdateIsEnergyFullCachedStatus();
            OnEnergyCountChanged?.Invoke();
        }
    }
    
    public void SetTopHeartPosition(Vector3 position)
    {
        _topHeartPosition = position;
    }

    public Vector3 GetTopHeartPosition()
    {
        return _topHeartPosition;
    }

    public void StartInfiniteEnergyFor(int seconds)
    {
        if (_isInfiniteEnergy)
        {
            var infiniteTimeLeft = InifiniteEnergyTimeLeft();
            seconds += infiniteTimeLeft;
        }
        
        MyDebug.Log("[InfiniteLives] Starting infinite lives for " + seconds + " seconds");
        SecurePlayerPrefs.SetInt(kPrefsInfiniteEnergySeconds, seconds);
        HMTimeManager.Instance.ResetTimeStampForKey(kPrefsInfiniteEnergyTimeStamp);
        _isInfiniteEnergy = true;
        _timeToNextInfiniteEnergyCheck = 1; // проверять будем раз в секунду
        EnergyCount = GameConstants.EnergyStartQuantity;
        OnEnergyCountChanged?.Invoke();
    }

    public int GetTimeToFullEnergy()
    {
        if(GameConstants.EnergyStartQuantity - EnergyCount == 0)
            return 0;
        
        int timeToFull = (GameConstants.EnergyStartQuantity - EnergyCount) * GameConstants.EnergyAddCooldown - (GameConstants.EnergyAddCooldown - TimeToNextEnergy);
        return timeToFull;
    }
    
    #endregion

    #region Private methods

    private void UpdateIsEnergyFullCachedStatus()
    {
        _isMaxEnergy = IsFullEnergy();
    }

    private void Update()
    {
        if (!_isMaxEnergy && !_isInfiniteEnergy)
        {
            int timeToNext = TimeToNextEnergy;
            if (timeToNext <= 0)
            {
                UpdateEnergyCount();
                UpdateIsEnergyFullCachedStatus();
            }

            if (timeToNext != _lastTimeToNexEnergy)
            {
                OnTimerToNextEnergyUpdated?.Invoke();
                _lastTimeToNexEnergy = timeToNext;
            }
        }

        if (_isInfiniteEnergy)
        {
            _timeToNextInfiniteEnergyCheck -= Time.deltaTime;
            if (_timeToNextInfiniteEnergyCheck <= 0)
            {
                var currentState = IsInfiniteEnergy();
                if (!currentState) // infinite life ended
                {
                    if (_isInfiniteEnergy)
                    {
                        SecurePlayerPrefs.DeleteKey(kPrefsInfiniteEnergySeconds);
                        HMTimeManager.Instance.RemoveTimeStampForKey(kPrefsInfiniteEnergyTimeStamp);
                    }
                    
                    _isInfiniteEnergy = false;
                    OnEnergyCountChanged?.Invoke();
                }
                
                OnTimerToNextEnergyUpdated?.Invoke();
                _timeToNextInfiniteEnergyCheck = 1;
            }
        }
    }

    private void UpdateEnergyCount()
    {
        if (EnergyAddTimeStamp == 0)
        {
            EnergyCount = GameConstants.EnergyStartQuantity;
            EnergyAddTimeStamp = HMTimeManager.UnixUtcNow;
        }
        else
        {
            //int wasLives = LivesCount;
            int time = HMTimeManager.UnixUtcNow - EnergyAddTimeStamp;
            while (time >= GameConstants.EnergyAddCooldown)
            {
                EnergyAddTimeStamp += GameConstants.EnergyAddCooldown;
                time -= GameConstants.EnergyAddCooldown;
                if (EnergyCount < GameConstants.EnergyStartQuantity)
                {
                    //IncrementLives();
                    EnergyCount++;
                    OnEnergyCountChanged?.Invoke();
                }
            }
        }
    }

    private void OnApplicationFocus(bool hasFocus)
    {
        if(!hasFocus)
            SecurePlayerPrefs.Save();
    }

    #endregion

    #region Debug

    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void StopInfiniteEnergy()
    {
        SecurePlayerPrefs.SetInt(kPrefsInfiniteEnergySeconds, 0);
        HMTimeManager.Instance.RemoveTimeStampForKey(kPrefsInfiniteEnergyTimeStamp);
        _isInfiniteEnergy = false;
        OnEnergyCountChanged?.Invoke();
    }
    
    [Button(enabledMode: EButtonEnableMode.Playmode)]
    private void IncrementEnergy()
    {
        IncrementEnergy(1);
    }
    
    [Button(enabledMode:EButtonEnableMode.Playmode)]
    private void DecrementLives()
    {
        IncrementEnergy(-1);
    }
    
    [Button(enabledMode:EButtonEnableMode.Playmode)]
    private void DecreaseTime20Seconds()
    {
        EnergyAddTimeStamp -= 20;
    }

    [Button(enabledMode:EButtonEnableMode.Playmode)]
    private void DecreaseTime1Minute()
    {
        EnergyAddTimeStamp -= 60;
    }
    
    [Button(enabledMode:EButtonEnableMode.Playmode)]
    private void DecreaseTime1Hour()
    {
        EnergyAddTimeStamp -= 3600;
    }
        
    [Button(enabledMode:EButtonEnableMode.Playmode)]
    private void StartInfiniteLives2Minutes()
    {
        StartInfiniteEnergyFor(120);
    }
    
    
    #endregion
}
