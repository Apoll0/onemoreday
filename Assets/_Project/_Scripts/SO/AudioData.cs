using UnityEngine;

[CreateAssetMenu(fileName = "AudioData", menuName = "Scriptable Objects/AudioData")]
public class AudioData : HMScriptableSingleton<AudioData>
{
    [SerializeField] private AudioClip _musicClip1;
    [SerializeField] private AudioClip _musicClip2;
    [Space]
    [SerializeField] private AudioClip _buttonClickSound;
    [SerializeField] private AudioClip _ravenSitSound;
    [SerializeField] private AudioClip _ravenFlyAwaySound;

    #region Properties

    public static AudioClip MusicClip1 => Instance._musicClip1;
    public static AudioClip MusicClip2 => Instance._musicClip2;
    
    public static AudioClip ButtonClickSound => Instance._buttonClickSound;
    public static AudioClip RavenSitSound => Instance._ravenSitSound;
    public static AudioClip RavenFlyAwaySound => Instance._ravenFlyAwaySound;

    #endregion    
}
