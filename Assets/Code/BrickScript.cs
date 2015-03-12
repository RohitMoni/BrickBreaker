using UnityEngine;

namespace Assets.Code
{
    public class BrickScript : MonoBehaviour
    {
        /* Properties */
        public int PointValue;
        public int HealthTotal;
        public int CurrentHealth;

        /* Sprites */
        public Sprite BrickHealth0, BrickHealth1, BrickHealth2, BrickHealth3;

        /* References */
        private GameObject _brickDestroyEffect;

        // Managers
        private GameManagerScript _gameManager;

        // Use this for initialization
        void Start () {
            _gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManagerScript>();
            _brickDestroyEffect = GameObject.FindGameObjectWithTag("ParticleBrickDestroy");
            PointValue = 50;
            HealthTotal = 1;
            CurrentHealth = 1;
            GetComponent<SpriteRenderer>().sprite = BrickHealth0;
        }

        private void Reset()
        {
            CurrentHealth = 1;
            GetComponent<SpriteRenderer>().sprite = BrickHealth0;
        }

        public void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (collision2D.gameObject.tag == "Ball")
            {
                CurrentHealth--;

                switch (CurrentHealth)
                {
                    case 3:
                        GetComponent<SpriteRenderer>().sprite = BrickHealth1;
                        break;
                    case 2:
                        GetComponent<SpriteRenderer>().sprite = BrickHealth2;
                        break;
                    case 1:
                        GetComponent<SpriteRenderer>().sprite = BrickHealth3;
                        break;
                    case 0:
                        // Move and rotate particle effect
                        var position = transform.position;
                        position.z = -4;
                        _brickDestroyEffect.transform.position = position;

                        var rotationVel = -collision2D.gameObject.GetComponent<BallScript>().Velocity.normalized;
                        var angle = Mathf.Atan2(rotationVel.y, rotationVel.x)*Mathf.Rad2Deg;
                        angle += 90;
                        var rotation = Quaternion.Euler(0, 0, angle);
                        _brickDestroyEffect.transform.rotation = rotation;

                        // Play particle effect
                        _brickDestroyEffect.GetComponent<ParticleSystem>().Emit((int) (transform.parent.localScale.x*8));

                        // Add points
                        _gameManager.AddScore(PointValue);

                        // Reset and 'Destroy' brick
                        Reset();
                        gameObject.SetActive(false);
                        break;
                }
            }
        }
    }
}
