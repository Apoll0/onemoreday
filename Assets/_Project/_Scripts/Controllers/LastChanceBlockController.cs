using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LastChanceBlockController : MonoBehaviour
{
    private const float kCardsAngle = 60f;
    
    public static event Action OnOneMoreDayTriggered;
    public static event Action OnNoThanksTriggered;
    
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

    public void ShowFrom(bool fromLeft, Action callback = null)
    {
        GameManager.Instance.DisableTouches();
        _cardRotation.DOKill();
        _cardRotation.rotation = Quaternion.Euler(0f, 0f, fromLeft ? kCardsAngle : -kCardsAngle);
        _cardRotation.DORotate(new Vector3(0, 0, 0), GameConstants.CardRotateDuration).OnComplete(() =>
        {
            GameManager.Instance.EnableTouches();
            callback?.Invoke();
            StartVideoButtonAnimation();
        });
        
        _buttonsBlock.transform.DOLocalMove(_buttonsBlockLocalPos, GameConstants.ChoicesAppearDuration);
    }

    public void HideTo(bool toLeft, Action callback = null)
    {
        GameManager.Instance.DisableTouches();
        _cardRotation.DOKill();
        _cardRotation.DORotate(new Vector3(0, 0, toLeft ? kCardsAngle : -kCardsAngle), GameConstants.CardRotateDuration).OnComplete(() =>
        {
            GameManager.Instance.EnableTouches();
            callback?.Invoke();
        });
        
        _buttonsBlock.transform.DOLocalMoveY(-1500f, GameConstants.ChoicesAppearDuration).OnComplete(() =>
        {
            //_caption.gameObject.SetActive(false);
        });
    }
    
    #endregion

    #region Private methods

    private void SetStartPositions()
    {
        _buttonsBlockLocalPos = _buttonsBlock.localPosition;
        _cardRotation.rotation = Quaternion.Euler(0f, 0f, kCardsAngle);
        
        _buttonsBlock.transform.Translate(new Vector3(0, -1500f, 0f));
        //_caption.gameObject.SetActive(false);
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
        
        // TODO: Show video
        OnOneMoreDayTriggered?.Invoke(); // TODO: Предусмотреть отказ от просмотра видео
    }

    public void NoThanksPressed()
    {
        _buttonAnimationTween?.Kill();
        OnNoThanksTriggered?.Invoke();
    }
    
    #endregion
}
