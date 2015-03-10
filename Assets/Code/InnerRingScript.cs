using Assets.Code;
using UnityEngine;
using System.Collections;

public class InnerRingScript : MonoBehaviour {

    /* Properties */
    private float _timer;

    /* References */
    private GameManagerScript _gameManager;
    private SpriteRenderer _innerRingFlash;

    /* Constants */
    private const float FlickerTime = 0.2f;

    // Use this for initialization
	void Start ()
	{
	    _gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManagerScript>();
	    _innerRingFlash = transform.GetChild(0).GetComponent<SpriteRenderer>();

	    _timer = 0;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    _timer += Time.smoothDeltaTime;
	    if (_timer > FlickerTime)
	    {
	        _timer = 0;
	        _innerRingFlash.enabled = false;
	    }
	}

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.tag == "Ball")
        {
            _gameManager.AddScore(GameManagerScript.BonusPointScore);
            _innerRingFlash.enabled = true;
        }
    }
}
