using NaughtyAttributes;
using UnityEngine;
using Random = UnityEngine.Random;

[CreateAssetMenu(fileName = "EventsData", menuName = "Scriptable Objects/EventsData")]
public class UpgradesData : HMScriptableSingleton<UpgradesData>
{
    [SerializeField] private EventData[] _basicEvents;
    [SerializeField] private EventData[] _midEvents;
    [SerializeField] private EventData[] _rareEvents;

    #region Public Methods

    public UpgradesData GetUpgradesData()
    {
        return null;
    }
    
    #endregion

    #region Private Methods

    
    
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
