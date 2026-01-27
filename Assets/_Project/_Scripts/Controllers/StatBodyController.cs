using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBodyController : MonoBehaviour
{
    private const string kQuestionMarkSpriteName = "<sprite=0>";
    private const string kArrowUpSpriteName = "<sprite=3>";
    private const string kArrowDownSpriteName = "<sprite=2>";
    private const string kMinusSpriteName = "<sprite=7>";
    private const string kPlusSpriteName = "<sprite=8>";
    
    [SerializeField] private RollingTextAnimator _statText;

    public void SetStatArrow(int stat)
    {
        if (stat == 0)
        {
            _statText.ChangeTextQuick(kQuestionMarkSpriteName);
        }
        else if (stat > 0)
        {
            _statText.ChangeTextQuick(kArrowUpSpriteName);
        }
        else
        {
            _statText.ChangeTextQuick(kArrowDownSpriteName);
        }
    }
    
    public void OpenStatNumber(int stat)
    {
        if (stat > 0)
        {
            _statText.ChangeText($"<sprite=8>{stat}", true);
        }
        else
            _statText.ChangeText($"<sprite=7>{-stat}", false);
    }
    
    public void OpenStatNumberQuick(int stat)
    {
        if (stat > 0)
        {
            _statText.ChangeTextQuick($"<sprite=8>{stat}");
        }
        else
            _statText.ChangeTextQuick($"<sprite=7>{-stat}");
    }
}
