using UnityEngine;
using UnityEngine.UI;

public class StatBodyController : MonoBehaviour
{
    [SerializeField] private Image targetImage;
    [SerializeField] private Sprite[] sprites;

    public void SetBodyImage(int stat) // 0 - question mark
    {
        if(stat == 0)
        {
            targetImage.sprite = sprites[^1];
            return;
        }

        stat = stat < 0 ? 0 : 1;
        targetImage.sprite = sprites[stat];
    }
}
