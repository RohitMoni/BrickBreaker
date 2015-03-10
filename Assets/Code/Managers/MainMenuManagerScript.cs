using System;
using System.Timers;
using UnityEngine;

public class MainMenuManagerScript : MonoBehaviour {

    /* Properties */
    public bool IsUsingSlider;

    private float _timer;
    private bool _cameraIsShifting;
    private int _cameraShiftingTo;  // 0 = Start, 1 = Options
    private Vector3 _shiftStartPosition;

    /* References */
    private Camera _camera;

    /* Constants */
    private const float CameraShiftTime = 0.2f;

	// Use this for initialization
	void Start ()
	{
	    IsUsingSlider = false;

	    _timer = 0;
	    _cameraIsShifting = false;
	    _cameraShiftingTo = 0;
        _shiftStartPosition = new Vector3(0, 0, -10);

	    _camera = Camera.main;
	}
	
	// Update is called once per frame
	void Update ()
    {
        #region Camera Shift
        if (_cameraIsShifting)
	    {
	        // Update timer
            _timer += Time.deltaTime;
	        _timer = Math.Min(_timer, CameraShiftTime);

            // shift here
	        var endPosition = new Vector3( _cameraShiftingTo*1080, 0, -10);
            _camera.transform.position = Vector3.Lerp(_shiftStartPosition, endPosition, _timer / CameraShiftTime);

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
        else
        {
            foreach (var touch in Input.touches)
            {
                if (touch.phase == TouchPhase.Moved && !IsUsingSlider)
                {
                    _camera.transform.position -= new Vector3(touch.deltaPosition.x, 0, 0);
                }
                else
                {
                    switch (_cameraShiftingTo)
                    {
                        case 0:
                            if (_camera.transform.position.x >= 100)
                                StartMenuToOptions();
                            else
                                OptionsToStartMenu();
                            break;
                        case 1:
                            if (_camera.transform.position.x <= 980)
                                OptionsToStartMenu();
                            else
                                StartMenuToOptions();
                            break;
                    }
                }
            }
        }
        #endregion
    }

    public void StartGame()
    {
        Application.LoadLevel("GameScene");
    }

    public void StartMenuToOptions()
    {
        _shiftStartPosition = _camera.transform.position;
        _cameraShiftingTo = 1;
        _cameraIsShifting = true;
    }

    public void OptionsToStartMenu()
    {
        _shiftStartPosition = _camera.transform.position;
        _cameraShiftingTo = 0;
        _cameraIsShifting = true;
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void SetRelativePaddle(bool value)
    {
        GameVariablesScript.RelativePaddle = value;
    }

    public void SetSensitivity(float value)
    {
        GameVariablesScript.Sensitivity = (int)value;
    }

    public void SetBallSpeed(float value)
    {
        GameVariablesScript.BallSpeed = value / GameVariablesScript.BallSpeedCoeff;
    }
}
