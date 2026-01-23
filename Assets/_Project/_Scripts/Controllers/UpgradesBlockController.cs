using System;
using System.Collections;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class UpgradesBlockController : MonoBehaviour
{
    public static event Action OnUpgradeApplyed;
    
    [SerializeField] private TextMeshProUGUI _caption;
    [SerializeField] private RectTransform[]   _cardRotations;
    [SerializeField] private TextMeshProUGUI[] _picNames;
    [SerializeField] private Image[]           _images;
    [SerializeField] private TextMeshProUGUI[] _descriptions;
    [SerializeField] private StatGroupController[] _statGroupControllers;
    [SerializeField] private Transform _refreshButtonContainer;

    private EventData[] _upgrades = new EventData[2];
    private float _refreshButtonContainerLocalPosY;
    
    private void Awake()
    {
        SetStartPositions();
    }

    #region Public methods

    public void Show(Action callback = null)
    {
        InitWithUpgrades();
        GameManager.Instance.DisableTouches();

        for (int i = 0; i < _cardRotations.Length; i++)
        {
            _cardRotations[i].DOKill();
            _cardRotations[i].rotation = Quaternion.Euler(0f, 0f, i == 0 ? 90f : -90f);
            
            bool lastIteration = i == _cardRotations.Length - 1;
            _cardRotations[i].DORotate(new Vector3(0, 0, 0), GameConstants.CardRotateDuration).OnComplete(() =>
            {
                _caption.gameObject.SetActive(true);
                if (lastIteration)
                {
                    GameManager.Instance.EnableTouches();
                    callback?.Invoke();
                }
            });
        }

        _refreshButtonContainer.DOKill();
        _refreshButtonContainer.DOLocalMoveY(_refreshButtonContainerLocalPosY, GameConstants.CardRotateDuration / 2f);
    }

    public void Hide(Action callback = null)
    {
        _caption.gameObject.SetActive(false);
        
        GameManager.Instance.DisableTouches();

        for (int i = 0; i < _cardRotations.Length; i++)
        {
            _cardRotations[i].DOKill();
            bool lastIteration = i == _cardRotations.Length - 1;
            _cardRotations[i].DORotate(new Vector3(0,0, i == 0 ? 90f : -90f), GameConstants.CardRotateDuration).OnComplete(() =>
            {
                if (lastIteration)
                {
                    GameManager.Instance.EnableTouches();
                    callback?.Invoke();
                }
            });
        }

        _refreshButtonContainer.DOKill();
        _refreshButtonContainer.DOLocalMoveY(-1500f, GameConstants.CardRotateDuration / 2f);
    }

    #endregion

    public void RefreshPressed()
    {
        // TODO: Show Ads
        InitWithUpgrades();
    }

    public void UpgradePressed(int index)
    {
        StartCoroutine(ApplyUpgrades(index));
    }

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
            // TODO: Run new game
        });
    }
    
    private void InitWithUpgrades()
    {
        // В данном случае апгрейды всегда парные: левый и правый
        _upgrades[0] = UpgradesData.Instance.GetRandomBodyUpgrade();
        _upgrades[1] = UpgradesData.Instance.GetRandomMindUpgrade();
        
        for (int i = 0; i < _upgrades.Length; i++)
        {
            var upgrade = _upgrades[i];
            _picNames[i].text = upgrade.name;
            upgrade.LoadImageAsync("Upgrades/", _images[i]);
            _descriptions[i].text = upgrade.description;

            var stats = upgrade.choices[0]; // Всегда берем первый, в апгрейдах всегда один выбор
            _statGroupControllers[i].SetStat(StatType.Body, stats.bodyEffect);
            _statGroupControllers[i].SetStat(StatType.Mind, stats.mindEffect);
            _statGroupControllers[i].SetStat(StatType.Supplies, stats.suppliesEffect);
            _statGroupControllers[i].SetStat(StatType.Hope, stats.hopeEffect);
        }
    }

    private void SetStartPositions()
    {
        _caption.gameObject.SetActive(false);
        _refreshButtonContainerLocalPosY = _refreshButtonContainer.localPosition.y;
        for (int i = 0; i < _cardRotations.Length; i++)
        {
            _cardRotations[i].rotation = Quaternion.Euler(0f, 0f, i == 0 ? 90f : -90f);
        }
        
        _refreshButtonContainer.Translate(0,-1500f,0);
    }

    #endregion
}
