using Assets.Code;
using UnityEngine;
using System.Collections;

public class HealthRingScript : MonoBehaviour {

    /* References */
    private HealthManagerScript _healthManager;
    private EdgeCollider2D _collider;
    
	// Use this for initialization
	void Start ()
	{
	    _collider = GetComponent<EdgeCollider2D>();
        _healthManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<HealthManagerScript>();
	}

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag != "Ball")
            return;

        _collider.enabled = false;
        _healthManager.HealthRingDestroyed(gameObject);
    }
}
