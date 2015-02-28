using UnityEngine;

namespace Assets.Code
{
    public class BrickScript : MonoBehaviour
    {

        private Transform _gameAnchor;

        // Use this for initialization
        void Start () {
            _gameAnchor = GameObject.FindGameObjectWithTag("GameAnchor").transform;
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        public void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (collision2D.gameObject.tag == "Ball")
            {
                Debug.Log("Brick Destroyed!");
                Destroy(gameObject);

                // Check for win condition
                var remainingBricks = GameObject.FindGameObjectsWithTag("Brick");
                var lost = true;
                foreach (var brick in remainingBricks)
                {
                    if (brick != gameObject)
                        lost = false;
                }

                if (lost)
                    _gameAnchor.GetComponent<GameManagerScript>().SetWinLossState(true);
            }
        }
    }
}
