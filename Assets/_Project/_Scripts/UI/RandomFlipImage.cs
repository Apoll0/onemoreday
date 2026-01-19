using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RandomFlipImage : MonoBehaviour
{
    private RectTransform _rectTransform;

    private void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
    }

    private void OnEnable()
    {
        ApplyRandomFlip();
    }

    /// <summary>
    /// Случайно инвертирует изображение по горизонтали и/или вертикали
    /// </summary>
    public void ApplyRandomFlip()
    {
        bool flipX = Random.value > 0.5f;
        bool flipY = Random.value > 0.5f;

        Vector3 scale = _rectTransform.localScale;
        scale.x = Mathf.Abs(scale.x) * (flipX ? -1f : 1f);
        scale.y = Mathf.Abs(scale.y) * (flipY ? -1f : 1f);

        _rectTransform.localScale = scale;
    }
}