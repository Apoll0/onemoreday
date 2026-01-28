using System;
using UnityEngine;

public enum StatType
{
    Body,
    Mind,
    Supplies,
    Hope,
}

public class DataManager : HMSingleton<DataManager>
{
    #region Constants

    private readonly string kCurrentDayPref = "CurrentDay";
    private readonly string kMaxDayReached = "MaxDayReached";
    private readonly string kScullsCountPref = "ScullsCount";
    private readonly string kRemoveAdsPref = "RemoveAdsPurchased";
    
    private const int kMaxScullsCount = 182;

    #endregion

    public static event Action<StatType> OnStatChanged;
    public static event Action<int> OnScullsCountChanged;
    
    #region Properties
    
    public int CurrentDay
    {
        get => SecurePlayerPrefs.HasKey(kCurrentDayPref) ? SecurePlayerPrefs.GetInt(kCurrentDayPref) : 1;
        set
        {
            SecurePlayerPrefs.SetInt(kCurrentDayPref, value);
            if(value > MaxDayReached)
                MaxDayReached = value;
            SecurePlayerPrefs.Save();
        }
    }

    public int MaxDayReached
    {
        get => SecurePlayerPrefs.HasKey(kMaxDayReached) ? SecurePlayerPrefs.GetInt(kMaxDayReached) : 0;
        private set => SecurePlayerPrefs.SetInt(kMaxDayReached, value);
    }

    public int ScullsCount
    {
        get => SecurePlayerPrefs.HasKey(kScullsCountPref) ? SecurePlayerPrefs.GetInt(kScullsCountPref) : 0;
        private set
        {
            var currentValue = SecurePlayerPrefs.HasKey(kScullsCountPref)
                ? SecurePlayerPrefs.GetInt(kScullsCountPref)
                : 0;
            var clampedValue = Mathf.Clamp(value, currentValue, kMaxScullsCount);
            if (clampedValue == currentValue)
                return;

            SecurePlayerPrefs.SetInt(kScullsCountPref, clampedValue);
            SecurePlayerPrefs.Save();
            OnScullsCountChanged?.Invoke(clampedValue);
        }
    }

    public bool RemoveAdsPurchased
    {
        get => SecurePlayerPrefs.GetBool(kRemoveAdsPref);
        set
        {
            if (value)
                SecurePlayerPrefs.SetBool(kRemoveAdsPref, true);
            else
                SecurePlayerPrefs.DeleteKey(kRemoveAdsPref);
            SecurePlayerPrefs.Save();
        }
    }
    
    #endregion

    public void SetPersistentStat(StatType stat, int value) // after upgrade
    {
        string key = $"PersistentStat_{stat}";
        SecurePlayerPrefs.SetInt(key, value);
        SecurePlayerPrefs.Save();
    }

    public int GetPersistentStat(StatType stat)
    {
        string key = $"PersistentStat_{stat}";
        var val = SecurePlayerPrefs.GetInt(key);
        if(val == 0) 
            val = GameConstants.StatDefault;
        
        return val;
    }
    
    public void SetStat(StatType stat, int value)
    {
        var oldValue = GetStat(stat);
        if (oldValue != value)
        {
            string key = $"Stat_{stat}";
            SecurePlayerPrefs.SetInt(key, value);
            SecurePlayerPrefs.Save();
            OnStatChanged?.Invoke(stat);
        }
    }
    
    public int GetStat(StatType stat)
    {
        string key = $"Stat_{stat}";
        return SecurePlayerPrefs.GetInt(key);
    }

    public int IncrementScullsCount()
    {
        var nextValue = Mathf.Min(ScullsCount + 1, kMaxScullsCount);
        if (nextValue != ScullsCount)
        {
            ScullsCount = nextValue;
        }
        return ScullsCount;
    }
}
