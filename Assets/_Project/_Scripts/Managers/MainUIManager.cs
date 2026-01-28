using System;
using Lofelt.NiceVibrations;
using TMPro;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    private const string kFirstGameCompleted = "FirstGameCompleted";
    private const string kDefaultDescription = "How long can you last?";
    private const string kUpgradesDescription = "Choose what matters";

    #region Exposed variables

    [SerializeField] private GameObject _viewsContainer;
    [SerializeField] private GameObject _startView;
    [SerializeField] private RollingTextAnimator _dynamicCaption;
    [SerializeField] private RollingTextAnimator _descriptionRoller;
    [SerializeField] private TextMeshProUGUI _bestNumberText;
    [SerializeField] private DayBlockController[] _dayBlockControllers;
    [SerializeField] private LastChanceBlockController _lastChanceBlockController;
    [SerializeField] private UpgradesBlockController _upgradesBlockController;
    [Space]
    [SerializeField] private GameObject[] _energyBars;
    [SerializeField] private GameObject[] _energyBarsDisabled;
    [SerializeField] private TextMeshProUGUI _energyTimerText;
    [SerializeField] private GameObject _energyContainer;
    [SerializeField] private GameObject _energyInfinite;
    [SerializeField] private GameObject _videoSignStartButton;
    [SerializeField] private GameObject _bestBlock;
    [SerializeField] private GameObject _settingsPopup;
    [SerializeField] private GameObject _settingsButton;

    #endregion

    #region Private vars

    private int _currentDayBlockIndex = 0;
    private bool _lastMoveSideToLeft = false;

    #endregion

    #region Init

    private void Start()
    {
        _dynamicCaption.ChangeTextQuick("one more day");
        
        foreach (DayBlockController dayBlockController in _dayBlockControllers)
            dayBlockController.gameObject.SetActive(true);
        _lastChanceBlockController.gameObject.SetActive(true);
        _upgradesBlockController.gameObject.SetActive(true);
        ShowStartView();
    }

    private void OnEnable()
    {
        EnergyManager.OnEnergyCountChanged += UpdateEnergyValue;
        EnergyManager.OnTimerToNextEnergyUpdated += UpdateEnergyTimerText;
        EnergyManager.OnEnergyCountChanged += UpdateEnergyTimerText;
    }

    private void OnDisable()
    {
        EnergyManager.OnEnergyCountChanged -= UpdateEnergyValue;
        EnergyManager.OnTimerToNextEnergyUpdated -= UpdateEnergyTimerText;
        EnergyManager.OnEnergyCountChanged -= UpdateEnergyTimerText;
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
        _settingsPopup.SetActive(true);
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
        _dynamicCaption.ChangeText("day " + DataManager.Instance.CurrentDay, false);
        _descriptionRoller.ChangeText(eventData.description, true);
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
        HapticPatterns.PlayPreset(HapticPatterns.PresetType.HeavyImpact);

        var desc = "";
        switch (failStat)
        {
            case StatType.Body:
                desc= "Your body couldn’t go any further";
                break;
            case StatType.Mind:
                desc = "Your mind finally broke";
                break;
            case StatType.Supplies:
                desc = "There was nothing left";
                break;
            case StatType.Hope:
                desc = "You lost the will to continue";
                break;
        }
        _descriptionRoller.ChangeText(desc, true);
        _lastChanceBlockController.ShowFrom(!_lastMoveSideToLeft);
    }
    
    public void ShowUpgradesBlock(Action callback = null)
    {
        _dynamicCaption.ChangeText("one more day", false);
        _descriptionRoller.ChangeText("Choose what matters", true);
        _upgradesBlockController.Show(callback);
    }

    public void HideLastChanceBlock(Action callback = null)
    {
        _lastChanceBlockController.HideTo(_lastMoveSideToLeft, callback);
    }
    
    public void ShowStartView()
    {
        _descriptionRoller.ChangeTextQuick(kDefaultDescription);
        _bestNumberText.text = DataManager.Instance.MaxDayReached + " days";
        UpdateEnergyValue();
        UpdateEnergyTimerText();
        _startView.SetActive(true);
        _bestBlock.SetActive(SecurePlayerPrefs.HasKey(kFirstGameCompleted));
    }

    public void CompleteFirstGame()
    {
        SecurePlayerPrefs.SetBool(kFirstGameCompleted, true);
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

    #endregion

    #region Delegate

    private void UpdateEnergyValue()
    {
        var energy = EnergyManager.Instance.EnergyCount;
        for (int i = 0; i < _energyBars.Length; i++)
        {
            bool isActive = i < energy;
            _energyBars[i].SetActive(isActive);
            _energyBarsDisabled[i].SetActive(!isActive);
        }
        
        _videoSignStartButton.SetActive(energy == 0);
    }

    private void UpdateEnergyTimerText()
    {
        if(EnergyManager.Instance.IsInfiniteEnergy())
        {
            _energyContainer.SetActive(false);
            _energyInfinite.SetActive(true);
            var timeToFinishInfinite = EnergyManager.Instance.InifiniteEnergyTimeLeft();
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeToFinishInfinite);
            _energyTimerText.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
            return;
        }
        else
        {
            _energyContainer.SetActive(true);
            _energyInfinite.SetActive(false);
        }
        
        var timeToNextEnergy = EnergyManager.Instance.TimeToNextEnergy;
        if (timeToNextEnergy > 0 && !EnergyManager.Instance.IsFullEnergy())
        {
            TimeSpan timeSpan = TimeSpan.FromSeconds(timeToNextEnergy);
            _energyTimerText.text = $"{timeSpan.Minutes:D2}:{timeSpan.Seconds:D2}";
        }
        else
        {
            _energyTimerText.text = "full"; // TODO: Localization
        }
    }

    private void OnSettingsClosed()
    {
        _descriptionRoller.gameObject.SetActive(true);
        _settingsButton.SetActive(true);
    }
    
    
    
    #endregion
}
