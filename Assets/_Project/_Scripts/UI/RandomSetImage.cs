using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class RandomSetImage : MonoBehaviour
{
    [SerializeField] private Sprite[] _sprites;
    
    private Image _img;

    private void Awake()
    {
        _img = GetComponent<Image>();
    }

    private void OnEnable()
    {
        ApplyRandomImage();
    }
    
    public void ApplyRandomImage()
    {
        _img.sprite = _sprites[Random.Range(0, _sprites.Length)];
    }
}