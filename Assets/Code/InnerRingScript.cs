using Assets.Code;
using UnityEngine;
using System.Collections;

public class InnerRingScript : MonoBehaviour {

    /* Properties */
    private float _timer;
    private int _currentLevel;
    private int _currentHitCounter;

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

	    _currentLevel = -1;
	    _currentHitCounter = 0;
	    _timer = 0;

        IncreaseLevel();
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

    public void IncreaseLevel()
    {
        _currentLevel++;

        Color col;
        switch (_currentLevel)
        {
            case 0:
                col = Color.red;
                break;
            case 1:
                col = Color.yellow;                
                break;
            case 2:
                col = Color.green;
                break;
            case 3:
                col = Color.cyan;
                break;
            default:
                col = Color.red;
                break;
        }

        GetComponent<SpriteRenderer>().color = col;
        _innerRingFlash.color = col;
    }

    public void IncreaseHitCounter()
    {
        _currentHitCounter++;

        if (_currentHitCounter > (_currentLevel+1) * 4)
            _gameManager.GoToNextLevel();
    }

    public void OnCollisionEnter2D(Collision2D collision2D)
    {
        if (collision2D.gameObject.tag == "Ball")
        {
            _gameManager.AddScore(GameManagerScript.BonusPointScore);
            _innerRingFlash.enabled = true;
            IncreaseHitCounter();
        }
    }
}
