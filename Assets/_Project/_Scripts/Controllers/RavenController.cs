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
    
    [Button(enabledMode:EButtonEnableMode.Playmode)]
    public void FlyIn()
    {
        _animator.enabled = true;
        _raven.position = _enterPoint.position;
        _animator.SetBool(kAnimatorFly, true);
        _raven.DOMove(_trashPoint.position, 0.7f).OnComplete(() => _animator.SetBool(kAnimatorFly, false));
    }
    
    [Button(enabledMode:EButtonEnableMode.Playmode)]
    public void FlyOut()
    {
        _raven.position = _trashPoint.position;
        _animator.SetBool(kAnimatorFly, true);
        _raven.DOMove(_exitPoint.position, 1.5f).OnComplete(() => _animator.enabled = false);
    }
}
