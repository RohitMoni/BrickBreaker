using Assets.Code;
using UnityEngine;
using System.Collections;

public class MenuManagerScript : MonoBehaviour {

    /* References */
    private GameManagerScript _gameManager;

    private GameObject _startMenu;
    private GameObject _inGameMenu;

	// Use this for initialization
	void Start ()
	{
	    _gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManagerScript>();
	    _startMenu = GameObject.FindGameObjectWithTag("StartMenu");
	    _inGameMenu = GameObject.FindGameObjectWithTag("InGameMenu");
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
