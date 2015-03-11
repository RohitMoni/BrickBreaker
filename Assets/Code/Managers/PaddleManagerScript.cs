﻿using System;
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

    // Use this for initialization
    void Start()
    {
        _gameManager = GetComponent<GameManagerScript>();
        _gameAnchor = GameObject.FindGameObjectWithTag("GameAnchor");
        _paddleAnchor = GameObject.FindGameObjectWithTag("PaddleAnchor");
        _eventManager = GameObject.FindGameObjectWithTag("EventText").GetComponent<EventTextScript>();
        _controlSlider = GameObject.FindGameObjectWithTag("ControlSlider").GetComponent<Slider>();
        _camera = Camera.main;

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
            }

            if (!GameVariablesScript.SliderMovement)
                NonSliderMovement();
#endif
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
        if (GameVariablesScript.RelativePaddle)
        {
            _paddleAnchor.transform.eulerAngles = new Vector3(0, 0, sliderValue-180);
        }
        else
        {
            
        }
    }

    public void NonSliderMovement()
    {
        foreach (var touch in Input.touches)
        {
            if (GameVariablesScript.RelativePaddle)
            {
                // OLD RELATIVE MOVEMENT
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
                    if (Mathf.Abs(angleDifference) > 330)
                        // arbitrary high number that represents when the two points are in different quadrants
                        angleDifference += (360 * -Mathf.Sign(angleDifference));

                    angleDifference *= GameVariablesScript.Sensitivity;

                    var rotationChange = new Quaternion { eulerAngles = new Vector3(0, 0, angleDifference) };

                    _paddleAnchor.transform.rotation = _paddleAnchor.transform.rotation * rotationChange;
                }
            }
            else
            {
                // OLD ABSOLUTE MOVEMENT
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

        _paddleAnchor.transform.rotation = Quaternion.identity;

        // Remove all balls
        var balls = GameObject.FindGameObjectsWithTag("Ball");
        
        foreach (var ball in balls)
            Destroy(ball);
    }
}
