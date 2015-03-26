using System;
using System.Linq;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour {

    /* Properties */
    public AudioClip BackgroundMusic;
    public AudioClip LoseGameMusic;
    public AudioClip BallHitsPaddleSe;
    public AudioClip BrickBreakingSe;
    public AudioClip CentreHitSe;
    public AudioClip ShockwaveSe;

    public AudioClip[] BrickCollideSes;
    private int _brickCollideCount;

    /* References */
    private AudioSource _backgroundSource;
    private AudioSource _midLevelSource;
    private AudioSource [] _brickCollideSource;

    /* Constants */

	// Use this for initialization
	void Start ()
	{
	    _backgroundSource = GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("BackgroundSound").GetComponent<AudioSource>();
        _midLevelSource = GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("MidLevelSound").GetComponent<AudioSource>();
	    _brickCollideSource = new AudioSource[3]
	    {
	        GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("BrickCollide1").GetComponent<AudioSource>(),
	        GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("BrickCollide2").GetComponent<AudioSource>(),
	        GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("BrickCollide3").GetComponent<AudioSource>()
	    };
	}

    public void ToggleBackgroundMusic()
    {
        if (_backgroundSource.isPlaying)
            _backgroundSource.Pause();
        else
            _backgroundSource.Play();
    }   

    public void PlayLoseMusic()
    {
        _backgroundSource.PlayOneShot(LoseGameMusic);
    }

    public void PlayBallHitsPaddleSound()
    {
        _midLevelSource.PlayOneShot(BallHitsPaddleSe);
    }

    public void PlayBrickBreakSound()
    {
        _midLevelSource.PlayOneShot(BrickBreakingSe);
    }

    public void PlayCentreIsHitSound()
    {
        _midLevelSource.PlayOneShot(CentreHitSe);
    }

    public void PlayShockwaveSound()
    {
        _midLevelSource.PlayOneShot(ShockwaveSe);
    }

    public void PlayBrickIsHitSound()
    {
        _brickCollideCount++;

        var soundEffectIndex = Math.Min(_brickCollideCount, BrickCollideSes.Count()); // This is the index of the sound effect we should play

        _brickCollideSource[_brickCollideCount%3].PlayOneShot(BrickCollideSes[soundEffectIndex]);
    }

    public void ResetBrickCollideCount()
    {
        _brickCollideCount = 0;
    }
}
