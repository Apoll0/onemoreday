using System;
using UnityEngine;

public class StatGroupController : MonoBehaviour
{
    [SerializeField] private StatBodyController _bodyController;
    [SerializeField] private StatBodyController _mindController;
    [SerializeField] private StatBodyController _suppliesController;
    [SerializeField] private StatBodyController _hopeController;

    public void SetStat(StatType statType, int statValue)
    {
        switch (statType)
        {
            case StatType.Body:
                _bodyController.ShowStatText(statValue);
                _bodyController.gameObject.SetActive(statValue != 0);
                break;
            case StatType.Mind:
                _mindController.ShowStatText(statValue);
                _mindController.gameObject.SetActive(statValue != 0);
                break;
            case StatType.Supplies:
                _suppliesController.ShowStatText(statValue);
                _suppliesController.gameObject.SetActive(statValue != 0);
                break;
            case StatType.Hope:
                _hopeController.ShowStatText(statValue);
                _hopeController.gameObject.SetActive(statValue != 0);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(statType), statType, null);
        }
        
    }
}
