using UnityEngine;
using System.Collections;

public class GameVariablesScript : MonoBehaviour {

    /* Constants */
    private const int DefaultSensitivity = 5;
    private const int DefaultBallSpeed = 5;
    private const bool DefaultMusicMuted = false;
    private const bool DefaultSoundEffectsMuted = false;
    private const int DefaultControlScheme = 1;
    public const int MaxBallSpeed = 15;
    public const int MinBallSpeed = 3;
    public static float BallSpeedCoeff = 2f;
    public static float PaddleSensitivityCoeff = 50f;

    public static string GameFile = "playerData.dat";

    /* Game Properties */
    public static int ControlScheme = DefaultControlScheme;
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
        ControlScheme = DefaultControlScheme;
        PaddleSensitivity = DefaultSensitivity/PaddleSensitivityCoeff;
        BallSpeed = DefaultBallSpeed/BallSpeedCoeff;
        HighScore = LastScore = 0;
    }

    public static string ConvertScoreToString(int score)
    {
        string scoreText = "";

        if (score > 999999)
        {
            scoreText += score/1000000;
            scoreText += ".";
            scoreText += (score%1000000) / 1000;
            scoreText += "M";
        }
        else if (score > 50000)
        {
            scoreText += score/1000;
            scoreText += ".";
            scoreText += (score%1000)/100;
            scoreText += "k";
        }
        else
        {
            scoreText += score.ToString();
        }

        return scoreText;
    }
}