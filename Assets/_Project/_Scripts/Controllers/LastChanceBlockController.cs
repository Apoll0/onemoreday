using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LastChanceBlockController : MonoBehaviour
{
    public static event Action OnOneMoreDayTriggered;
    public static event Action OnNoThanksTriggered;
    
    [SerializeField] private TextMeshProUGUI _caption;
    [Space]
    //[SerializeField] private GameObject      _bodyPic;
    //[SerializeField] private GameObject      _mindPic;
    //[SerializeField] private GameObject      _supplyPic;
    //[SerializeField] private GameObject      _hopePic;
    [Space]
    [SerializeField] private RectTransform   _cardRotation;
    [SerializeField] private Image           _image;
    [SerializeField] private Transform       _buttonsBlock;
    [SerializeField] private Image           _progressBar;

    private Vector3 _buttonsBlockLocalPos;
    private Tween _buttonAnimationTween;
    
    private void Awake()
    {
        SetStartPositions();
    }

    #region Public methods

    public void InitWithStatType(StatType statType)
    {
        //_bodyPic.SetActive(false);
        //_mindPic.SetActive(false);
        //_supplyPic.SetActive(false);
        //_hopePic.SetActive(false);
        
        _caption.gameObject.SetActive(true);
        switch (statType)
        {
            case StatType.Body:
                //_bodyPic.SetActive(true);
                _caption.text = "Your body couldn’t go any further";
                break;
            case StatType.Mind:
                //_mindPic.SetActive(true);
                _caption.text = "Your mind finally broke";
                break;
            case StatType.Supplies:
                //_supplyPic.SetActive(true);
                _caption.text = "There was nothing left";
                break;
            case StatType.Hope:
                //_hopePic.SetActive(true);
                _caption.text = "You lost the will to continue";
                break;
        }
    }

    public void ShowFrom(bool fromLeft, Action callback = null)
    {
        GameManager.Instance.DisableTouches();
        _cardRotation.DOKill();
        _cardRotation.rotation = Quaternion.Euler(0f, 0f, fromLeft ? 90f : -90f);
        _cardRotation.DORotate(new Vector3(0, 0, 0), GameConstants.CardRotateDuration).OnComplete(() =>
        {
            GameManager.Instance.EnableTouches();
            callback?.Invoke();
            StartVideoButtonAnimation();
        });
        
        _buttonsBlock.transform.DOLocalMove(_buttonsBlockLocalPos, GameConstants.CardRotateDuration / 2f);
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
        
        _buttonsBlock.transform.DOLocalMoveY(-1500f, GameConstants.CardRotateDuration / 2f).OnComplete(() =>
        {
            _caption.gameObject.SetActive(false);
        });
    }
    
    #endregion

    #region Private methods

    private void SetStartPositions()
    {
        _buttonsBlockLocalPos = _buttonsBlock.localPosition;
        _cardRotation.rotation = Quaternion.Euler(0f, 0f, 90f);
        
        _buttonsBlock.transform.Translate(new Vector3(0, -1500f, 0f));
        _caption.gameObject.SetActive(false);
    }

    private void StartVideoButtonAnimation()
    {
        _buttonAnimationTween =
            _progressBar.DOFillAmount(0, GameConstants.WaitLastChanceDuration).From(1).OnComplete(TimeIsOut);
    }

    private void TimeIsOut()
    {
        NoThanksPressed();
    }
    
    #endregion

    #region Button callbacks

    public void OneMoreDayPressed()
    {
        _buttonAnimationTween?.Kill();
        // TODO: Implement one more day logic
        // Show video
        OnOneMoreDayTriggered?.Invoke(); // TODO: Предусмотреть отказ от просмотра видео
    }

    public void NoThanksPressed()
    {
        _buttonAnimationTween?.Kill();
        OnNoThanksTriggered?.Invoke();
    }
    
    #endregion
}
