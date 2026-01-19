using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class EventData
{
    public string picName;
    public string name;
    public string description;
    public Decision[] decisions;
    [HideInInspector] public Sprite sprite;

    public async void LoadImageAsync(Image image)
    {
        if (string.IsNullOrEmpty(picName))
        {
            MyDebug.LogRed($"[EventData] picName is null or empty.");
            return;
        }
    
        var request = Resources.LoadAsync<Sprite>(picName);
        
        while (!request.isDone)
        {
            await System.Threading.Tasks.Task.Yield();
        }
    
        sprite = request.asset as Sprite;
        
        if (sprite != null && image != null)
        {
            image.sprite = sprite;
        }
        else
        {
            MyDebug.LogRed($"[Situation] Failed to load sprite: {picName}");
        }
    }
}

[Serializable]
public class Decision
{
    public string text;
    public int bodyEffect;
    public int mindEffect;
    public int suppliesEffect;
    public int hopeEffect;
}

[CreateAssetMenu(fileName = "EventsData", menuName = "Scriptable Objects/EventsData")]
public class EventsData : HMScriptableSingleton<EventsData>
{
    [SerializeField] private EventData[] _basicEvents;

    #region Public Methods

    public EventData GetRandomBasicEvent()
    {
        if (_basicEvents == null || _basicEvents.Length == 0)
        {
            MyDebug.LogRed("[EventsData] No events available.");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, _basicEvents.Length);
        var eventData =  _basicEvents[randomIndex];
        return eventData;
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
