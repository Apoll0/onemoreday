using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DayBlockController : MonoBehaviour
{
    private const float kCardsAngle = 60f;
    private const float kAnimationTimeDiapason = 0.5f;
    
    //[SerializeField] private TextMeshProUGUI _caption;
    [SerializeField] private TextMeshProUGUI _picName;
    [SerializeField] private RectTransform   _cardRotation;
    [SerializeField] private Image           _image;
    [SerializeField] private ChoiceController[] _choices;
    [SerializeField] private Transform[]       _choiceContainers;
    [SerializeField] private RandomFlipImage[] _randomFlipImages;

    private float _choicesContainerLocalPosY;
    
    private void Awake()
    {
        SetStartPositions();
    }

    #region Public methods

    public void InitWithEventData(EventData eventData)
    {
        _picName.text = eventData.name;
        eventData.LoadImageAsync("Ills/", _image);
        InitChoices(eventData);
    }

    public void ShowFrom(bool fromLeft, Action callback = null)
    {
        GameManager.Instance.DisableTouches();
        _cardRotation.DOKill();
        _cardRotation.rotation = Quaternion.Euler(0f, 0f, fromLeft ? kCardsAngle : -kCardsAngle);
        _cardRotation.DORotate(new Vector3(0, 0, 0), GameConstants.CardRotateDuration)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
        {
            GameManager.Instance.EnableTouches();
            callback?.Invoke();
        });

        foreach (var choiceContainer in _choiceContainers)
        {
            choiceContainer.DOKill();
            choiceContainer.DOLocalMoveY(_choicesContainerLocalPosY, GameConstants.ChoicesAppearDuration * Random.Range(1-kAnimationTimeDiapason,1+kAnimationTimeDiapason))
                .SetEase(Ease.OutCubic)
                .SetDelay(GameConstants.ChoicesAppearDuration * 0.7f);
        }
    }

    public void HideTo(bool toLeft, Action callback = null)
    {
        GameManager.Instance.DisableTouches();
        _cardRotation.DOKill();
        _cardRotation.DORotate(new Vector3(0, 0, toLeft ? kCardsAngle : -kCardsAngle), GameConstants.CardRotateDuration)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() =>
        {
            GameManager.Instance.EnableTouches();
            callback?.Invoke();
        });

        foreach (var choiceContainer in _choiceContainers)
        {
            choiceContainer.DOKill();
            choiceContainer.DOLocalMoveY(-1500f, (GameConstants.ChoicesAppearDuration / 2f) * Random.Range(1-kAnimationTimeDiapason,1+kAnimationTimeDiapason))
                .SetEase(Ease.InQuad);    
        }
    }

    public void OpenChoicesArrows()
    {
        foreach (var choiceController in _choices)
        {
            if(choiceController.gameObject.activeSelf)
                choiceController.OpenArrows();
        }
    }

    public void OpenArrowOnRandomStat(StatType choice, int value)
    {
        foreach (var choiceController in _choices)
        {
            if (choiceController.isRandomChoice)
            {
                choiceController.OpenRandomStatArrow(choice, value);
                break;
            }
        }
    }
    
    #endregion

    #region Private methods

    private void SetStartPositions()
    {
        _choicesContainerLocalPosY = _choiceContainers[0].localPosition.y;
        _cardRotation.rotation = Quaternion.Euler(0f, 0f, kCardsAngle);
        
        foreach (var choiceContainer in _choiceContainers)
            choiceContainer.Translate(0, -1500f, 0);
    }

    private void InitChoices(EventData eventData)
    {
        bool canBeQuestion = Random.value < GameConstants.FirstQuestionProbability;
        
        // отключить второй вариант выбора для событий с одним выбором
        _choices[1].gameObject.SetActive(eventData.choices.Length != 1);

        if (_choices[1].gameObject.activeSelf)
        {
            _choiceContainers[0].GetComponent<RectTransform>().SetLocalX(-286.5f); // debug value!
            _choiceContainers[1].GetComponent<RectTransform>().SetLocalX(286.5f);  // debug value!
        }
        else
        {
            _choiceContainers[0].GetComponent<RectTransform>().SetLocalX(0f);
        }

        for (int i = 0; i < eventData.choices.Length && i < _choices.Length; i++)
        {
            _choices[i].InitWithStats(eventData.choices[i], canBeQuestion);
        }

        for (int i = 0; i < _randomFlipImages.Length; i++)
            _randomFlipImages[i].ApplyRandomFlip();
    }

    #endregion
}
