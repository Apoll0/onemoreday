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
    
    private Choice _currentChoiceStats;

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
            _statObjectMind.SetBodyImage(canBeQuestion ? ConvertStatWithProbability(choiceStats.mindEffect) : choiceStats.mindEffect);
        }
        if (choiceStats.bodyEffect != 0)
        {
            _statObjectBody.gameObject.SetActive(true);
            _statObjectBody.SetBodyImage(canBeQuestion ? ConvertStatWithProbability(choiceStats.bodyEffect) : choiceStats.bodyEffect);
        }
        if (choiceStats.suppliesEffect != 0)
        {
            _statObjectSupp.gameObject.SetActive(true);
            _statObjectSupp.SetBodyImage(canBeQuestion ? ConvertStatWithProbability(choiceStats.suppliesEffect) : choiceStats.suppliesEffect);
        }
        if (choiceStats.hopeEffect != 0)
        {
            _statObjectHope.gameObject.SetActive(true);
            _statObjectHope.SetBodyImage(canBeQuestion ? ConvertStatWithProbability(choiceStats.hopeEffect) : choiceStats.hopeEffect);
        }
    }

    public void OpenArrows()
    {
        _statObjectBody.ShowStatText(_currentChoiceStats.bodyEffect);
        _statObjectMind.ShowStatText(_currentChoiceStats.mindEffect);
        _statObjectSupp.ShowStatText(_currentChoiceStats.suppliesEffect);
        _statObjectHope.ShowStatText(_currentChoiceStats.hopeEffect);
    }
    
    public void OnPressedChoice()
    {
        OnMadeChoice?.Invoke(this);
    }
    
    private int ConvertStatWithProbability(int stat)
    {
        if (stat == 0) return 0;
        return Random.value < GameConstants.EveryQuestionProbability ? Mathf.RoundToInt(Mathf.Sign(stat)) : 0;
    }
}
