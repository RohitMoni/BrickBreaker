using System;
using System.Linq;
using UnityEngine;

public class SoundManagerScript : MonoBehaviour {

    /* Properties */
    public AudioClip BackgroundMusic;
    public AudioClip BallHitsPaddleSe;
    public AudioClip BrickBreakingSe;
    public AudioClip CentreHitSe;
    public AudioClip ShockwaveSe;
    public AudioClip PowerupSe;

    public AudioClip[] BrickCollideSes;
    private int _brickCollideCount;

    /* References */
    private AudioSource _backgroundSource;
    private AudioSource _midLevelSource;
    private AudioSource _midLevelSource2;
    private AudioSource[] _brickCollideSource;

    /* Constants */

	// Use this for initialization
	void Start ()
	{
	    _backgroundSource = GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("BackgroundSound").GetComponent<AudioSource>();
        _midLevelSource = GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("MidLevelSound").GetComponent<AudioSource>();
        _midLevelSource2 = GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("MidLevelSound2").GetComponent<AudioSource>();
        _brickCollideSource = new AudioSource[3]
	    {
	        GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("BrickCollide1").GetComponent<AudioSource>(),
	        GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("BrickCollide2").GetComponent<AudioSource>(),
	        GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("BrickCollide3").GetComponent<AudioSource>()
	    };

	    if (BackgroundMusic)
	    {
	        _backgroundSource.clip = BackgroundMusic;
	        _backgroundSource.loop = true;
            _backgroundSource.Play();
	    }
	}

    public void ToggleBackgroundMusic()
    {
        if (!_backgroundSource)
            return;

        if (_backgroundSource.isPlaying)
            _backgroundSource.Pause();
        else
            _backgroundSource.Play();
    }   

    public void PlayBallHitsPaddleSound()
    {
        if (BallHitsPaddleSe)
            MidSource().PlayOneShot(BallHitsPaddleSe);
    }

    public void PlayPowerupSound()
    {
        if (PowerupSe)
            MidSource().PlayOneShot(PowerupSe);
    }

    public void PlayBrickBreakSound()
    {
        if (BrickBreakingSe)
            MidSource().PlayOneShot(BrickBreakingSe);
    }

    public void PlayCentreIsHitSound()
    {
        if (CentreHitSe)
            MidSource().PlayOneShot(CentreHitSe);
    }

    public void PlayShockwaveSound()
    {
        if (ShockwaveSe)
            MidSource().PlayOneShot(ShockwaveSe);
    }

    public void PlayBrickIsHitSound()
    {
        _brickCollideCount++;

        var soundEffectIndex = Math.Min(_brickCollideCount, BrickCollideSes.Count()-1); // This is the index of the sound effect we should play

        _brickCollideSource[_brickCollideCount%3].PlayOneShot(BrickCollideSes[soundEffectIndex]);
    }

    public void ResetBrickCollideCount()
    {
        _brickCollideCount = 0;
    }

    private AudioSource MidSource()
    {
        return _midLevelSource.isPlaying ? _midLevelSource2 : _midLevelSource;
    }
}
