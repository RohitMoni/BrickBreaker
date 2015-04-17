using System;
using System.Linq;
using System.Net.Mime;
using Assets.Code;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class PaddleManagerScript : MonoBehaviour
{
    /* Properties */
#if UNITY_EDITOR
    private float _paddleSpeed;
#endif
    private int _numberOfActivePaddles;
    private int _controlFingerId;
    private Quaternion _currentSliderMovement;
    private float _currentPaddleWidth;
    private IEnumerator _widenCoroutine;
    
    /* References */
    // Managers
    private GameManagerScript _gameManager;

    // Anchors
    private GameObject _gameAnchor;
    private GameObject _paddleAnchor;

    // Other
    public  GameObject BallPrefab;
    private Camera _camera;
    private EventTextScript _eventManager;
    private Slider _controlSlider;

    /* Constants */
    private const int MaxNumberOfPaddles = 4;
    private const float MaxPaddleWidthScale = 1.5f;
    private const float PaddleWidthIncrease = 0.1f;

    // Use this for initialization
    void Start()
    {
        _gameManager = GetComponent<GameManagerScript>();
        _gameAnchor = GameObject.FindGameObjectWithTag("GameAnchor");
        _paddleAnchor = GameObject.FindGameObjectWithTag("PaddleAnchor");
        _eventManager = GameObject.FindGameObjectWithTag("EventText").GetComponent<EventTextScript>();
        _controlSlider = GameObject.FindGameObjectWithTag("ControlSlider").GetComponent<Slider>();
        _camera = Camera.main;

        Reset();

#if UNITY_EDITOR
        _paddleSpeed = 1.0f;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameManager.IsPaused)
        {

#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _paddleAnchor.transform.Rotate(Vector3.forward, _paddleSpeed);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _paddleAnchor.transform.Rotate(Vector3.back, _paddleSpeed);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                LaunchBalls();
            }
#endif
#if UNITY_ANDROID
            foreach (var touch in Input.touches)
            {
                if (touch.tapCount == 2)
                {
                    LaunchBalls();
                }

                // Reset slider if the user stops touching it
                if (touch.phase == TouchPhase.Ended || touch.phase == TouchPhase.Canceled)
                {
                    // End finger
                    if (GameVariablesScript.RelativePaddle)
                        _currentSliderMovement = Quaternion.identity;
                    else
                        _currentSliderMovement = _paddleAnchor.transform.rotation;

                    _controlFingerId = -1;
                    _controlSlider.value = 180;
                }
            }

            if (!GameVariablesScript.SliderMovement)
                NonSliderMovement();
            else
                if (GameVariablesScript.RelativePaddle)
                    _paddleAnchor.transform.rotation = _paddleAnchor.transform.rotation * _currentSliderMovement;
#endif
        }
    }

    public void WidenPaddles()
    {
        if (!(_currentPaddleWidth < MaxPaddleWidthScale)) return;

        if (_widenCoroutine != null)
            StopCoroutine(_widenCoroutine);

        _currentPaddleWidth = _currentPaddleWidth + PaddleWidthIncrease;
        _widenCoroutine = StretchSquashPaddles(_currentPaddleWidth, 5, 0.01f);
        StartCoroutine(_widenCoroutine);
    }

    private IEnumerator StretchSquashPaddles(float finalWidth, int nSteps, float timePerStep)
    {
        var paddles = GameObject.FindGameObjectsWithTag("Paddle");

        var scaleDifference = finalWidth - paddles[0].transform.localScale.x;
        var scaleChange = scaleDifference/nSteps;

        while (paddles[0].transform.localScale.x < finalWidth + scaleDifference)
        {
            foreach (var paddle in paddles)
                paddle.transform.localScale = paddle.transform.localScale + new Vector3(scaleChange*2, 0, 0);

            yield return new WaitForSeconds(timePerStep);
        }
        while (paddles[0].transform.localScale.x > finalWidth)
        {
            foreach (var paddle in paddles)
                paddle.transform.localScale = paddle.transform.localScale - new Vector3(scaleChange, 0, 0);

            yield return new WaitForSeconds(timePerStep);
        }

        foreach (var paddle in paddles)
            paddle.transform.localScale = new Vector3(_currentPaddleWidth, 1, 1);

        _widenCoroutine = null;
    }

    public void CreateNewPaddle()
    {
        if (_numberOfActivePaddles >= MaxNumberOfPaddles)
            return;

        foreach (var child in _paddleAnchor.transform.Cast<Transform>().Where(child => child.tag == "Paddle" && !child.gameObject.activeSelf))
        {
            child.gameObject.SetActive(true);
            child.localScale = _paddleAnchor.transform.GetChild(0).transform.localScale;
            break;
        }
    }

    public void CreateNewBall()
    {
        var ball = Instantiate(BallPrefab);
        ball.transform.position = _paddleAnchor.transform.GetChild(0).transform.position;
        ball.transform.position += new Vector3(0, 0.25f, 0);
        ball.GetComponent<CircleCollider2D>().enabled = false;

        // Create initial event text
        _eventManager.CreateEvent("Tap Twice");
    }

    public void LaunchBalls()
    {
        foreach (Transform child in _paddleAnchor.transform)
        {
            if (child.tag == "Ball")
            {
                // Parent the ball to the game anchor so that it stops using the paddles movement
                child.parent = _gameAnchor.transform;

                // Enable the collider so it collides with bricks now
                child.GetComponent<CircleCollider2D>().enabled = true;

                // Apply an acceleration to its velocity, rotated by the paddle's rotation
                var rotation = _paddleAnchor.transform.rotation;
                var appliedVelocity = rotation * Vector3.up;

                child.GetComponent<BallScript>().ApplyVelocity(appliedVelocity);
            }
        }

        if (_eventManager.GetCurrentEventText() == "Tap Twice")
        {
            _eventManager.StopEvent();
            _gameManager.TimerStarted = true;
        }
    }

    public void SliderMovement(float sliderValue)
    {
        if (!GameVariablesScript.RelativePaddle)
        {
            _paddleAnchor.transform.rotation = _currentSliderMovement * new Quaternion { eulerAngles = new Vector3(0, 0, sliderValue-180)};

            foreach (var touch in Input.touches)
            {
                _controlFingerId = touch.fingerId;
            }
        }
        else
        {
            foreach (var touch in Input.touches)
            {
                _controlFingerId = touch.fingerId;

                // If we're tracking the right finger
                if (touch.fingerId == _controlFingerId)
                {
                    var valueDifference = sliderValue - 180;

                    var angleDifference = valueDifference * GameVariablesScript.PaddleSensitivity;

                    _currentSliderMovement = new Quaternion { eulerAngles = new Vector3(0, 0, angleDifference) };
                }
            }
        }
    }

    public void NonSliderMovement()
    {
        foreach (var touch in Input.touches)
        {
            if (GameVariablesScript.RelativePaddle)
            {
                // RELATIVE MOVEMENT - Buggy
                if (touch.phase == TouchPhase.Moved)
                {
                    var touchPoint = _camera.ScreenToWorldPoint(touch.position);
                    touchPoint.z = 0;
                    var deltaV2 = touch.deltaPosition;
                    var delta = new Vector3(deltaV2.x, deltaV2.y, 0);

                    var finalPoint = touchPoint + delta;

                    var touchPointAngle = Mathf.Atan2(touchPoint.y, touchPoint.x) * Mathf.Rad2Deg + 180f;
                    var finalPointAngle = Mathf.Atan2(finalPoint.y, finalPoint.x) * Mathf.Rad2Deg + 180f;

                    var angleDifference = finalPointAngle - touchPointAngle;
                    if (Mathf.Abs(angleDifference) > 100)
                    {
                        // arbitrary high number that represents when the two points are in different quadrants
                        angleDifference += (360*-Mathf.Sign(angleDifference));
                    }

                    angleDifference *= GameVariablesScript.PaddleSensitivity;

                    var rotationChange = new Quaternion { eulerAngles = new Vector3(0, 0, angleDifference) };

                    _paddleAnchor.transform.rotation = _paddleAnchor.transform.rotation * rotationChange;
                }
            }
            else
            {
                // ABSOLUTE MOVEMENT
                if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                {
                    // Ignore touches that hit the pause button
                    if (touch.position.y > Screen.height / 10 * 9)
                        break;

                    var worldPos = _camera.ScreenToWorldPoint(touch.position);

                    var angleRadians = Math.Atan2(worldPos.y, worldPos.x);
                    var angleDegrees = angleRadians * 180.0f / Math.PI;

                    angleDegrees += 90;

                    _paddleAnchor.transform.eulerAngles = new Vector3(0, 0, (float)angleDegrees);
                }
            }
        }
    }

    public void Reset()
    {
        _controlSlider.value = 180;
        _controlFingerId = -1;
        _currentSliderMovement = Quaternion.identity;
        _numberOfActivePaddles = 1;

        _paddleAnchor.transform.rotation = Quaternion.identity;
        _currentPaddleWidth = 1;
        _widenCoroutine = null;

        // Remove all balls
        var balls = GameObject.FindGameObjectsWithTag("Ball");
        
        foreach (var ball in balls)
            Destroy(ball);

        // Reset all paddle objects
        foreach (Transform child in _paddleAnchor.transform)
        {
            child.localScale = new Vector3(1, 1, 1);
            child.gameObject.SetActive(false);
        }
        _paddleAnchor.transform.GetChild(0).gameObject.SetActive(true);
    }
}
