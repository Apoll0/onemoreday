using System;
using UnityEngine;

public enum GameState
{
    MainMenu,
    InGame,
    GameOver
}

public class GameManager : HMSingleton<GameManager>
{
    #region Properties

    public GameState CurrentGameState {get; private set;} = GameState.MainMenu;

    #endregion

    #region Init

    private void Start()
    {
        Initialize();
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
        DataManager.Instance.CurrentDay = 1;

        // Initialize player stats
        DataManager.Instance.SetStat(StatType.Body, GameConstants.StatDefault);
        DataManager.Instance.SetStat(StatType.Mind, GameConstants.StatDefault);
        DataManager.Instance.SetStat(StatType.Supplies, GameConstants.StatDefault);
        DataManager.Instance.SetStat(StatType.Hope, GameConstants.StatDefault);

        MyDebug.Log("[GameManager] Game started. Current Day: " + DataManager.Instance.CurrentDay);
    }

    #endregion

    #region Private Methods

    

    #endregion
}
