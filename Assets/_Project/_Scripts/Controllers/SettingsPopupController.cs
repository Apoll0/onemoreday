using System;
using DG.Tweening;
using Lofelt.NiceVibrations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopupController : MonoBehaviour
{
    public static event Action OnSettingsOpened;
    public static event Action OnSettingsClosed;
    
    [SerializeField] private GameObject _removeAdsButton;
    [SerializeField] private GameObject _hapticButton;
    [SerializeField] private GameObject[] _restoreButtonObjects;
    [SerializeField] private TextMeshProUGUI _versionText;
    [Space] 
    [SerializeField] private Image _musicImage;
    [SerializeField] private Image _soundImage;
    [SerializeField] private Image _hapticImage;
    [SerializeField] private Sprite _musicDisabledSprite;
    [SerializeField] private Sprite _soundDisabledSprite;
    [SerializeField] private Sprite _hapticDisabledSprite;
    [Space]
    [SerializeField] private CanvasGroup _canvasGroup;

    private void OnEnable()
    {
        // TODO: Add purchase callback to update remove ads button state
        OnSettingsOpened?.Invoke();
        _hapticButton.SetActive(DeviceCapabilities.isVersionSupported);
        
        _versionText.text = "version " + Application.version;
        _canvasGroup.alpha = 0;
        _canvasGroup.DOFade(1, 0.3f);

#if UNITY_ANDROID
        foreach (var obj in _restoreButtonObjects)
            obj.SetActive(false);
#endif
    }

    #region Button callbacks

    public void RemoveAdsButtonPressed()
    {
        // TODO: Implement remove ads functionality
    }
    
    public void ClosePressed()
    {
        _canvasGroup.DOFade(0, 0.3f).OnComplete(() =>
        {
            OnSettingsClosed?.Invoke();
            gameObject.SetActive(false);
        });
    }

    public void SoundButtonPressed()
    {
        SoundManager.Instance.SoundEnabled = !SoundManager.Instance.SoundEnabled;
        UpdateButtonsState();
    }

    public void MusicButtonPressed()
    {
        SoundManager.Instance.MusicEnabled = !SoundManager.Instance.MusicEnabled;
        UpdateButtonsState();
        
        if(SoundManager.Instance.MusicEnabled)
            SoundManager.Instance.PlayMusics(AudioData.MusicClip1, AudioData.MusicClip2);
        else
            SoundManager.Instance.StopMusics();
    }

    public void HapticButtonPressed()
    {
        HapticController.hapticsEnabled = !HapticController.hapticsEnabled;
        UpdateButtonsState();
    }

    public void LeaderboardsPressed()
    {
        // TODO: Implement leaderboards functionality
    }
    
    public void TermsPressed()
    {
        Application.OpenURL(GameConstants.TermsOfServiceURL);
    }

    public void RestorePressed()
    {
        // TODO: Implement restore purchases functionality
    }

    public void RateUasPressed()
    {
        // TODO: Implement rate us functionality
    }

    #endregion

    #region Private methods

    private void UpdateButtonsState()
    {
        _soundImage.overrideSprite = SoundManager.Instance.SoundEnabled ? null : _soundDisabledSprite;
        _musicImage.overrideSprite = SoundManager.Instance.MusicEnabled ? null : _musicDisabledSprite;
        _hapticImage.overrideSprite = HapticController.hapticsEnabled ? null : _hapticDisabledSprite;
        
        _removeAdsButton.SetActive(!DataManager.Instance.RemoveAdsPurchased);
    }

    #endregion
}
