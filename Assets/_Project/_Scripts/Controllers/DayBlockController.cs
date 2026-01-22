using System;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class DayBlockController : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _caption;
    [SerializeField] private TextMeshProUGUI _picName;
    [SerializeField] private RectTransform   _cardRotation;
    [SerializeField] private Image           _image;
    [SerializeField] private ChoiceController[] _choices;
    [SerializeField] private Transform       _choicesContainer;

    private float _choicesContainerLocalPosY;
    
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

    public void ShowFrom(bool fromLeft, Action callback = null)
    {
        GameManager.Instance.DisableTouches();
        _cardRotation.DOKill();
        _cardRotation.rotation = Quaternion.Euler(0f, 0f, fromLeft ? 90f : -90f);
        _cardRotation.DORotate(new Vector3(0, 0, 0), GameConstants.CardRotateDuration).OnComplete(() =>
        {
            _caption.gameObject.SetActive(true);
            GameManager.Instance.EnableTouches();
            callback?.Invoke();
        });
        
        _choicesContainer.DOKill();
        _choicesContainer.DOLocalMoveY(_choicesContainerLocalPosY, GameConstants.CardRotateDuration / 2f);
    }

    public void HideTo(bool toLeft, Action callback = null)
    {
        _caption.gameObject.SetActive(false);
        
        GameManager.Instance.DisableTouches();
        _cardRotation.DOKill();
        _cardRotation.DORotate(new Vector3(0, 0, toLeft ? 90f : -90f), GameConstants.CardRotateDuration).OnComplete(() =>
        {
            GameManager.Instance.EnableTouches();
            callback?.Invoke();
        });

        _choicesContainer.DOKill();
        _choicesContainer.DOLocalMoveY(-1500f, GameConstants.CardRotateDuration / 2f);
    }

    public void OpenChoicesArrows()
    {
        foreach (var choiceController in _choices)
        {
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
        _caption.gameObject.SetActive(false);
        _choicesContainerLocalPosY = _choicesContainer.localPosition.y;
        _cardRotation.rotation = Quaternion.Euler(0f, 0f, 90f);
        
        _choicesContainer.Translate(0,-1500f,0);
    }

    private void InitChoices(EventData eventData)
    {
        bool canBeQuestion = Random.value < GameConstants.FirstQuestionProbability;
        
        // отключить второй вариант выбора для событий с одним выбором
        _choices[1].gameObject.SetActive(eventData.choices.Length != 1);

        for (int i = 0; i < eventData.choices.Length && i < _choices.Length; i++)
        {
            _choices[i].InitWithStats(eventData.choices[i], canBeQuestion);
        }
    }

    #endregion
}
