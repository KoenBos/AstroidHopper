using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    //Gebruik:
    // AudioManager.Instance.PlaySFX("Explosion");
    // AudioManager.Instance.PlayMusic("BackgroundMusic", 0.5f); // 50% volume


    public static AudioManager Instance { get; private set; }

    [SerializeField] private AudioSource sfxSource;
    [SerializeField] private AudioSource musicSource;
    [SerializeField] private List<AudioClip> sfxClips = new List<AudioClip>(); // Lijst van SFX clips
    [SerializeField] private List<AudioClip> musicClips = new List<AudioClip>(); // Lijst van muziek clips

    private Dictionary<string, AudioClip> sfxClipDictionary = new Dictionary<string, AudioClip>();
    private Dictionary<string, AudioClip> musicClipDictionary = new Dictionary<string, AudioClip>();

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            InitializeAudioDictionaries();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void InitializeAudioDictionaries()
    {
        foreach (var clip in sfxClips)
        {
            sfxClipDictionary[clip.name] = clip;
        }
        foreach (var clip in musicClips)
        {
            musicClipDictionary[clip.name] = clip;
        }
    }

    public void PlaySFX(string name, float volume = 1.0f)
    {
        if (sfxClipDictionary.TryGetValue(name, out var clip))
        {
            sfxSource.PlayOneShot(clip, volume);
        }
        else
        {
            Debug.LogWarning("SFX clip not found: " + name);
        }
    }
    
    public void StopSFX()
    {
        sfxSource.Stop();
    }

    public void PlayMusic(string name, float volume = 1.0f, bool loop = true)
    {
        if (musicClipDictionary.TryGetValue(name, out var clip))
        {
            musicSource.clip = clip;
            musicSource.volume = volume;
            musicSource.loop = loop;
            musicSource.Play();
        }
        else
        {
            Debug.LogWarning("Music clip not found: " + name);
        }
    }

    public bool IsPlaying(string name)
    {
        if (sfxClipDictionary.TryGetValue(name, out var clip))
        {
            return sfxSource.clip == clip && sfxSource.isPlaying;
        }
        if (musicClipDictionary.TryGetValue(name, out clip))
        {
            return musicSource.clip == clip && musicSource.isPlaying;
        }
        return false;
    }

    public void StopMusic()
    {
        musicSource.Stop();
    }

    public void PauseMusic()
    {
        musicSource.Pause();
    }

    public void ResumeMusic()
    {
        musicSource.UnPause();
    }
}
