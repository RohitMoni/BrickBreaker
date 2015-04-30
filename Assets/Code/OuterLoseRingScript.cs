using Assets.Code;
using UnityEngine;
using System.Collections;

public class OuterLoseRingScript : MonoBehaviour {

    /* References */
    private GameManagerScript _gameManager;

	// Use this for initialization
	void Start () {
        _gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManagerScript>();
	}

    void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag == "Ball")
        {
            // Destroy ball
            Destroy(col.gameObject);

            // Check for loss condition
            var remainingBalls = GameObject.FindGameObjectsWithTag("Ball");
            var lost = true;
            foreach (var ball in remainingBalls)
            {
                if (ball != col.gameObject)
                    lost = false;
            }

            if (lost)
                _gameManager.SetWinLossState(false);
        }
    }
}
