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
    public static event Action<StatType> OnStatChanged;
    
    #region Properties
    
    public int CurrentDay
    {
        get => SecurePlayerPrefs.HasKey("CurrentDay") ? SecurePlayerPrefs.GetInt("CurrentDay") : 1;
        set
        {
            SecurePlayerPrefs.SetInt("CurrentDay", value);
            SecurePlayerPrefs.Save();
        }
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
