using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DayBlockController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _caption;
    [SerializeField] private TextMeshProUGUI _picName;
    [SerializeField] private RectTransform   _cardRotation;
    [SerializeField] private Image           _image;
    [SerializeField] private ChoiceController[] _choices;

    private Vector3 _choice1localPos;
    private Vector3 _choice2localPos;
    
    private void Awake()
    {
        SetStartPositions();
    }

    #region Public methods

    public void InitWithEventData(EventData eventData)
    {
        _caption.text = eventData.description;
        _picName.text = eventData.name;
        eventData.LoadImageAsync(_image);
        InitChoices(eventData);
    }

    public void ShowFromLeft(Action callback = null)
    {
        GameManager.Instance.DisableTouches();
        _cardRotation.DOKill();
        _cardRotation.rotation = Quaternion.Euler(0f, 0f, 90f);
        _cardRotation.DORotate(new Vector3(0, 0, 0), GameConstants.CardRotateDuration).OnComplete(() =>
        {
            GameManager.Instance.EnableTouches();
            callback?.Invoke();
        });
        
        _choices[0].transform.DOKill();
        _choices[0].transform.DOLocalMove(_choice1localPos, GameConstants.CardRotateDuration);
        _choices[1].transform.DOKill();
        _choices[1].transform.DOLocalMove(_choice2localPos, GameConstants.CardRotateDuration);
    }

    public void HideTo(bool toLeft, Action callback = null)
    {
        GameManager.Instance.DisableTouches();
        _cardRotation.DOKill();
        _cardRotation.DORotate(new Vector3(0, 0, toLeft ? 90f : -90f), GameConstants.CardRotateDuration).OnComplete(() =>
        {
            GameManager.Instance.EnableTouches();
            callback?.Invoke();
        });
        
        _choices[0].transform.DOKill();
        _choices[0].transform.DOLocalMoveY(-1500f, GameConstants.CardRotateDuration);
        _choices[1].transform.DOKill();
        _choices[1].transform.DOLocalMoveY(-1500f, GameConstants.CardRotateDuration);
    }
    
    #endregion

    #region Private methods

    private void SetStartPositions()
    {
        _choice1localPos = _choices[0].transform.localPosition;
        _choice2localPos = _choices[1].transform.localPosition;
        _cardRotation.rotation = Quaternion.Euler(0f, 0f, 90f);
        
        _choices[0].transform.Translate(0,-1500f,0);
        _choices[1].transform.Translate(0,-1500f,0);
    }

    private void InitChoices(EventData eventData)
    {
        for (int i = 0; i < eventData.choices.Length && i < _choices.Length; i++)
        {
            _choices[i].InitWithStats(eventData.choices[i]);
        }
    }

    #endregion
}
