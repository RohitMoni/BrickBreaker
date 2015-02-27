using UnityEngine;
using System.Collections;

public class BrickScript : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
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
        }
    }
}
