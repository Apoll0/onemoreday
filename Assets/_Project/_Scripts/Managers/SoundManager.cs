using UnityEngine;

public class SoundManager : HMSingleton<SoundManager>
{
    private const string kSoundEnabledPref = "SoundEnabled";
    private const string kMusicEnabledPref = "MusicEnabled";

    #region Private vars

    private bool _initialized;
    private AudioSource _musicSource1;
    private AudioSource _musicSource2;
    private AudioSource _soundSource;

    #endregion
    
    public bool SoundEnabled
    {
        get => SecurePlayerPrefs.GetBool(kSoundEnabledPref);
        set
        {
            SecurePlayerPrefs.SetBool(kSoundEnabledPref, value);
            SecurePlayerPrefs.Save();
        }
    }
    
    public bool MusicEnabled
    {
        get => SecurePlayerPrefs.GetBool(kMusicEnabledPref);
        set
        {
            SecurePlayerPrefs.SetBool(kMusicEnabledPref, value);
            SecurePlayerPrefs.Save();
        }
    }

    public void Initialize()
    {
        if (_initialized) 
            return;
        
        if (!SecurePlayerPrefs.HasKey(kSoundEnabledPref))
            SoundEnabled = true;
        
        if (!SecurePlayerPrefs.HasKey(kMusicEnabledPref))
            MusicEnabled = true;
        
        _musicSource1 = gameObject.AddComponent<AudioSource>();
        _musicSource1.loop = true;
        _musicSource2 = gameObject.AddComponent<AudioSource>();
        _musicSource2.loop = true;
        
        _soundSource = gameObject.AddComponent<AudioSource>();
        
        _initialized = true;
    }

    public void PlaySound(AudioClip clip)
    {
        if(SoundEnabled)
            _soundSource.PlayOneShot(clip);
    }

    #region Music

    public void PlayMusics(AudioClip clip1, AudioClip clip2)
    {
        if (clip1 != null)
        {
            _musicSource1.clip = clip1;
            if (MusicEnabled)
                _musicSource1.Play();
        }
        
        if (clip2 != null)
        {
            _musicSource2.clip = clip2;
            if (MusicEnabled)
                _musicSource2.Play();
        }
    }
    
    public void StopMusics()
    {
        _musicSource1.Stop();
        _musicSource2.Stop();
    }

    public void PauseMusics(bool isPaused)
    {
        if (isPaused)
        {
            _musicSource1.Pause();
            _musicSource2.Pause();
        }
        else
        {
            _musicSource1.UnPause();
            _musicSource2.UnPause();
        }
    }

    #endregion
}
