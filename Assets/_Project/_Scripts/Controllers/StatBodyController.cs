using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBodyController : MonoBehaviour
{
    private const string kQuestionMarkSpriteName = "<sprite=0>";
    private const string kArrowUpSpriteName = "<sprite=3>";
    private const string kArrowDownSpriteName = "<sprite=2>";
    
    //[SerializeField] private Sprite[] sprites;
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
            _statText.ChangeText($"+{stat}", true);
        else
            _statText.ChangeText(stat.ToString(), false);
    }
    
    public void OpenStatNumberQuick(int stat)
    {
        if (stat > 0)
            _statText.ChangeTextQuick($"+{stat}");
        else
            _statText.ChangeTextQuick(stat.ToString());
    }
    
    /*public void SetBodyImage(int stat) // 0 - question mark
    {
        _statText.gameObject.SetActive(false);
        targetImage.gameObject.SetActive(true);
        if(stat == 0)
        {
            targetImage.sprite = sprites[^1];
            return;
        }

        stat = stat < 0 ? 0 : 1;
        targetImage.sprite = sprites[stat];
    }
    
    public void ShowStatText(int stat)
    {
        targetImage.gameObject.SetActive(false);
        _statText.gameObject.SetActive(true);
        if(stat > 0)
            _statText.text = $"+{stat}";
        else
            _statText.text = stat.ToString();
    }*/
}
