using UnityEngine;
using System.Collections;

public class GameVariablesScript : MonoBehaviour {

    /* Constants */
    private const bool DefaultRelativePaddle = true;
    private const int DefaultSensitivity = 5;
    private const int DefaultBallSpeed = 5;

    /* Properties */
    public static float BallSpeedCoeff = 100f;

    public static bool RelativePaddle = DefaultRelativePaddle;
    public static int Sensitivity = DefaultSensitivity;
    public static float BallSpeed = DefaultBallSpeed / BallSpeedCoeff;
    public static int LastPointScore = 0;

}
