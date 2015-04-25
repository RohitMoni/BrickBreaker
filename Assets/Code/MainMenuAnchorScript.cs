using UnityEngine;
using System.Collections;

public class MainMenuAnchorScript : MonoBehaviour
{

    private const float RefHeight = 1920;
    private const float RefWidth = 1080;

	// Use this for initialization
	void Start ()
	{
	    var screenHeight = Screen.height;
	    var screenWidth = Screen.width;

	    Vector3 finalMenuScale = Vector3.zero;

	    finalMenuScale.x = screenWidth/RefWidth;
	    finalMenuScale.y = screenHeight/RefHeight;
	    finalMenuScale.z = 1;

	    transform.localScale = finalMenuScale;
	    Camera.main.orthographicSize = finalMenuScale.y*RefHeight/2;
	}
}
