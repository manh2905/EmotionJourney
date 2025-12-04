using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager Instance;

    [Header("Audio Sources")]
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private AudioSource sfxSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void PlayMusic(AudioClip clip)
    {
        musicSource.clip = clip;
        musicSource.loop = true;
        musicSource.Play();
    }

    public void PlaySFX(AudioClip clip)
    {
        sfxSource.PlayOneShot(clip);
    }

    //public void SetMusicVolume(float volume)
    //{
    //    musicSource.volume = volume;
    //}

    //public void SetSFXVolume(float volume)
    //{
    //    sfxSource.volume = volume;
    //}

    /// <summary>
    /// Mute tất cả âm thanh
    /// </summary>
    public void MuteAll()
    {
        if (musicSource != null) musicSource.mute = true;
        if (sfxSource != null) sfxSource.mute = true;
    }

    /// <summary>
    /// Unmute tất cả âm thanh
    /// </summary>
    public void UnmuteAll()
    {
        if (musicSource != null) musicSource.mute = false;
        if (sfxSource != null) sfxSource.mute = false;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }
}
