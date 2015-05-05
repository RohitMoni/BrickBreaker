using System;
using System.Linq;
using System.Security.AccessControl;
using Assets.Code;
using UnityEngine;
using UnityEngine.UI;

public class SoundManagerScript : MonoBehaviour {

    /* Properties */
    public AudioClip BallHitsPaddleSe;
    public AudioClip BrickBreakingSe;
    public AudioClip CentreHitSe;
    public AudioClip ShockwaveSe;
    public AudioClip PowerupSe;

    public AudioClip[] BrickCollideSes;
    private int _brickCollideCount;

    public Sprite SoundEffectsOnImage;
    public Sprite SoundEffectsOffImage;

    public Sprite MusicOnImage;
    public Sprite MusicOffImage;

    /* References */
    private AudioSource _midLevelSource;
    private AudioSource _midLevelSource2;
    private AudioSource[] _brickCollideSource;
    private Image _muteSoundEffectsButtonImage;
    private Image _muteMusicButtonImage;

    /* Constants */

	// Use this for initialization
	void Start ()
	{
        _midLevelSource = GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("MidLevelSound").GetComponent<AudioSource>();
        _midLevelSource2 = GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("MidLevelSound2").GetComponent<AudioSource>();
        _brickCollideSource = new AudioSource[3]
	    {
	        GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("BrickCollide1").GetComponent<AudioSource>(),
	        GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("BrickCollide2").GetComponent<AudioSource>(),
	        GameObject.FindGameObjectWithTag("MainCamera").transform.FindChild("BrickCollide3").GetComponent<AudioSource>()
	    };

	    _muteSoundEffectsButtonImage = GameObject.Find("MuteSoundEffectsButton").GetComponent<Image>();
        _muteMusicButtonImage = GameObject.Find("MuteMusicButton").GetComponent<Image>();
	}

    public void StartUp()
    {
        ToggleMuteSounds();
        ToggleMuteSounds();
        ToggleMuteMusic();
        ToggleMuteMusic();
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

    public void ToggleMuteSounds()
    {
        GameVariablesScript.SoundEffectsMuted = !GameVariablesScript.SoundEffectsMuted;

        _muteSoundEffectsButtonImage.overrideSprite = GameVariablesScript.SoundEffectsMuted ? SoundEffectsOffImage : SoundEffectsOnImage;

        _midLevelSource.mute = GameVariablesScript.SoundEffectsMuted;
        _midLevelSource2.mute = GameVariablesScript.SoundEffectsMuted;
        _brickCollideSource[0].mute = GameVariablesScript.SoundEffectsMuted;
        _brickCollideSource[1].mute = GameVariablesScript.SoundEffectsMuted;
        _brickCollideSource[2].mute = GameVariablesScript.SoundEffectsMuted;

        FileServices.SaveGame();
    }

    public void ToggleMuteMusic()
    {
        GameVariablesScript.MusicMuted = !GameVariablesScript.MusicMuted;

        _muteMusicButtonImage.overrideSprite = GameVariablesScript.MusicMuted ? MusicOffImage : MusicOnImage;

        BackgroundMusicScript.SetBackgroundMusicMute(GameVariablesScript.MusicMuted);

        FileServices.SaveGame();
    }

    private AudioSource MidSource()
    {
        return _midLevelSource.isPlaying ? _midLevelSource2 : _midLevelSource;
    }
}
