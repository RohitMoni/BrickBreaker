using UnityEngine;

namespace Assets.Code
{
    public class PowerupScript : MonoBehaviour {

        /* Properties */
        public string PowerupTag;

        /* References */
        private PowerupManagerScript _powerupManager;

        void Start()
        {
            _powerupManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<PowerupManagerScript>();
        }

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.tag == "Paddle")
            {
                _powerupManager.TriggerPowerup(gameObject);
            }
        }
    }
}
