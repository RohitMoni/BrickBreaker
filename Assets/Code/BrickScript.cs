using UnityEngine;

namespace Assets.Code
{
    public class BrickScript : MonoBehaviour
    {
        /* Properties */
        public int CollectionIndex;
        public int RingIndex;

        /* References */
        private GameObject _brickDestroyEffect;

        // Managers
        private GameManagerScript _gameManager;

        // Use this for initialization
        void Start () {
            _gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManagerScript>();
            _brickDestroyEffect = GameObject.FindGameObjectWithTag("ParticleBrickDestroy");
            //GetComponent<BoxCollider2D>().enabled = false;
        }

        public void Initialise()
        {
            //GetComponent<BoxCollider2D>().enabled = true;            
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        public void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (collision2D.gameObject.tag == "Ball")
            {
                Debug.Log("Brick Destroyed!");

                // Move and rotate particle effect
                var position = transform.position;
                position.z = -4;
                _brickDestroyEffect.transform.position = position;

                var rotationVel = -collision2D.gameObject.GetComponent<BallScript>()._velocity.normalized;
                var angle = Mathf.Atan2(rotationVel.y, rotationVel.x)*Mathf.Rad2Deg;
                angle += 90;
                var rotation = Quaternion.Euler(0, 0, angle);
                _brickDestroyEffect.transform.rotation = rotation;

                // Play particle effect
                _brickDestroyEffect.GetComponent<ParticleSystem>().Emit((int)(transform.parent.localScale.x * 8));

                // Destroy brick
                Destroy(gameObject);

                // Check for win condition
                var remainingBricks = GameObject.FindGameObjectsWithTag("Brick");
                var won = true;
                foreach (var brick in remainingBricks)
                {
                    if (brick != gameObject)
                        won = false;
                }

                if (won)
                    _gameManager.SetWinLossState(true);
            }
        }
    }
}
