using Assets.Code;
using UnityEngine;
using System.Collections;

public class MenuManagerScript : MonoBehaviour {

    /* References */
    private GameObject _startMenu;
    private GameObject _optionsMenu;
    private GameObject _inGameMenu;

	// Use this for initialization
	void Start ()
	{
	    _startMenu = GameObject.FindGameObjectWithTag("StartMenu");
	    _optionsMenu = GameObject.FindGameObjectWithTag("OptionsMenu");
	    _inGameMenu = GameObject.FindGameObjectWithTag("InGameMenu");

	    _optionsMenu.SetActive(false);
	    _inGameMenu.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {

	}

    public void StartMenuToGame()
    {
        _startMenu.SetActive(false);
        _inGameMenu.SetActive(false);
    }

    public void StartMenuToOptions()
    {
        _startMenu.SetActive(false);
        _optionsMenu.SetActive(true);
    }

    public void OptionsToStartMenu()
    {
        _optionsMenu.SetActive(false);
        _startMenu.SetActive(true);
    }

    public void GameToStartMenu()
    {
        _inGameMenu.SetActive(false);
        _startMenu.SetActive(true);
    }

    public void SetInGameMenuActive(bool isActive)
    {
        _inGameMenu.SetActive(isActive);
    }
}
