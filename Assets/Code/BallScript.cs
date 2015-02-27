using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour
{
    /* Properties */
    private bool _started;
    private float _speed;
    private Vector3 _velocity;

    /* References */
    private Transform _gameAnchor, _paddleAnchor;


	// Use this for initialization
	void Start ()
	{
	    _started = false;
	    _speed = 0.03f;


	    _gameAnchor = GameObject.FindGameObjectWithTag("GameAnchor").transform;
	    _paddleAnchor = GameObject.FindGameObjectWithTag("PaddleAnchor").transform;

	    transform.parent = _paddleAnchor;
	}
	
	// Update is called once per frame
	void Update () {
        if (Input.GetKey(KeyCode.Space) && !_started)
        {
            // Parent the ball to the game anchor so that it stops using the paddles movement
            transform.parent = _gameAnchor;

            // Apply an acceleration to its velocity, rotated by the paddle's rotation
            var rotation = _paddleAnchor.rotation;
            var appliedVelocity = rotation * Vector3.up;

            _velocity = appliedVelocity * _speed;
            _started = true;
        }

	    transform.position += _velocity;
	}

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        Debug.Log("collided!");

        if (collision2D.gameObject.tag == "OuterRing")
        {
            // Destroy ball
            Destroy(gameObject);
        }
        else
        {
            var rotation = Quaternion.FromToRotation(Vector3.up, collision2D.contacts[0].normal);
            var appliedVelocity = rotation*Vector3.up;

            _velocity = appliedVelocity*_speed;
        }
    }

}
