using UnityEngine;

namespace Assets.Code
{
    public class BallScript : MonoBehaviour
    {
        /* Properties */
        private float _speed;
        private Vector3 _velocity;

        /* References */
        private GameManagerScript _gameManager;

        // Anchors
        private GameObject _paddleAnchor;

        // Use this for initialization
        void Start ()
        {
            _speed = 0.03f;
            
            _gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManagerScript>();
            _paddleAnchor = GameObject.FindGameObjectWithTag("PaddleAnchor");

            transform.parent = _paddleAnchor.transform;
        }
	
        // Update is called once per frame
        void Update () {
            if (!_gameManager.IsPaused)
            {
                transform.position += _velocity;
            }
        }

        public void ApplyVelocity(Vector3 newVelocity)
        {
            _velocity = newVelocity * _speed;
        }

        public void OnCollisionEnter2D(Collision2D collision2D)
        {
            Debug.Log("collided!");

            if (collision2D.gameObject.tag == "OuterRing")
            {
                // Destroy ball
                Destroy(gameObject);

                // Check for loss condition
                var remainingBalls = GameObject.FindGameObjectsWithTag("Ball");
                var lost = true;
                foreach (var ball in remainingBalls)
                {
                    if (ball != gameObject)
                        lost = false;
                }

                if (lost)
                    _gameManager.SetWinLossState(false);
            }
            else
            {
                var rotation = Quaternion.FromToRotation(Vector3.up, collision2D.contacts[0].normal);
                var appliedVelocity = rotation*Vector3.up;

                _velocity = appliedVelocity*_speed;
            }
        }

    }
}
