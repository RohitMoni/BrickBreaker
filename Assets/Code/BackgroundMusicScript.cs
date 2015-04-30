using System;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class BackgroundMusicScript : MonoBehaviour {

    /* Properties */
    public AudioClip Music;

    /* References */
    private AudioSource _backgroundMusicSource;

    public static BackgroundMusicScript Instance { get; private set; }

    /* Constants */

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    
        Instance = this;
        _backgroundMusicSource = GetComponent<AudioSource>();
        _backgroundMusicSource.clip = Music;
        _backgroundMusicSource.loop = true;
        _backgroundMusicSource.Play();

        DontDestroyOnLoad(gameObject);
    }

    public static void SetBackgroundMusicMute(bool muted)
    {
        Instance._backgroundMusicSource.mute = muted;
    }

    public static void ToggleBackgroundMusic()
    {
        if (Instance._backgroundMusicSource.isPlaying)
            Instance._backgroundMusicSource.Pause();
        else
            Instance._backgroundMusicSource.UnPause();
    }

    public static void PlayBackgroundMusic()
    {
        Instance._backgroundMusicSource.UnPause();
    }
}
