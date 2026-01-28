using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using Lofelt.NiceVibrations;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public enum ButtonExtState 
{
	Normal = 0,
	Highlighted,
	Disabled,
}

public class ButtonExt : MonoBehaviour,
						 IPointerDownHandler,
						 IPointerUpHandler,
						 IPointerEnterHandler,
						 IPointerExitHandler,
						 IPointerClickHandler
{
	#region Exposed vars
	
	[SerializeField] protected ButtonExtState _state = ButtonExtState.Normal;
	[FormerlySerializedAs("_contentTransform")] [SerializeField] protected RectTransform _movedContentTransform;
	[SerializeField] private Image _imgButtonBg;
	[SerializeField] private Image _customButtonImage;
	[SerializeField] private Image _imgIcon;
	[SerializeField] private float _highlightedStateDelay;
	[SerializeField] private MaskableGraphic[] _maskable;
	[SerializeField] private States		_states;
	[SerializeField] private bool		_useCustomAudioData;
	//[SerializeField] private SoundData  _customAudioData;
	
	[Header("Badge")]
	[SerializeField] private GameObject _badge;
	[SerializeField] private GameObject _alternativeBadge;
	[SerializeField] protected TextMeshProUGUI _lblBadgeValue;
	[SerializeField] protected TextMeshProUGUI _alternativeLblBadgeValue;

	[Header("Double click")] 
	[SerializeField] private float _newClickDelay;
	
	
	#endregion
	
	[Serializable] private class StateData
	{
		public Sprite spriteBg;
		public Sprite spriteIcon;
		public Vector2 contentOffset;
		public Color[] colors;
		public float contentScale = 1.0f;
		public float animationTime = 0.2f;
		public Ease animationEase = Ease.Linear;
	}
	
	[Serializable] private class States
	{
		public StateData normal;
		public StateData highlighted;
		public StateData disabled;

		public StateData[] states => new[] { normal, highlighted, disabled };
	}

	#region Private vars
	
	private bool _pressed;
	private bool _doHighlight;
	private RectTransform _rectTransform;
	private RectTransform _iconTransform;
	private float _defaultScale = 1f;
	
	#endregion

	#region Properties

	public RectTransform rectTransform
	{
		get
		{
			if (_rectTransform == null)
				_rectTransform = GetComponent<RectTransform>();
			return _rectTransform;
		}
	}

	public RectTransform iconTransform
	{
		get
		{
			if (_iconTransform == null)
				_iconTransform = _imgIcon.GetComponent<RectTransform>();
			return _iconTransform;
		}
	}


	public RectTransform content { get => _movedContentTransform; set => _movedContentTransform = value; }
	public Image icon => _imgIcon;
	public Image background => _imgButtonBg;

	public ButtonExtState state
	{
		get => _state;
		set => UpdateState(value);
	}
	
	#endregion

	#region Init
	
	private void Start()
	{
		if(_movedContentTransform != null)
			_defaultScale = _movedContentTransform.localScale.x;
		UpdateState();
	}

	private void OnDestroy()
	{
		_movedContentTransform.DOKill();
	}

	#endregion

	public void AnimateBadge()
	{
		if (_badge == null || !_badge.activeSelf)
			return;
		
		_badge.transform.DOScale(1.0f, 0.3f).From(1.5f).SetEase(Ease.OutBack);
	}

	public void SetBadgeActive(bool active, string value = null, bool animated = false)
	{
		if (_badge == null)
			return;

		if (_lblBadgeValue != null)
			_lblBadgeValue.text = value;

		_badge.transform.DOKill(true);

		if (_badge.activeSelf == active)
			return;
		
		if (animated)
		{
			if (active)
			{
				_badge.SetActive(true);
				_badge.transform.DOScale(1.0f, 0.2f).From(0.0f).SetEase(Ease.OutBack);
			}
			else
			{
				_badge.transform.DOScale(0.0f, 0.2f).From(0.0f).SetEase(Ease.InBack).OnComplete(() => _badge.SetActive(false));
			}
		}
		else
		{
			_badge.transform.localScale = Vector3.one;
			_badge.SetActive(active);
		}
	}

	public void SetAlternativeBadge(bool isAlternative, string value = null)
	{
		if (isAlternative)
		{
			if (_badge != null && _alternativeBadge != null)
			{
				_badge.SetActive(false);
				_alternativeBadge.SetActive(true);
				if(value != null && _alternativeLblBadgeValue != null)
					_alternativeLblBadgeValue.text = value;
			}	
		}
		else
		{
			if (_badge != null)
			{
				if(_alternativeBadge != null)
					_alternativeBadge.SetActive(false);
				_badge.SetActive(true);
			}
		}
		
	}
	
	protected void UpdateState(ButtonExtState value)
	{
		_state = value;
		if (_state == ButtonExtState.Disabled)
		{
			if (_pressed)
				CancelInvoke(nameof(Highlight));
			_pressed = false;
		}
		UpdateState();
	}

	protected virtual void UpdateState()
	{
		if (_customButtonImage != null)
			_customButtonImage.raycastTarget = (_state != ButtonExtState.Disabled);
		else
		{
			var img = GetComponent<Image>();
			if (img != null)
				img.raycastTarget = (_state != ButtonExtState.Disabled);
		}
		GetComponent<Button>().interactable = (_state != ButtonExtState.Disabled);

		int stateIndex = (int)_state;
		if (_imgButtonBg != null)
		{
			if (_states.states[stateIndex].spriteBg != null)
				_imgButtonBg.sprite = _states.states[stateIndex].spriteBg;
		}

		for (int i = 0; i < _maskable.Length; i++)
		{
			if (i < _states.states[stateIndex].colors.Length)
				_maskable[i].color = _states.states[stateIndex].colors[i];
		}
		
		if (_imgIcon != null)
		{
			if (_states.states[stateIndex].spriteIcon != null)
				_imgIcon.sprite = _states.states[stateIndex].spriteIcon;
		}
		
		if (_movedContentTransform != null)
		{
			_movedContentTransform.anchoredPosition = _states.states[stateIndex].contentOffset;
			_movedContentTransform.DOKill(true);
			if (_states.states[stateIndex].animationTime > 0.0f)
				_movedContentTransform.DOScale(_states.states[stateIndex].contentScale * _defaultScale, _states.states[stateIndex].animationTime).SetEase(_states.states[stateIndex].animationEase);
			else
				_movedContentTransform.SetScale(_states.states[stateIndex].contentScale * _defaultScale);
		}
	}

	private void Highlight()
	{
		state = ButtonExtState.Highlighted;
		if(!_useCustomAudioData)
			SoundManager.Instance.PlaySound(AudioData.ButtonClickSound);
		//SoundManager.Instance.PlayAudioClip(_useCustomAudioData ? _customAudioData : SoundsData.ButtonClick);
		HapticPatterns.PlayPreset(HapticPatterns.PresetType.LightImpact);
		_doHighlight = false;
	}

	private void Unhighlight()
	{
		if (state == ButtonExtState.Highlighted)
			state = ButtonExtState.Normal;
	}

	private IEnumerator WaitNextFrame(Action action)
	{
		yield return new WaitForEndOfFrame();
		action?.Invoke();
	}
	
	public void OnPointerDown(PointerEventData evd)
	{
		if (state == ButtonExtState.Disabled || state == ButtonExtState.Highlighted)
			return;
		
		_pressed = true;
		_doHighlight = _highlightedStateDelay > 0.0f;
		if (_doHighlight)
			Invoke(nameof(Highlight), _highlightedStateDelay);
		else
			Highlight();
	}

	public void OnPointerUp(PointerEventData evd)
	{
		if (_pressed)
			CancelInvoke(nameof(Highlight));
		
		_pressed = false;
		if (state == ButtonExtState.Disabled || state == ButtonExtState.Normal)
			return;

		if(_newClickDelay > 0.0f)
		{
			StartCoroutine(WaitNextFrame(() =>
			{
				if (state != ButtonExtState.Disabled)
				{
					state = ButtonExtState.Disabled;
					DOVirtual.DelayedCall(_newClickDelay, (() =>
					{
						if(this != null)
							state = ButtonExtState.Normal;
					}), false);	
				}
			}));
		}
		else
			state = ButtonExtState.Normal;
	}

	public void OnPointerEnter(PointerEventData evd)
	{
		if (!_pressed || state == ButtonExtState.Disabled || state == ButtonExtState.Highlighted)
			return;
		
		state = ButtonExtState.Highlighted;
	}

	public void OnPointerExit(PointerEventData evd)
	{
		if (!_pressed || state == ButtonExtState.Disabled || state == ButtonExtState.Normal)
			return;
		
		state = ButtonExtState.Normal;
	}

	public void OnPointerClick(PointerEventData evd)
	{
		if (_doHighlight)
		{
			Highlight();
			Invoke(nameof(Unhighlight), 0.1f);
		}
	}
}
