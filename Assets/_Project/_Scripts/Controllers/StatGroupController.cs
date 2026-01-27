using System;
using UnityEngine;

public class StatGroupController : MonoBehaviour
{
    [SerializeField] private StatBodyController _bodyController;
    [SerializeField] private StatBodyController _mindController;
    [SerializeField] private StatBodyController _suppliesController;
    [SerializeField] private StatBodyController _hopeController;

    public void SetStat(StatType statType, int statValue, bool quickly = false)
    {
        switch (statType)
        {
            case StatType.Body:
                _bodyController.gameObject.SetActive(statValue != 0);
                if(quickly)
                    _bodyController.OpenStatNumberQuick(statValue);
                else
                    _bodyController.OpenStatNumber(statValue);
                break;
            case StatType.Mind:
                _mindController.gameObject.SetActive(statValue != 0);
                if(quickly)
                    _mindController.OpenStatNumberQuick(statValue);
                else
                    _mindController.OpenStatNumber(statValue);
                break;
            case StatType.Supplies:
                _suppliesController.gameObject.SetActive(statValue != 0);
                if(quickly)
                    _suppliesController.OpenStatNumberQuick(statValue);
                else
                    _suppliesController.OpenStatNumber(statValue);
                break;
            case StatType.Hope:
                _hopeController.gameObject.SetActive(statValue != 0);
                if(quickly)
                    _hopeController.OpenStatNumberQuick(statValue);
                else
                    _hopeController.OpenStatNumber(statValue);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(statType), statType, null);
        }
        
    }
}
