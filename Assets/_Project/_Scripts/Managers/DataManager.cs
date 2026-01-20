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

    #endregion

    public static event Action<StatType> OnStatChanged;
    
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
    
    #endregion
    
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
}
