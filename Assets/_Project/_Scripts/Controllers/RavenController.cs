using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class RavenController : MonoBehaviour
{
    private static readonly int kAnimatorFly = Animator.StringToHash("Fly");
    [SerializeField] private Transform _enterPoint;
    [SerializeField] private Transform _exitPoint;
    [SerializeField] private Transform _trashPoint;
    [Space]
    [SerializeField] private Transform _raven;
    [SerializeField] private Animator _animator;
    
    private Sequence _flySequence;
    
    [Button(enabledMode:EButtonEnableMode.Playmode)]
    public void FlyIn()
    {
        _flySequence?.Kill();
        
        _animator.enabled = true;
        _raven.position = _enterPoint.position;
        _animator.SetBool(kAnimatorFly, true);

        SoundManager.Instance.PlaySound(AudioData.RavenSitSound);
        _flySequence = DOTween.Sequence();
        _flySequence.Append(_raven.DOMove(_trashPoint.position, 0.7f).SetEase(Ease.OutQuad));
        //_flySequence.AppendCallback(() => SoundManager.Instance.PlaySound(AudioData.RavenSitSound));
        _flySequence.AppendInterval(0.3f);
        _flySequence.AppendCallback(() => _animator.SetBool(kAnimatorFly, false));
        
        //_raven.DOMove(_trashPoint.position, 0.7f).OnComplete(() => _animator.SetBool(kAnimatorFly, false));
    }
    
    [Button(enabledMode:EButtonEnableMode.Playmode)]
    public void FlyOut()
    {
        _flySequence?.Kill();
        
        SoundManager.Instance.PlaySound(AudioData.RavenFlyAwaySound);
        _raven.position = _trashPoint.position;
        _animator.SetBool(kAnimatorFly, true);
        _raven.DOMove(_exitPoint.position, 1.5f).OnComplete(() => _animator.enabled = false);
    }
}
