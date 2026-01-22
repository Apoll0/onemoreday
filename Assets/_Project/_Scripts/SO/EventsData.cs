using System;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

[Serializable]
public class EventData
{
    public string picName;
    public string name;
    public string description;
    [FormerlySerializedAs("decisions")] public Choice[] choices;
    public Choice[] choices2; // only if needed alternative set of choices
    [HideInInspector] public Sprite sprite;

    public async void LoadImageAsync(Image image)
    {
        if (string.IsNullOrEmpty(picName))
        {
            MyDebug.LogRed($"[EventData] picName is null or empty.");
            return;
        }
    
        var request = Resources.LoadAsync<Sprite>("Ills/" + picName);
        
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
public class Choice
{
    public string text;
    [ShowIf("ShowDeath")]
    [AllowNesting] 
    public bool isDeath;
    [ShowIf("ShowRandom2")]
    [AllowNesting] 
    public bool isRandom2;
    [ShowIf("ShowStat")]
    [AllowNesting] 
    public int bodyEffect;
    [ShowIf("ShowStat")]
    [AllowNesting] 
    public int mindEffect;
    [ShowIf("ShowStat")]
    [AllowNesting] 
    public int suppliesEffect;
    [ShowIf("ShowStat")]
    [AllowNesting] 
    public int hopeEffect;
    
    public bool ShowStat() { return (!isDeath && !isRandom2); }
    public bool ShowRandom2 => !isDeath;
    public bool ShowDeath => !isRandom2;
}

[CreateAssetMenu(fileName = "EventsData", menuName = "Scriptable Objects/EventsData")]
public class EventsData : HMScriptableSingleton<EventsData>
{
    [SerializeField] private EventData[] _basicEvents;
    [SerializeField] private EventData[] _midEvents;
    [SerializeField] private EventData[] _rareEvents;

    #region Public Methods

    public EventData GetRandomBasicEvent()
    {
        return GetRandomEvent(_basicEvents);
    }

    public EventData GetRandomMidEvent()
    {
        return GetRandomEvent(_midEvents);
    }
    
    public EventData GetRandomRareEvent()
    {
        return GetRandomEvent(_rareEvents);
    }
    
    #endregion

    #region Private Methods

    private EventData GetRandomEvent(EventData[] eventsArray)
    {
        if (eventsArray == null || eventsArray.Length == 0)
        {
            MyDebug.LogRed("[EventsData] No events available.");
            return null;
        }

        int randomIndex = UnityEngine.Random.Range(0, eventsArray.Length);
        var eventData =  eventsArray[randomIndex];
        var eventCopy = new EventData
        {
            picName = eventData.picName,
            name = eventData.name,
            description = eventData.description,
            choices = new Choice[eventData.choices.Length]
        };
        
        // Decide which set of choices to use
        var choiceArray = eventData.choices;
        if(eventData.choices2.Length >= 2 && Random.value > 0.5f)
        {
            choiceArray = eventData.choices2;
        }
        
        for (int i = 0; i < choiceArray.Length; i++)
        {
            var choice = choiceArray[i];
            eventCopy.choices[i] = new Choice
            {
                text = choice.text,
                bodyEffect = choice.bodyEffect,
                mindEffect = choice.mindEffect,
                suppliesEffect = choice.suppliesEffect,
                hopeEffect = choice.hopeEffect
            };
        }
        
        return eventCopy;
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
