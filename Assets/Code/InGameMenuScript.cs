using Assets.Code;
using UnityEngine;
using System.Collections;

public class InGameMenuScript : MonoBehaviour
{

    private bool _isEnabled;

    /* References */
    private GameObject _inGameMenu;
    private GameObject _gameAnchor;

	// Use this for initialization
	void Start ()
	{
	    _isEnabled = false;
	    _inGameMenu = transform.GetChild(0).gameObject;
	    _gameAnchor = GameObject.FindGameObjectWithTag("GameAnchor");
	}
	
	// Update is called once per frame
	void Update ()
	{
	    if (Input.GetKeyUp(KeyCode.Escape))
            ToggleInGameMenu();
	}

    public void ToggleInGameMenu()
    {
        _inGameMenu.SetActive(!_isEnabled);
        _isEnabled = !_isEnabled;
        _gameAnchor.GetComponent<GameManagerScript>().IsPaused = _isEnabled;
    }
}
