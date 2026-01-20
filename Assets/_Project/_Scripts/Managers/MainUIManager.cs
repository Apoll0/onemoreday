using System;
using TMPro;
using UnityEngine;

public class MainUIManager : MonoBehaviour
{
    #region Exposed variables

    [SerializeField] private GameObject _viewsContainer;
    [SerializeField] private GameObject _startView;
    [SerializeField] private TextMeshProUGUI _dynamicCaption;
    [SerializeField] private DayBlockController _dayBlockController;

    #endregion

    private void Start()
    {
        _dynamicCaption.text = "one more day"; // TODO: Localization
    }

    #region Button callbacks

    public void StartButtonPressed()
    {
        GameManager.Instance.StartGame();
        _startView.SetActive(false);
    }

    public void ShowNewDay(EventData eventData, Action callback = null)
    {
        _dynamicCaption.text = "day " + DataManager.Instance.CurrentDay; // TODO: Localization
        _dayBlockController.InitWithEventData(eventData);
        _dayBlockController.ShowFromLeft(callback);
    }
    
    public void SettingsButtonPressed()
    {
        // TODO: Implement settings UI
    }

    public void HideDay(bool toLeft, Action callback = null)
    {
        _dayBlockController.HideTo(toLeft, callback);
    }
    
    public void ShowNewDay(int dayNumber)
    {
        
    }
    
    #endregion
}
