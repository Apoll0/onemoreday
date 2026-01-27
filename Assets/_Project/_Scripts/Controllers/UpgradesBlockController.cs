using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UpgradesBlockController : MonoBehaviour
{
    private const float kCardsTopY = 2500f;
    
    public static event Action OnUpgradeApplyed;
    
    [FormerlySerializedAs("_cardRotations")] [SerializeField] private RectTransform[]   _cards;
    [SerializeField] private TextMeshProUGUI[] _picNames;
    [SerializeField] private Image[]           _images;
    [SerializeField] private TextMeshProUGUI[] _descriptions;
    [SerializeField] private StatGroupController[] _statGroupControllers;
    [SerializeField] private Transform _refreshButtonContainer;

    private EventData[] _upgrades = new EventData[2];
    private float _refreshButtonContainerLocalPosY;
    private float[] _cardsLocalPosY = new float[2];
    
    private void Awake()
    {
        SetStartPositions();
    }

    #region Public methods

    public void Show(Action callback = null)
    {
        InitWithUpgrades();
        GameManager.Instance.DisableTouches();

        for (int i = 0; i < _cards.Length; i++)
        {
            _cards[i].DOKill();
            
            bool lastIteration = i == _cards.Length - 1;
            _cards[i].DOLocalMoveY(_cardsLocalPosY[i], GameConstants.CardRotateDuration)
                .SetEase(Ease.OutQuad)
                .OnComplete(() =>
            {
                if (lastIteration)
                {
                    GameManager.Instance.EnableTouches();
                    callback?.Invoke();
                }
            });
        }

        _refreshButtonContainer.DOKill();
        _refreshButtonContainer.DOLocalMoveY(_refreshButtonContainerLocalPosY, GameConstants.ChoicesAppearDuration);
    }

    public void Hide(Action callback = null)
    {
        GameManager.Instance.DisableTouches();

        for (int i = 0; i < _cards.Length; i++)
        {
            _cards[i].DOKill();
            bool lastIteration = i == _cards.Length - 1;
            _cards[i].DOLocalMoveY(kCardsTopY, GameConstants.CardRotateDuration)
                .SetEase(Ease.InQuad)
                .OnComplete(() =>
            {
                if (lastIteration)
                {
                    GameManager.Instance.EnableTouches();
                    callback?.Invoke();
                }
            });
        }

        _refreshButtonContainer.DOKill();
        _refreshButtonContainer.DOLocalMoveY(-1500f, GameConstants.ChoicesAppearDuration);
    }

    #endregion

    #region Button callbacks

    public void RefreshPressed()
    {
        // TODO: Show Ads
        InitWithUpgrades();
    }

    public void UpgradePressed(int index)
    {
        StartCoroutine(ApplyUpgrades(index));
    }

    #endregion

    #region Private methods

    private IEnumerator ApplyUpgrades(int index)
    {
        GameManager.Instance.DisableTouches();
        
        DataManager.Instance.SetPersistentStat(StatType.Body, DataManager.Instance.GetPersistentStat(StatType.Body) + _upgrades[index].choices[0].bodyEffect);
        DataManager.Instance.SetPersistentStat(StatType.Mind, DataManager.Instance.GetPersistentStat(StatType.Mind) + _upgrades[index].choices[0].mindEffect);
        DataManager.Instance.SetPersistentStat(StatType.Supplies, DataManager.Instance.GetPersistentStat(StatType.Supplies) + _upgrades[index].choices[0].suppliesEffect);
        DataManager.Instance.SetPersistentStat(StatType.Hope, DataManager.Instance.GetPersistentStat(StatType.Hope) + _upgrades[index].choices[0].hopeEffect);
        
        yield return new WaitForSeconds(1f);
        Hide(() =>
        {
            GameManager.Instance.EnableTouches();
            OnUpgradeApplyed?.Invoke();
        });
    }
    
    private void InitWithUpgrades()
    {
        // В данном случае апгрейды всегда парные: левый и правый
        // Первый апгрейд случайный, но не уводящий в минус
        _upgrades[0] = UpgradesData.Instance.GetRandomUpgradeWithProbability(15f,15f,15f,15f, 15f, 5f, 15f, 5f);
        
        if(DataManager.Instance.GetPersistentStat(StatType.Body) <= 1)
            _upgrades[1] = UpgradesData.Instance.GetSafetyUpgradeByStat(StatType.Body);
        else if(DataManager.Instance.GetPersistentStat(StatType.Supplies) <= 1)
            _upgrades[1] = UpgradesData.Instance.GetSafetyUpgradeByStat(StatType.Supplies);
        else if(DataManager.Instance.GetPersistentStat(StatType.Hope) <= 1)
            _upgrades[1] = UpgradesData.Instance.GetSafetyUpgradeByStat(StatType.Hope);
        else if(DataManager.Instance.GetPersistentStat(StatType.Mind) <= 1)
            _upgrades[1] = UpgradesData.Instance.GetSafetyUpgradeByStat(StatType.Mind);
        else
            // Если все в порядке, то второй апгрейд тоже случайный
            _upgrades[1] = UpgradesData.Instance.GetRandomUpgradeWithProbability(15f,15f,15f,15f, 15f, 5f, 15f, 5f);
        
        for (int i = 0; i < _upgrades.Length; i++)
        {
            var upgrade = _upgrades[i];
            _picNames[i].text = upgrade.name;
            upgrade.LoadImageAsync("Upgrades/", _images[i]);
            _descriptions[i].text = upgrade.description;

            var stats = upgrade.choices[0]; // Всегда берем первый, в апгрейдах всегда один выбор
            _statGroupControllers[i].SetStat(StatType.Body, stats.bodyEffect, true);
            _statGroupControllers[i].SetStat(StatType.Mind, stats.mindEffect, true);
            _statGroupControllers[i].SetStat(StatType.Supplies, stats.suppliesEffect, true);
            _statGroupControllers[i].SetStat(StatType.Hope, stats.hopeEffect, true);
        }
    }

    private void SetStartPositions()
    {
        _refreshButtonContainerLocalPosY = _refreshButtonContainer.localPosition.y;
        for (int i = 0; i < _cards.Length; i++)
        {
            _cardsLocalPosY[i] = _cards[i].localPosition.y;
            _cards[i].Translate(0,kCardsTopY,0);
        }
        
        _refreshButtonContainer.Translate(0,-1500f,0);
    }

    #endregion
}
