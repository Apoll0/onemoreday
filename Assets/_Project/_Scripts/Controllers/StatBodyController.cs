using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StatBodyController : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite[] sprites;
    [SerializeField] private TextMeshProUGUI _statText;

    public void SetBodyImage(int stat) // 0 - question mark
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
    }
}
