﻿using UnityEngine;

namespace Assets.Code
{
    public class BrickScript : MonoBehaviour
    {
        /* Properties */
        public int CollectionIndex;
        public int RingIndex;

        /* References */

        // Managers
        private GameManagerScript _gameManager;

        // Use this for initialization
        void Start () {
            _gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManagerScript>();
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
