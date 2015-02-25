using UnityEngine;
using System.Collections;

public class BallScript : MonoBehaviour
{

    private int _direction; // towards the centre (0) or away from the centre (1)
    private Transform _innerRing;

	// Use this for initialization
	void Start ()
	{
	    _direction = 1;
	    _innerRing = GameObject.FindGameObjectWithTag("InnerRing").transform;
	}
	
	// Update is called once per frame
	void Update () {
	    
	}
}
