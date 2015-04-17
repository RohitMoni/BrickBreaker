using UnityEngine;

namespace Assets.Code
{
    public class BallScript : MonoBehaviour
    {
        /* Properties */
        public Vector3 Velocity;
        public bool PowerBall;

        /* References */
        private GameManagerScript _gameManager;
        private SoundManagerScript _soundManager;

        // Anchors
        private GameObject _paddleAnchor;

        // Use this for initialization
        void Start ()
        {
            //Speed = 0.03f;
            PowerBall = false;
            
            _gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManagerScript>();
            _soundManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<SoundManagerScript>();
            _paddleAnchor = GameObject.FindGameObjectWithTag("PaddleAnchor");

            transform.parent = _paddleAnchor.transform;
        }
	
        // Update is called once per frame
        void Update () {
            if (!_gameManager.IsPaused)
            {
                transform.position += Velocity * Time.smoothDeltaTime;
            }
        }

        public void ApplyVelocity(Vector3 newDirection)
        {
            Velocity = newDirection * GameVariablesScript.BallSpeed;
        }

        public void OnCollisionEnter2D(Collision2D collision2D)
        {
            var rotation = Quaternion.FromToRotation(Vector3.up, collision2D.contacts[0].normal);
            var appliedVelocity = rotation*Vector3.up;

            ApplyVelocity(appliedVelocity);

            if (collision2D.gameObject.tag == "Paddle")
            {
                _gameManager.ResetComboValue();
                _soundManager.PlayBallHitsPaddleSound();
                _soundManager.ResetBrickCollideCount();
            }
        }
    }
}
