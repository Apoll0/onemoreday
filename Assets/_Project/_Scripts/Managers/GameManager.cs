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
        LastChanceBlockController.OnOneMoreDayTriggered += OnOneMoreDayTriggered;
        LastChanceBlockController.OnNoThanksTriggered += NoThanksTriggered;
    }
    
    private void OnDisable()
    {
        ChoiceController.OnMadeChoice -= OnMadeChoice;
        LastChanceBlockController.OnOneMoreDayTriggered -= OnOneMoreDayTriggered;
        LastChanceBlockController.OnNoThanksTriggered -= NoThanksTriggered;
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
        EnergyManager.Instance.IncrementEnergy(-1); // consume one energy to start the game

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
        
        var chance = UnityEngine.Random.value;
        if (chance < GameConstants.RareEventChance)
        {
            MyDebug.Log("[GameManager] Rare event triggered!");
            _currentEventData = EventsData.Instance.GetRandomRareEvent();
        }
        else if (chance < GameConstants.MidEventChance + GameConstants.RareEventChance)
        {
            MyDebug.Log("[GameManager] Mid event triggered!");
            _currentEventData = EventsData.Instance.GetRandomMidEvent();
        }
        else
        {
            MyDebug.Log("[GameManager] Basic event triggered!");
            _currentEventData = EventsData.Instance.GetRandomBasicEvent();
        }
        
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

    private IEnumerator ChangeDayAfterChoiceCoroutine(bool toLeft, bool forceGameOver = false)
    {
        yield return new WaitForSeconds(GameConstants.StatChangeDuration);
        
        _mainUIManager.HideCurrentDayBlock(toLeft);
        
        yield return  new WaitForSeconds(GameConstants.CardRotateDuration / 4f);

        if (forceGameOver)
        {
            TriggerGameOver(StatType.Body); // TODO: change to upgrade behavior
            yield break;
        }
        
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
                TriggerGameOver(statType);
                return true;
            }
        }
        return false;
    }

    private void CorrectStatsFromZero()
    {
        foreach (StatType statType in Enum.GetValues(typeof(StatType)))
        {
            if (DataManager.Instance.GetStat(statType) <= 0)
            {
                DataManager.Instance.SetStat(statType, 1);
            }
        }
    }

    private void TriggerGameOver(StatType statType)
    {
        if (CurrentGameState == GameState.GameOver)
            return;

        CurrentGameState = GameState.GameOver;
        DataManager.Instance.IncrementScullsCount();
        _mainUIManager.ShowLastChanceBlock(statType);
    }

    #endregion

    #region Delegates

    private void OnMadeChoice(ChoiceController choiceController)
    {
        MyDebug.Log("[GameManager] Made choice " + choiceController.ChoiceIndex);
        var choice = _currentEventData.choices[choiceController.ChoiceIndex];

        if (choice.isDeath)
        {
            // Handle death choice
            DisableTouches();
            StartCoroutine(ChangeDayAfterChoiceCoroutine(choiceController.ChoiceIndex == 0, true)); // 0 - left, 1 - right
        }
        else if (choice.isRandom2)
        {
            // add random effect +2 or -2
            var plusOrMinus = UnityEngine.Random.value < 0.5f ? 2 : -2;
            var statTypes = new [] { StatType.Body, StatType.Mind, StatType.Supplies, StatType.Hope };
            var randomStat = statTypes[UnityEngine.Random.Range(0, statTypes.Length)];
            DataManager.Instance.SetStat(randomStat, DataManager.Instance.GetStat(randomStat) + plusOrMinus);
            
            // TODO: Поправить статы. Анимировать стрелки в цифры. Анимировать статы на экране.
            DisableTouches();
            _mainUIManager.OpenArrowOnRandomStat(randomStat, plusOrMinus);

            StartCoroutine(ChangeDayAfterChoiceCoroutine(choiceController.ChoiceIndex == 0)); // 0 - left, 1 - right
        }
        else
        {
            DataManager.Instance.SetStat(StatType.Body, DataManager.Instance.GetStat(StatType.Body) + choice.bodyEffect);
            DataManager.Instance.SetStat(StatType.Mind, DataManager.Instance.GetStat(StatType.Mind) + choice.mindEffect);
            DataManager.Instance.SetStat(StatType.Supplies, DataManager.Instance.GetStat(StatType.Supplies) + choice.suppliesEffect);
            DataManager.Instance.SetStat(StatType.Hope, DataManager.Instance.GetStat(StatType.Hope) + choice.hopeEffect);
        
            // TODO: Поправить статы. Анимировать стрелки в цифры. Анимировать статы на экране.
            DisableTouches();
            _mainUIManager.OpenArrowsOnCurrentDayBlock();

            StartCoroutine(ChangeDayAfterChoiceCoroutine(choiceController.ChoiceIndex == 0)); // 0 - left, 1 - right
        }
    }

    private void OnOneMoreDayTriggered()
    {
        MyDebug.Log("[GameManager] One more day triggered.");
        _mainUIManager.HideLastChanceBlock(() =>
        {
            CurrentGameState = GameState.InGame;
            CorrectStatsFromZero();
            BeginNewDay();
        });
    }

    private void NoThanksTriggered()
    {
        MyDebug.Log("[GameManager] No thanks triggered. Returning to main menu.");
        _mainUIManager.HideLastChanceBlock(() =>
        {
            CurrentGameState = GameState.MainMenu;
            _mainUIManager.ShowStartView();
        });
    }
    
    #endregion
}
