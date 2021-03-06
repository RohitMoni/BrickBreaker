﻿using System;
using System.Collections;
using System.Linq;
using System.Net.Mime;
using System.Timers;
using Assets.Code;
using Soomla.Profile;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManagerScript : MonoBehaviour {

    /* Editor Set */
    public Sprite SoundEffectsOnImage;
    public Sprite SoundEffectsOffImage;
    public Sprite MusicOnImage;
    public Sprite MusicOffImage;

    /* Properties */
    public bool IsUsingSlider;

    private float _timer;
    private bool _cameraIsShifting;
    private Vector3 _shiftEndPosition;  // 0 = Start, 1 = Options, 2 = Score screen, 3 = Social Screen (Either Log in or already logged in)
    private Vector3 _shiftStartPosition;

    /* References */
    private Camera _camera;

    private Text _lastScoreText;
    private GameObject[] _highScoreText;
    private Slider _sensitivitySlider;
    private Slider _ballSpeedSlider;
    private Transform[] _controlSchemeToggleButtons;
    private GameObject[] _controlSchemeDemoImages;
    private Image _muteSoundEffectsButtonImage;
    private Image _muteMusicButtonImage;
    private GameObject _socialMenus;

    /* Constants */
    private const float CameraShiftTime = 0.2f;

	// Use this for initialization
	void Start ()
	{
	    IsUsingSlider = false;

	    _timer = 0;
	    _cameraIsShifting = false;
	    _shiftEndPosition = new Vector3(0, 0, -10);
        _shiftStartPosition = new Vector3(0, 0, -10);

	    _camera = Camera.main;
	    _lastScoreText = GameObject.FindGameObjectWithTag("LastScoreText").GetComponent<Text>();
	    _highScoreText = GameObject.FindGameObjectsWithTag("HighScoreText");
        _sensitivitySlider = GameObject.FindGameObjectWithTag("SensitivitySlider").GetComponent<Slider>();
        _ballSpeedSlider = GameObject.FindGameObjectWithTag("BallSpeedSlider").GetComponent<Slider>();
	    _controlSchemeToggleButtons = GameObject.Find("ControlSchemeToggleButtons").transform.GetComponentsInChildren<Transform>();
	    _controlSchemeDemoImages = new GameObject[4];
	    _controlSchemeDemoImages[0] = GameObject.Find("FreeScheme");
        _controlSchemeDemoImages[1] = GameObject.Find("PreciseScheme");
        _controlSchemeDemoImages[2] = GameObject.Find("SliderScheme");
        _controlSchemeDemoImages[3] = GameObject.Find("TapScheme");
        _muteSoundEffectsButtonImage = GameObject.Find("MuteSoundEffectsButton").GetComponent<Image>();
        _muteMusicButtonImage = GameObject.Find("MuteMusicButton").GetComponent<Image>();
	    _socialMenus = GameObject.Find("SocialMenus");

        _shiftStartPosition = new Vector3(GameVariablesScript.ScreenToStartOn * 1080, 0, -10);
	    _camera.transform.position = _shiftStartPosition;

        FileServices.LoadGame();

        BackgroundMusicScript.SetBackgroundMusicMute(GameVariablesScript.MusicMuted);

        SetUiFromGameVariables();
	}
	
	// Update is called once per frame
	void Update ()
    {
        #region Camera Shift
        if (_cameraIsShifting)
	    {
	        // Update timer
            _timer += Time.smoothDeltaTime;
	        _timer = Math.Min(_timer, CameraShiftTime);

            // shift here
            _camera.transform.position = Vector3.Lerp(_shiftStartPosition, _shiftEndPosition, Mathf.SmoothStep(0f, 1f, _timer / CameraShiftTime));

            // Check to see if we stop shifting
	        if (_timer == CameraShiftTime) // the earlier min function ensures we hit the exact number
	        {
                // Reset the timer
	            _timer = 0;

                // Exit
	            _cameraIsShifting = false;
	        }
        }
        #endregion

        #region Swiping
        //else
        //{
        //    foreach (var touch in Input.touches)
        //    {
        //        if (touch.phase == TouchPhase.Moved && !IsUsingSlider)
        //        {
        //            _camera.transform.position -= new Vector3(touch.deltaPosition.x, 0, 0);
        //        }
        //        else
        //        {
        //            switch (_shiftEndPosition)
        //            {
        //                case 0:
        //                    if (_camera.transform.position.x >= 100)
        //                        ShiftToScreen(1);
        //                    else
        //                        ShiftToScreen(0);
        //                    break;
        //                case 1:
        //                    if (_camera.transform.position.x <= 980)
        //                        ShiftToScreen(0);
        //                    else
        //                        ShiftToScreen(1);
        //                    break;
        //            }
        //        }
        //    }
        //}
        #endregion

        if (Input.GetKeyDown(KeyCode.Escape))
            Application.Quit();

	    if (Input.GetKeyDown(KeyCode.Return))
	        SetUserLoggedIn(true);
        else if (Input.GetKeyDown(KeyCode.Backslash))
            SetUserLoggedIn(false);
    }

    public void StartGame()
    {
        Application.LoadLevel("GameScene");
    }

    public void ShiftToScreen(int screenNumber)
    {
        _shiftStartPosition = _camera.transform.position;

        switch (screenNumber)
        {
            case 0:
            case 1:
            case 2:
                _shiftEndPosition = new Vector3( screenNumber*Screen.width, 0, -10);
                break;
            case 3:
                _shiftEndPosition = new Vector3( 0, Screen.height, -10);
                break;
        }

        _cameraIsShifting = true;
    }

    public void QuitGame()
    {
        FileServices.SaveLog();
        Application.Quit();
    }

    #region Menu Controlled Settings

    public void SetUserLoggedIn(bool loggedIn)
    {
        var endX = loggedIn ? -Screen.width : 0;

        StartCoroutine(ShiftSocialMenus(_socialMenus.transform.position.x, endX, 0.2f));
    }

    public void ToggleMuteSounds()
    {
        GameVariablesScript.SoundEffectsMuted = !GameVariablesScript.SoundEffectsMuted;

        _muteSoundEffectsButtonImage.overrideSprite = GameVariablesScript.SoundEffectsMuted ? SoundEffectsOffImage : SoundEffectsOnImage;

        FileServices.SaveGame();
    }

    public void ToggleMuteMusic()
    {
        GameVariablesScript.MusicMuted = !GameVariablesScript.MusicMuted;

        _muteMusicButtonImage.overrideSprite = GameVariablesScript.MusicMuted ? MusicOffImage : MusicOnImage;

        BackgroundMusicScript.SetBackgroundMusicMute(GameVariablesScript.MusicMuted);

        FileServices.SaveGame();
    }

    public void SetControlScheme(int schemeNumber)
    {
        GameVariablesScript.ControlScheme = schemeNumber;

        SetControlSchemeUiFromGameVariables();

        FileServices.SaveGame();
    }

    public void SetSensitivity(float value)
    {
        GameVariablesScript.PaddleSensitivity = value / GameVariablesScript.PaddleSensitivityCoeff;
        FileServices.SaveGame();
    }

    public void SetBallSpeed(float value)
    {
        GameVariablesScript.BallSpeed = value / GameVariablesScript.BallSpeedCoeff;
        FileServices.SaveGame();
    }

    public void Reset()
    {
        GameVariablesScript.ResetVariables();
        SetUiFromGameVariables();
        FileServices.SaveGame();
    }

    private void SetSensitivitySliderEnabled(bool Enabled)
    {
        _sensitivitySlider.interactable = Enabled;
        var colour = Enabled ? Color.white : Color.gray;
        _sensitivitySlider.transform.FindChild("Background").GetComponent<Image>().color = colour;
        _sensitivitySlider.transform.FindChild("Fill Area").FindChild("Fill").GetComponent<Image>().color = colour;
    }

    private void SetUiFromGameVariables()
    {
        // Setting ui elements from game variables

        // Score
        foreach (var highScoreTextObj in _highScoreText)
            highScoreTextObj.GetComponent<Text>().text =  GameVariablesScript.ConvertScoreToString(GameVariablesScript.HighScore);
        _lastScoreText.text = GameVariablesScript.ConvertScoreToString(GameVariablesScript.LastScore);

        // Options sliders
        _sensitivitySlider.value = GameVariablesScript.PaddleSensitivity * GameVariablesScript.PaddleSensitivityCoeff;
        _ballSpeedSlider.value = GameVariablesScript.BallSpeed * GameVariablesScript.BallSpeedCoeff;

        // Control Scheme
        SetControlSchemeUiFromGameVariables();

        // Music  & Sound
        _muteMusicButtonImage.overrideSprite = GameVariablesScript.MusicMuted ? MusicOffImage : MusicOnImage;
        _muteSoundEffectsButtonImage.overrideSprite = GameVariablesScript.SoundEffectsMuted ? SoundEffectsOffImage : SoundEffectsOnImage;

        // Social Network stuff
        if (SoomlaProfile.IsLoggedIn(Provider.FACEBOOK) || SoomlaProfile.IsLoggedIn(Provider.TWITTER) || SoomlaProfile.IsLoggedIn(Provider.GOOGLE))
            SetUserLoggedIn(true);
    }

    private void SetControlSchemeUiFromGameVariables()
    {
        // Control Scheme Button colours and demo images
        for (var i = 1; i < _controlSchemeToggleButtons.Length; i++)
        {
            var button = _controlSchemeToggleButtons[i];
            var demoImage = _controlSchemeDemoImages[i - 1];
            if (i == GameVariablesScript.ControlScheme)
            {
                button.GetComponent<Image>().color = new Color(255f / 255f, 132f / 255f, 0, 1);
                demoImage.SetActive(true);
            }
            else
            {
                button.GetComponent<Image>().color = new Color(1, 1, 1, 1);
                demoImage.SetActive(false);
            }
        }

        if (GameVariablesScript.ControlScheme == 1 || GameVariablesScript.ControlScheme == 4)
            SetSensitivitySliderEnabled(true);
        else
            SetSensitivitySliderEnabled(false);
    }

    private IEnumerator ShiftSocialMenus(float startX, float endX, float time)
    {
        var timer = 0f;
        while (timer < time)
        {
            timer += Time.deltaTime;
            _socialMenus.transform.position = Vector3.Lerp(new Vector3(startX, 0, 0), new Vector3(endX, 0, 0), Mathf.SmoothStep(0, 1, timer/time));
            yield return null;
        }

        _socialMenus.transform.position = new Vector3(endX, 0, 0);
    }
    #endregion
}
