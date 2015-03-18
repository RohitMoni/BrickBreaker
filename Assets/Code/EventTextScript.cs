using System;
using Assets.Code;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EventTextScript : MonoBehaviour {

    /* Properties */
    private bool _eventEnabled;
    private float _timeToStop;
    private float _timerFlashTime;
    private float _timerTotalTime;
    private int _flashDirection;
    private int _priority;

    /* References */
    private GameManagerScript _gameManager;
    private Image _backPanel;
    private Text _eventText;
    private Text _comboText;

    /* Constants */
    private const float FlashTimeCycle = 1.0f/2f;
    private const float FlashTimeHold = 0.5f;
    private const float PanelMaxAlpha = 200/255f;
    private const float TextMaxAlpha = 255/255f;

	// Use this for initialization
	void Start ()
	{
        _gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManagerScript>();
        _comboText = GameObject.FindGameObjectWithTag("ComboText").GetComponent<Text>();
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

	    _comboText.enabled = false;

        // Update flash direction
	    if (_timerFlashTime >= FlashTimeCycle + FlashTimeHold || _timerFlashTime < 0)
	    {
	        _flashDirection = -1*_flashDirection;
	    }

        // Update timer
        _timerFlashTime += _flashDirection * Time.smoothDeltaTime;
        _timerTotalTime += Time.smoothDeltaTime;

        // Continue flashing
	    var flashFactor = Math.Min(1, (_timerFlashTime/(FlashTimeCycle)));

	    var newColour = _backPanel.color;
	    newColour.a = PanelMaxAlpha * flashFactor;
        _backPanel.color = newColour;

	    newColour = _eventText.color;
	    newColour.a = TextMaxAlpha * flashFactor;
	    _eventText.color = newColour;

        // Check to see if we stop the event
        if (_timeToStop != 0 && _timerTotalTime > _timeToStop)
            StopEvent();
	}

    public void CreateEvent(string textToDisplay, float timeTillStop =0, int priority =0)
    {
        if (priority < _priority)
            return;

        // Reset timers and things
        Reset();

        // Enable the event
        _eventEnabled = true;

        // Set values
        _eventText.text = textToDisplay;
        _timeToStop = timeTillStop;
        _priority = priority;

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

    public string GetCurrentEventText()
    {
        return _eventText.text;
    }

    private void Reset()
    {
        _priority = 0;
        _timeToStop = 0;
        _timerFlashTime = 0;
        _timerTotalTime = 0;
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
