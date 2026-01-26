using System.Linq;
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

    public EventData GetRandomUpgradeWithProbability(float bodyProb, float mindProb, float suppliesProb, float hopeProb,
        float mixedProb, float safetyProb, float highRiskProb, float chaosProb)
    {
        float totalProb = bodyProb + mindProb + suppliesProb + hopeProb + mixedProb + safetyProb + highRiskProb + chaosProb;
        float randomValue = Random.Range(0f, totalProb);
        
        if (randomValue < bodyProb)
            return GetRandomBodyUpgrade();
        randomValue -= bodyProb;
        
        if (randomValue < mindProb)
            return GetRandomMindUpgrade();
        randomValue -= mindProb;
        
        if (randomValue < suppliesProb)
            return GetRandomSuppliesUpgrade();
        randomValue -= suppliesProb;
        
        if (randomValue < hopeProb)
            return GetRandomHopeUpgrade();
        randomValue -= hopeProb;
        
        if (randomValue < mixedProb)
            return GetRandomMixedUpgrade();
        randomValue -= mixedProb;
        
        if (randomValue < safetyProb)
            return GetRandomSafetyUpgrade();
        randomValue -= safetyProb;
        
        if (randomValue < highRiskProb)
            return GetRandomHighRiskUpgrade();
        randomValue -= highRiskProb;
        
        return GetRandomChaosUpgrade();
    }
    
    public EventData GetSafetyUpgradeByStat(StatType stat)
    {
        foreach (var upgrade in _safetyUpgrades)
        {
            var choice = upgrade.choices[0];
            if(stat == StatType.Body && choice.bodyEffect > 0 || 
               stat == StatType.Mind && choice.mindEffect > 0 || 
               stat == StatType.Supplies && choice.suppliesEffect > 0 ||
               stat == StatType.Hope && choice.hopeEffect > 0)
                return upgrade;
        }
        return null;
    }
    
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
        if (upgrades == null || upgrades.Length == 0)
        {
            MyDebug.LogRed("[UpgradesData] No upgrades available.");
            return null;
        }

        var upgradesCopy = upgrades.ToList();
        // exclude upgrades that can make zero stat
        upgradesCopy.RemoveAll(upgrade =>
        {
            var choice = upgrade.choices[0];
            return DataManager.Instance.GetPersistentStat(StatType.Body) + choice.bodyEffect <= 0 ||
                   DataManager.Instance.GetPersistentStat(StatType.Mind) + choice.mindEffect <= 0 ||
                   DataManager.Instance.GetPersistentStat(StatType.Supplies) + choice.suppliesEffect <= 0 ||
                   DataManager.Instance.GetPersistentStat(StatType.Hope) + choice.hopeEffect <= 0;
        });
        
        
        int randomIndex = UnityEngine.Random.Range(0, upgradesCopy.Count);
        var upgradeData =  upgradesCopy[randomIndex];
        // Decide which set of choices to use
        var choiceArray = upgradeData.choices;
        if (upgradeData.choices2 != null && upgradeData.choices2.Length > 0 && Random.value > 0.5f)
        {
            choiceArray = upgradeData.choices2;
        }
        
        var upgradeCopy = new EventData
        {
            picName = upgradeData.picName,
            name = upgradeData.name,
            description = upgradeData.description,
            choices = new Choice[choiceArray.Length]
        };
        
        for (int i = 0; i < choiceArray.Length; i++)
        {
            var choice = choiceArray[i];
            upgradeCopy.choices[i] = new Choice
            {
                text = choice.text,
                isDeath = choice.isDeath,
                isRandom2 = choice.isRandom2,
                bodyEffect = choice.bodyEffect,
                mindEffect = choice.mindEffect,
                suppliesEffect = choice.suppliesEffect,
                hopeEffect = choice.hopeEffect
            };
        }
        
        return upgradeCopy;
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
