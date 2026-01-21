using System;
using TMPro;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    #region Exposed variables

    [SerializeField] private GameObject _viewsContainer;
    [SerializeField] private GameObject _startView;
    [SerializeField] private TextMeshProUGUI _dynamicCaption;
    [SerializeField] private DayBlockController[] _dayBlockControllers;
    [SerializeField] private LastChanceBlockController _lastChanceBlockController; 

    #endregion

    #region Private vars

    private int _currentDayBlockIndex = 0;
    private bool _lastMoveSideToLeft = false;

    #endregion

    private void Start()
    {
        _dynamicCaption.text = "one more day"; // TODO: Localization
        
        foreach (DayBlockController dayBlockController in _dayBlockControllers)
            dayBlockController.gameObject.SetActive(true);
        _lastChanceBlockController.gameObject.SetActive(true);
        ShowStartView();
    }

    #region Button callbacks

    public void StartButtonPressed()
    {
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

    #endregion
}
