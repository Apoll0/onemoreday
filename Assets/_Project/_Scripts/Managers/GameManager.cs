using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using Random = System.Random;

public enum GameState
{
    MainMenu,
    InGame,
    GameOver
}

public class GameManager : HMSingleton<GameManager>
{       
    #region Events
    

    #endregion
    
    [SerializeField] private EventSystem _eventSystem;
    [SerializeField] private MainUIManager _mainUIManager;

    #region Private vars

    private EventData _currentEventData;

    #endregion
    
    #region Properties

    public GameState CurrentGameState {get; private set;} = GameState.MainMenu;

    #endregion

    #region Init

    private void Start()
    {
        Initialize();
    }

    private void OnEnable()
    {
        ChoiceController.OnMadeChoice += OnMadeChoice;
    }
    
    private void OnDisable()
    {
        ChoiceController.OnMadeChoice -= OnMadeChoice;
    }

    #endregion

    #region Public Methods

    public void Initialize()
    {
        HMTimeManager.Instance.Initialize();
        EnergyManager.Instance.Initialize();
    }
    
    public void StartGame()
    {
        if (CurrentGameState == GameState.InGame)
            return;

        CurrentGameState = GameState.InGame;
        DataManager.Instance.CurrentDay = 0;

        // Initialize player stats
        DataManager.Instance.SetStat(StatType.Body, GameConstants.StatDefault);
        DataManager.Instance.SetStat(StatType.Mind, GameConstants.StatDefault);
        DataManager.Instance.SetStat(StatType.Supplies, GameConstants.StatDefault);
        DataManager.Instance.SetStat(StatType.Hope, GameConstants.StatDefault);

        MyDebug.Log("[GameManager] Game started. Current Day: " + DataManager.Instance.CurrentDay);
        
        BeginNewDay();
    }

    public void DisableTouches()
    {
        _eventSystem.enabled = false;
    }
    
    public void EnableTouches()
    {
        _eventSystem.enabled = true;
    }
    
    #endregion

    #region Private Methods

    private void BeginNewDay()
    {
        DataManager.Instance.CurrentDay++;
        _currentEventData = UnityEngine.Random.value < GameConstants.RareEventChance ? EventsData.Instance.GetRandomRareEvent() : EventsData.Instance.GetRandomBasicEvent();
        UpdateCurrentEventStats();
        _mainUIManager.ShowNewDayBlock(_currentEventData);
    }

    private void UpdateCurrentEventStats()
    {
        int GetFactor()
        {
            var factor = 1;
            var rnd = UnityEngine.Random.value;
            if(rnd > 0.9f)
                factor = 3;
            else if(rnd > 0.6f)
                factor = 2;
            return factor;
        }
        
        for (int i = 0; i < _currentEventData.choices.Length; i++)
        {
            var choice = _currentEventData.choices[i];
            choice.bodyEffect *= GetFactor();
            choice.mindEffect *= GetFactor();
            choice.suppliesEffect *= GetFactor();
            choice.hopeEffect *= GetFactor();
        }
    }
    
    private void OnMadeChoice(ChoiceController choiceController)
    {
        MyDebug.Log("[GameManager] Made choice " + choiceController.ChoiceIndex);
        var choice = _currentEventData.choices[choiceController.ChoiceIndex];
        
        DataManager.Instance.SetStat(StatType.Body, DataManager.Instance.GetStat(StatType.Body) + choice.bodyEffect);
        DataManager.Instance.SetStat(StatType.Mind, DataManager.Instance.GetStat(StatType.Mind) + choice.mindEffect);
        DataManager.Instance.SetStat(StatType.Supplies, DataManager.Instance.GetStat(StatType.Supplies) + choice.suppliesEffect);
        DataManager.Instance.SetStat(StatType.Hope, DataManager.Instance.GetStat(StatType.Hope) + choice.hopeEffect);
        
        // TODO: Поправить статы. Анимировать стрелки в цифры. Анимировать статы на экране.
        DisableTouches();
        _mainUIManager.OpenArrowsOnCurrentDayBlock();

        StartCoroutine(ChangeDayAfterChoiceCoroutine(choiceController.ChoiceIndex == 0)); // 0 - left, 1 - right
    }

    private IEnumerator ChangeDayAfterChoiceCoroutine(bool toLeft)
    {
        yield return new WaitForSeconds(GameConstants.StatChangeDuration);
        
        _mainUIManager.HideCurrentDayBlock(toLeft);
        
        yield return  new WaitForSeconds(GameConstants.CardRotateDuration / 4f);

        if (!IsGameOver())
            BeginNewDay();
    }
    
    private bool IsGameOver()
    {
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            if (DataManager.Instance.GetStat(statType) <= 0)
            {
                MyDebug.Log("[GameManager] Game Over! Stat " + statType + " reached zero.");
                CurrentGameState = GameState.GameOver;
                _mainUIManager.ShowLastChanceBlock(statType);
                return true;
            }
        }
        return false;
    }
    
    #endregion
}
