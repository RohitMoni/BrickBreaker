using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class IsUsingSliderScript : MonoBehaviour {

    /* References */
    private MainMenuManagerScript _mainMenuManager;

	// Use this for initialization
	void Start ()
	{
	    _mainMenuManager = GameObject.FindGameObjectWithTag("MainMenuManager").GetComponent<MainMenuManagerScript>();
	}
	
	// Update is called once per frame
    private void Update()
    {
        foreach (var touch in Input.touches)
        {
            if (RectTransformUtility.RectangleContainsScreenPoint(GetComponent<Slider>().handleRect, touch.rawPosition, Camera.main))
                _mainMenuManager.IsUsingSlider = true;
        }
    }
}
