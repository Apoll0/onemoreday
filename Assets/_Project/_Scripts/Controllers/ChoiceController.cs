using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChoiceController : MonoBehaviour
{
    public static event Action<ChoiceController> OnMadeChoice;
    
    [SerializeField] private int _choiceIndex;
    [SerializeField] private TextMeshProUGUI _choiceName;
    [SerializeField] private StatBodyController _statObjectBody;
    [SerializeField] private StatBodyController _statObjectMind;
    [SerializeField] private StatBodyController _statObjectSupp;
    [SerializeField] private StatBodyController _statObjectHope;

    public int ChoiceIndex => _choiceIndex;
    
    private Choice _currentChoiceStats;

    #region Init

    private void OnEnable()
    {
        GameManager.OnNeedToOpenArrows += OpenArrows;
    }

    private void OnDisable()
    {
        GameManager.OnNeedToOpenArrows -= OpenArrows;
    }
    
    #endregion
    
    public void InitWithStats(Choice choiceStats)
    {
        _currentChoiceStats = choiceStats;
        _choiceName.text = choiceStats.text;
        
        _statObjectMind.gameObject.SetActive(false);
        _statObjectBody.gameObject.SetActive(false);
        _statObjectSupp.gameObject.SetActive(false);
        _statObjectHope.gameObject.SetActive(false);
        
        if (choiceStats.mindEffect != 0)
        {
            _statObjectMind.gameObject.SetActive(true);
            _statObjectMind.SetBodyImage(0);
        }
        if (choiceStats.bodyEffect != 0)
        {
            _statObjectBody.gameObject.SetActive(true);
            _statObjectBody.SetBodyImage(0);
        }
        if (choiceStats.suppliesEffect != 0)
        {
            _statObjectSupp.gameObject.SetActive(true);
            _statObjectSupp.SetBodyImage(0);
        }
        if (choiceStats.hopeEffect != 0)
        {
            _statObjectHope.gameObject.SetActive(true);
            _statObjectHope.SetBodyImage(0);
        }
    }

    public void OpenArrows()
    {
        _statObjectBody.SetBodyImage(_currentChoiceStats.bodyEffect);
        _statObjectMind.SetBodyImage(_currentChoiceStats.mindEffect);
        _statObjectSupp.SetBodyImage(_currentChoiceStats.suppliesEffect);
        _statObjectHope.SetBodyImage(_currentChoiceStats.hopeEffect);
    }
    
    public void OnPressedChoice()
    {
        OnMadeChoice?.Invoke(this);
    }
    
}
