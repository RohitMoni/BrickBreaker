using UnityEngine;
using System.Collections;

public class GameVariablesScript : MonoBehaviour {

    /* Constants */
    private const bool DefaultRelativePaddle = false;
    private const bool DefaultSliderMovement = true;
    private const int DefaultSensitivity = 5;
    private const int DefaultBallSpeed = 5;
    private const bool DefaultMusicMuted = false;
    private const bool DefaultSoundEffectsMuted = false;
    public const int MaxBallSpeed = 15;
    public const int MinBallSpeed = 3;
    public static float BallSpeedCoeff = 2f;
    public static float PaddleSensitivityCoeff = 50f;

    public static string GameFile = "playerData.dat";

    /* Game Properties */
    public static bool RelativePaddle = DefaultRelativePaddle;
    public static bool SliderMovement = DefaultSliderMovement;
    public static float PaddleSensitivity = DefaultSensitivity / PaddleSensitivityCoeff;
    public static float BallSpeed = DefaultBallSpeed / BallSpeedCoeff;
    public static bool MusicMuted = DefaultMusicMuted;
    public static bool SoundEffectsMuted = DefaultSoundEffectsMuted;

    /* Other Global Variables */
    public static int ScreenToStartOn = 0;
    public static int HighScore = 0;
    public static int LastScore = 0;

    /* Functions */
    public static void ResetVariables()
    {
        RelativePaddle = DefaultRelativePaddle;
        SliderMovement = DefaultSliderMovement;
        PaddleSensitivity = DefaultSensitivity/PaddleSensitivityCoeff;
        BallSpeed = DefaultBallSpeed/BallSpeedCoeff;
        HighScore = LastScore = 0;
    }
}
