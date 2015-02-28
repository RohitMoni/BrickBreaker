using UnityEngine;

namespace Assets.Code
{
    public class PaddleScript : MonoBehaviour
    {

        public int Speed;

        private GameObject _gameAnchor;

        // Use this for initialization
        void Start ()
        {
            _gameAnchor = GameObject.FindGameObjectWithTag("GameAnchor");
        }
	
        // Update is called once per frame
        void Update () {
            if (!_gameAnchor.GetComponent<GameManagerScript>().IsPaused)
            {
                if (Input.GetKey(KeyCode.LeftArrow))
                {
                    transform.Rotate(Vector3.forward, Speed);
                }
                if (Input.GetKey(KeyCode.RightArrow))
                {
                    transform.Rotate(Vector3.back, Speed);
                }
            }
        }

    
    }
}
