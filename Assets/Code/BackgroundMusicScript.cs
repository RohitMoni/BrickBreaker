using System;
using System.Linq;
using UnityEngine;

public class BackgroundMusicScript : MonoBehaviour {

    /* Properties */
    public AudioClip Music;

    /* References */
    private AudioSource _backgroundMusicSource;

    private static BackgroundMusicScript _instance = null;
    public static BackgroundMusicScript Instance
    {
        get { return _instance; }
    }

    /* Constants */

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
    
        _instance = this;
        _backgroundMusicSource = GetComponent<AudioSource>();
        _backgroundMusicSource.clip = Music;
        _backgroundMusicSource.loop = true;
        _backgroundMusicSource.Play();
        DontDestroyOnLoad(gameObject);
    }

    public static void ToggleBackgroundMusic()
    {
        if (Instance._backgroundMusicSource.isPlaying)
            Instance._backgroundMusicSource.Pause();
        else
            Instance._backgroundMusicSource.UnPause();
    }
}
