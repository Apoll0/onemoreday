using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using Random = UnityEngine.Random;

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
    public bool isRandomChoice => _currentChoiceStats.isRandom2;
    
    private Choice _currentChoiceStats = new ();

    #region Init
    
    
    #endregion
    
    public void InitWithStats(Choice choiceStats, bool canBeQuestion)
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
            _statObjectMind.SetStatArrow(canBeQuestion ? ConvertStatWithProbability(choiceStats.mindEffect) : choiceStats.mindEffect);
        }
        if (choiceStats.bodyEffect != 0)
        {
            _statObjectBody.gameObject.SetActive(true);
            _statObjectBody.SetStatArrow(canBeQuestion ? ConvertStatWithProbability(choiceStats.bodyEffect) : choiceStats.bodyEffect);
        }
        if (choiceStats.suppliesEffect != 0)
        {
            _statObjectSupp.gameObject.SetActive(true);
            _statObjectSupp.SetStatArrow(canBeQuestion ? ConvertStatWithProbability(choiceStats.suppliesEffect) : choiceStats.suppliesEffect);
        }
        if (choiceStats.hopeEffect != 0)
        {
            _statObjectHope.gameObject.SetActive(true);
            _statObjectHope.SetStatArrow(canBeQuestion ? ConvertStatWithProbability(choiceStats.hopeEffect) : choiceStats.hopeEffect);
        }
    }

    public void OpenArrows()
    {
        if(_statObjectBody.gameObject.activeSelf)
            _statObjectBody.OpenStatNumber(_currentChoiceStats.bodyEffect);
        
        if (_statObjectMind.gameObject.activeSelf)
            _statObjectMind.OpenStatNumber(_currentChoiceStats.mindEffect);
        
        if(_statObjectSupp.gameObject.activeSelf)
            _statObjectSupp.OpenStatNumber(_currentChoiceStats.suppliesEffect);
        
        if (_statObjectHope.gameObject.activeSelf)
            _statObjectHope.OpenStatNumber(_currentChoiceStats.hopeEffect);
    }
    
    public void OpenRandomStatArrow(StatType statType, int value)
    {
        switch (statType)
        {
            case StatType.Body:
                _statObjectBody.gameObject.SetActive(true);
                _statObjectBody.OpenStatNumber(value);
                break;
            case StatType.Mind:
                _statObjectMind.gameObject.SetActive(true);
                _statObjectMind.OpenStatNumber(value);
                break;
            case StatType.Supplies:
                _statObjectSupp.gameObject.SetActive(true);
                _statObjectSupp.OpenStatNumber(value);
                break;
            case StatType.Hope:
                _statObjectHope.gameObject.SetActive(true);
                _statObjectHope.OpenStatNumber(value);
                break;
        }
    }
    
    public void OnPressedChoice()
    {
        OnMadeChoice?.Invoke(this);
    }
    
    private int ConvertStatWithProbability(int stat)
    {
        if (stat == 0) 
            return 0;
        
        var rnd = Random.value;
        return rnd >= GameConstants.EveryQuestionProbability ? stat : 0;
    }
}
