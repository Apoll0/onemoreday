using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "UpgradesData", menuName = "Scriptable Objects/UpgradesData")]
public class UpgradesData : HMScriptableSingleton<UpgradesData>
{
    #region Exposed vars

    [SerializeField] private EventData[] _bodyUpgrades;
    [SerializeField] private EventData[] _mindUpgrades;
    [SerializeField] private EventData[] _suppliesUpgrades;
    [SerializeField] private EventData[] _hopeUpgrades;
    [SerializeField] private EventData[] _mixedUpgrades;
    [SerializeField] private EventData[] _safetyUpgrades;
    [SerializeField] private EventData[] _highRiskUpgrades;
    [SerializeField] private EventData[] _chaosUpgrades;
    
    #endregion

    #region Public Methods

    public EventData GetRandomBodyUpgrade()
    {
        return GetRandomUpgrade(_bodyUpgrades);
    }
    public EventData GetRandomMindUpgrade()
    {
        return GetRandomUpgrade(_mindUpgrades);
    }
    public EventData GetRandomSuppliesUpgrade()
    {
        return GetRandomUpgrade(_suppliesUpgrades);
    }
    public EventData GetRandomHopeUpgrade()
    {
        return GetRandomUpgrade(_hopeUpgrades);
    }
    public EventData GetRandomMixedUpgrade()
    {
        return GetRandomUpgrade(_mixedUpgrades);
    }
    public EventData GetRandomSafetyUpgrade()
    {
        return GetRandomUpgrade(_safetyUpgrades);
    }
    public EventData GetRandomHighRiskUpgrade()
    {
        return GetRandomUpgrade(_highRiskUpgrades);
    }
    public EventData GetRandomChaosUpgrade()
    {
        return GetRandomUpgrade(_chaosUpgrades);
    }
    
    #endregion

    #region Private Methods

    private EventData GetRandomUpgrade(EventData[] upgrades)
    {
        if (upgrades.Length == 0)
        {
            Debug.LogWarning("No upgrades available!");
            return null;
        }
        
        int randomIndex = Random.Range(0, upgrades.Length);
        return upgrades[randomIndex];
    }
    
    [Button(enabledMode:EButtonEnableMode.Editor)]
    private void SaveNow()
    {
#if UNITY_EDITOR 
        UnityEditor.EditorUtility.SetDirty(this); 
        UnityEditor.AssetDatabase.SaveAssets(); 
#endif 
    }

    #endregion
}
