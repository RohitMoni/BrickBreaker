using Assets.Code;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EventTextScript : MonoBehaviour {

    /* Properties */
    private bool _eventEnabled;
    private float _timeToStop;
    private float _timerCurrentTime;
    private int _flashDirection;

    /* References */
    private GameManagerScript _gameManager;
    private Image _backPanel;
    private Text _eventText;

    /* Constants */
    private const float FlashTimeCycle = 1.0f;
    private const float Threshold = 0.1f;
    private const int PanelMaxAlpha = 150;
    private const int TextMaxAlpha = 255;

	// Use this for initialization
	void Start ()
	{
        _gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManagerScript>();
        _backPanel = GetComponent<Image>();
        _eventText = transform.GetChild(0).gameObject.GetComponent<Text>();

	    Reset();

	    _backPanel.enabled = false;
	    _eventText.enabled = false;
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (_gameManager.IsPaused || !_eventEnabled)
	        return;

        // Update timer
	    _timerCurrentTime += Time.deltaTime;

        // Update flash direction
	    if (_timerCurrentTime/(FlashTimeCycle/2f) <= Threshold)
	        _flashDirection = -1*_flashDirection;

        // Check to see if we stop the event
	    if (_timeToStop != 0 && _timerCurrentTime > _timeToStop)
	        StopEvent();

        // Continue flashing
	    var flashFactor = (_flashDirection*(Time.deltaTime/(FlashTimeCycle/2f)));

	    var newColour = _backPanel.color;
	    newColour.a += PanelMaxAlpha * flashFactor;
        _backPanel.color = newColour;

	    newColour = _eventText.color;
	    newColour.a += TextMaxAlpha * flashFactor;
	    _eventText.color = newColour;
	}

    public void CreateEvent(string textToDisplay, float timeTillStop =0)
    {
        // Reset timers and things
        Reset();

        // Enable the event
        _eventEnabled = true;

        // Set values
        _eventText.text = textToDisplay;
        _timeToStop = timeTillStop;

        // Enable stuff to show
        _backPanel.enabled = true;
        _eventText.enabled = true;
    }

    public void StopEvent()
    {
        // Disable stuff
        _backPanel.enabled = false;
        _eventText.enabled = false;

        // Reset timers and things
        Reset();
    }

    private void Reset()
    {
        _timeToStop = 0;
        _timerCurrentTime = 0;
        _flashDirection = 1;
        _eventEnabled = false;

        var colour = _backPanel.color;
        colour.a = 0;
        _backPanel.color = colour;
        colour = _eventText.color;
        colour.a = 0;
        _eventText.color = colour;
    }
}
