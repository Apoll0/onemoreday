using System;
using TMPro;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    #region Exposed variables

    [SerializeField] private GameObject _viewsContainer;
    [SerializeField] private GameObject _startView;
    [SerializeField] private TextMeshProUGUI _dynamicCaption;
    [SerializeField] private TextMeshProUGUI _bestNumberText;
    [SerializeField] private DayBlockController[] _dayBlockControllers;
    [SerializeField] private LastChanceBlockController _lastChanceBlockController;
    [Space]
    [SerializeField] private GameObject[] _energyBars;
    [SerializeField] private GameObject[] _energyBarsDisabled;
    [SerializeField] private TextMeshProUGUI _energyTimerText;

    #endregion

    #region Private vars

    private int _currentDayBlockIndex = 0;
    private bool _lastMoveSideToLeft = false;

    #endregion

    #region Init

    private void Start()
    {
        _dynamicCaption.text = "one more day"; // TODO: Localization
        
        foreach (DayBlockController dayBlockController in _dayBlockControllers)
            dayBlockController.gameObject.SetActive(true);
        _lastChanceBlockController.gameObject.SetActive(true);
        ShowStartView();
    }

    private void OnEnable()
    {
        EnergyManager.OnEnergyCountChanged += UpdateEnergyValue;
        EnergyManager.OnTimerToNextEnergyUpdated += UpdateEnergyTimerText;
    }

    private void OnDisable()
    {
        EnergyManager.OnEnergyCountChanged -= UpdateEnergyValue;
        EnergyManager.OnTimerToNextEnergyUpdated -= UpdateEnergyTimerText;
    }
    
    #endregion

    #region Button callbacks

    public void StartButtonPressed()
    {
        if(EnergyManager.Instance.EnergyCount == 0)
            return;
        
        GameManager.Instance.StartGame();
        _startView.SetActive(false);
    }

    public void SettingsButtonPressed()
    {
        // TODO: Implement settings UI
    }

    #endregion

    #region Public methods

    public void OpenArrowsOnCurrentDayBlock()
    {
        _dayBlockControllers[_currentDayBlockIndex].OpenChoicesArrows();
    }

    public void OpenArrowOnRandomStat(StatType choice, int value)
    {
        _dayBlockControllers[_currentDayBlockIndex].OpenArrowOnRandomStat(choice, value);
    }
    
    public void ShowNewDayBlock(EventData eventData, Action callback = null)
    {
        _dynamicCaption.text = "day " + DataManager.Instance.CurrentDay; // TODO: Localization and animation
        ChangeDayBlockIndex();
        _dayBlockControllers[_currentDayBlockIndex].InitWithEventData(eventData);
        _dayBlockControllers[_currentDayBlockIndex].ShowFrom(!_lastMoveSideToLeft, callback);
    }

    public void HideCurrentDayBlock(bool toLeft, Action callback = null)
    {
        _lastMoveSideToLeft = toLeft;
        _dayBlockControllers[_currentDayBlockIndex].HideTo(toLeft, callback);
    }

    public void ShowLastChanceBlock(StatType failStat)
    {
        _lastChanceBlockController.InitWithStatType(failStat);
        _lastChanceBlockController.ShowFrom(!_lastMoveSideToLeft);
    }

    public void HideLastChanceBlock(Action callback = null)
    {
        _lastChanceBlockController.HideTo(_lastMoveSideToLeft, callback);
    }
    
    public void ShowStartView()
    {
        _bestNumberText.text = DataManager.Instance.MaxDayReached + " days"; // TODO: Localization
        UpdateEnergyValue();
        UpdateEnergyTimerText();
        _startView.SetActive(true);
    }
    
    #endregion

    #region Private methods

    private void ChangeDayBlockIndex()
    {
        _currentDayBlockIndex++;
        if (_currentDayBlockIndex >= _dayBlockControllers.Length)
        {
            _currentDayBlockIndex = 0;
        }
    }

    private void UpdateEnergyValue()
    {
        var energy = EnergyManager.Instance.EnergyCount;
        for (int i = 0; i < _energyBars.Length; i++)
        {
            bool isActive = i < energy;
            _energyBars[i].SetActive(isActive);
            _energyBarsDisabled[i].SetActive(!isActive);
        }
    }

    private void UpdateEnergyTimerText()
    {
        var timeToNextEnergy = EnergyManager.Instance.TimeToNextEnergy;
        if (timeToNextEnergy > 0)
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeToNextEnergy);
            _energyTimerText.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        else
        {
            _energyTimerText.text = "full"; // TODO: Localization
        }
    }
    
    #endregion
}
