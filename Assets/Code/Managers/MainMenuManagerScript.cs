using System;
using System.Linq;
using System.Net.Mime;
using System.Timers;
using Assets.Code;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuManagerScript : MonoBehaviour {

    /* Properties */
    public bool IsUsingSlider;

    private float _timer;
    private bool _cameraIsShifting;
    private int _cameraShiftingTo;  // 0 = Start, 1 = Options, 2 = Score screen
    private Vector3 _shiftStartPosition;

    /* References */
    private Camera _camera;

    private Text _lastScoreText;
    private GameObject[] _highScoreText;
    private Toggle _relativeMovementToggle;
    private Toggle _sliderMovementToggle;
    private Slider _sensitivitySlider;
    private Slider _ballSpeedSlider;

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
	    _lastScoreText = GameObject.FindGameObjectWithTag("LastScoreText").GetComponent<Text>();
	    _highScoreText = GameObject.FindGameObjectsWithTag("HighScoreText");
        _relativeMovementToggle = GameObject.FindGameObjectWithTag("RelativeMovementToggle").GetComponent<Toggle>();
	    _sliderMovementToggle = GameObject.FindGameObjectWithTag("SliderMovementToggle").GetComponent<Toggle>();
        _sensitivitySlider = GameObject.FindGameObjectWithTag("SensitivitySlider").GetComponent<Slider>();
        _ballSpeedSlider = GameObject.FindGameObjectWithTag("BallSpeedSlider").GetComponent<Slider>();

        _shiftStartPosition = new Vector3(GameVariablesScript.ScreenToStartOn * 1080, 0, -10);
	    _camera.transform.position = _shiftStartPosition;

        FileServices.LoadGame();

        // Setting ui elements from game variables
        foreach (var highScoreTextObj in _highScoreText)
            highScoreTextObj.GetComponent<Text>().text = GameVariablesScript.HighScore.ToString();
        _lastScoreText.text = GameVariablesScript.LastScore.ToString();
	    _relativeMovementToggle.isOn = GameVariablesScript.RelativePaddle;
	    _sliderMovementToggle.isOn = GameVariablesScript.SliderMovement;
	    _sensitivitySlider.value = GameVariablesScript.PaddleSensitivity*GameVariablesScript.PaddleSensitivityCoeff;
	    _ballSpeedSlider.value = GameVariablesScript.BallSpeed*GameVariablesScript.BallSpeedCoeff;
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
        //            switch (_cameraShiftingTo)
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
    }

    public void StartGame()
    {
        Application.LoadLevel("GameScene");
    }

    public void ShiftToScreen(int screenNumber)
    {
        _shiftStartPosition = _camera.transform.position;
        _cameraShiftingTo = screenNumber;
        _cameraIsShifting = true;
    }

    public void QuitGame()
    {
        FileServices.SaveLog();
        Application.Quit();
    }

    #region Menu Controlled Settings
    public void SetRelativePaddle(bool value)
    {
        GameVariablesScript.RelativePaddle = value;
        _sensitivitySlider.interactable = value;

        var colour = (value ? new Color(1, 1, 1, 1) : new Color(100f / 256f, 100 / 256f, 100f / 256f, 1));

        _sensitivitySlider.transform.GetChild(0).GetComponent<Image>().color = colour;
        _sensitivitySlider.transform.GetChild(1).GetChild(0).GetComponent<Image>().color = colour;
        _sensitivitySlider.transform.GetChild(3).GetComponent<Text>().color = colour;

        FileServices.SaveGame();
    }

    public void SetSliderMovement(bool value)
    {
        GameVariablesScript.SliderMovement = value;
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
    #endregion
}
