using System;
using TMPro;
using UnityEngine;

public class StatBlockController : MonoBehaviour
{
    [SerializeField] private StatType _statType;
    [SerializeField] private TextMeshProUGUI _statText;

    private int _currentStatValue;

    #region Init

    private void OnEnable()
    {
        DataManager.OnStatChanged += OnStatChanged;
    }

    private void OnDisable()
    {
        DataManager.OnStatChanged -= OnStatChanged;
    }

    #endregion
    
    #region Private Methods

    private void OnStatChanged(StatType statType)
    {
        if (statType == _statType)
        {
            _currentStatValue = DataManager.Instance.GetStat(_statType);
            _statText.text = _currentStatValue.ToString(); // TODO: Add animation
        }
    }

    #endregion
}
