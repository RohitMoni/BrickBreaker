using System;
using UnityEngine;

namespace Assets.Code
{
    public class PaddleScript : MonoBehaviour
    {

        public float Speed;

        private GameObject _gameAnchor;
        private Camera _camera;

        // Use this for initialization
        void Start ()
        {
            _gameAnchor = GameObject.FindGameObjectWithTag("GameAnchor");
            _camera = Camera.main;
            Speed = 0.1f;
        }
	
        // Update is called once per frame
        void Update () {
            if (!_gameAnchor.GetComponent<GameManagerScript>().IsPaused)
            {

#if UNITY_EDITOR
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.Rotate(Vector3.forward, Speed);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.Rotate(Vector3.back, Speed);
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
                        var worldPos = _camera.ScreenToWorldPoint(touch.position);

                        var angleRadians = Math.Atan2(worldPos.y, worldPos.x);
                        var angleDegrees = angleRadians * 180.0f / Math.PI;

                        angleDegrees += 90;

                        transform.eulerAngles = new Vector3(0, 0, (float)angleDegrees);
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

                    //    transform.eulerAngles += new Vector3(0, 0, (float)angleDegrees * Speed);
                    //}
                }
#endif
            }
        }

        public void LaunchBalls()
        {
            foreach (Transform child in transform)
            {
                if (child.tag == "Ball")
                {
                    // Parent the ball to the game anchor so that it stops using the paddles movement
                    child.parent = _gameAnchor.transform;

                    // Apply an acceleration to its velocity, rotated by the paddle's rotation
                    var rotation = transform.rotation;
                    var appliedVelocity = rotation * Vector3.up;

                    child.GetComponent<BallScript>().ApplyVelocity(appliedVelocity);
                }
            }
        }

    
    }
}
