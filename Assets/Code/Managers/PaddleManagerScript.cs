using System;
using System.Linq;
using Assets.Code;
using UnityEngine;
using System.Collections;

public class PaddleManagerScript : MonoBehaviour
{
    private float _speed;

    /* References */
    // Managers
    private GameManagerScript _gameManager;

    // Anchors
    private GameObject _gameAnchor;
    private GameObject _paddleAnchor;

    // Other
    public  GameObject BallPrefab;
    private Camera _camera;

    // Use this for initialization
    void Start()
    {
        _gameManager = GetComponent<GameManagerScript>();
        _gameAnchor = GameObject.FindGameObjectWithTag("GameAnchor");
        _paddleAnchor = GameObject.FindGameObjectWithTag("PaddleAnchor");
        _camera = Camera.main;
        _speed = 0.1f;
    }

    // Update is called once per frame
    void Update()
    {
        if (!_gameManager.IsPaused)
        {

#if UNITY_EDITOR
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                _paddleAnchor.transform.Rotate(Vector3.forward, _speed);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                _paddleAnchor.transform.Rotate(Vector3.back, _speed);
            }
            if (Input.GetKey(KeyCode.Space))
            {
                LaunchBalls();
            }
#endif
#if UNITY_ANDROID
            foreach (var touch in Input.touches)
            {
                if (touch.phase != TouchPhase.Ended && touch.phase != TouchPhase.Canceled)
                {
                    // Ignore touches that hit the pause button
                    if (touch.position.y > Screen.height / 9.0f)
                        break;

                    var worldPos = _camera.ScreenToWorldPoint(touch.position);

                    var angleRadians = Math.Atan2(worldPos.y, worldPos.x);
                    var angleDegrees = angleRadians * 180.0f / Math.PI;

                    angleDegrees += 90;

                    _paddleAnchor.transform.eulerAngles = new Vector3(0, 0, (float)angleDegrees);
                }

                if (touch.tapCount == 2)
                {
                    LaunchBalls();
                }

                //if (touch.phase == TouchPhase.Moved)
                //{
                //    var adj = _camera.ScreenToWorldPoint(touch.position);
                //    var opp = _camera.ScreenToWorldPoint(touch.deltaPosition);

                //    var angleDegrees = Math.Atan(opp.magnitude / adj.magnitude) * 180 / Math.PI / 10.0f;

                //    if ((opp.x > 0 && adj.y < 0) || (opp.x < 0 && adj.y > 0))
                //        angleDegrees = -angleDegrees;

                //    transform.eulerAngles += new Vector3(0, 0, (float)angleDegrees * _speed);
                //}
            }
#endif
        }
    }

    public void LaunchBalls()
    {
        foreach (Transform child in _paddleAnchor.transform)
        {
            if (child.tag == "Ball")
            {
                // Parent the ball to the game anchor so that it stops using the paddles movement
                child.parent = _gameAnchor.transform;

                // Apply an acceleration to its velocity, rotated by the paddle's rotation
                var rotation = _paddleAnchor.transform.rotation;
                var appliedVelocity = rotation * Vector3.up;

                child.GetComponent<BallScript>().ApplyVelocity(appliedVelocity);
            }
        }
    }

    public void Reset()
    {
        _paddleAnchor.transform.rotation = Quaternion.identity;

        // Remove all balls
        foreach (Transform child in _gameAnchor.transform.Cast<Transform>().Where(child => child.tag == "Ball"))
        {
            Destroy(child.gameObject);
        }
    }

    public void CreateNewBall()
    {
        var ball = Instantiate(BallPrefab) as GameObject;
        ball.transform.position = _paddleAnchor.transform.GetChild(0).transform.position;
        ball.transform.position += new Vector3(0, 0.25f, 0);
    }
}
