﻿using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;
using Timer = System.Timers.Timer;

namespace Assets.Code
{
    public class GameManagerScript : MonoBehaviour
    {
        /* Properties */
        private int _currentState; // -1 = loss, 0 = playing, 1 = won
        public bool IsPaused;
        public bool IsStarted;

        public float GameSpeed;
        private int _textPointScore;
        private int _actualPointScore;
        private float _timer;

        /* References */
        private MenuManagerScript   _menuManager;
        private PaddleManagerScript _paddleManager;
        private BrickManagerScript  _brickManager;
        private EventTextScript     _eventManager;
        private Text _scoreText;
        private Text _timeText;

        /* Constants */
        private const float DefaultGameSpeed = 0.03f;
        public static float GameSpeedFactor = 1.0f;

        public const int BonusPointScore = 50;

        // Use this for initialization
        void Start ()
        {
            GameSpeed = DefaultGameSpeed;
            IsPaused = true;
            _currentState = 0;
            _textPointScore = _actualPointScore = 0;
            _timer = 0;

            _menuManager = GetComponent<MenuManagerScript>();
            _paddleManager = GetComponent<PaddleManagerScript>();
            _brickManager = GetComponent<BrickManagerScript>();
            _eventManager = GameObject.FindGameObjectWithTag("EventText").GetComponent<EventTextScript>();
            _scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
            _timeText = GameObject.FindGameObjectWithTag("TimeText").GetComponent<Text>();
        }
	
        // Update is called once per frame
        void Update ()
        {
            switch (_currentState)
            {
                case -1:    // Switch to lost screen
                    StopGame();
                    break;
                case 1:     // Switch to won screen
                    StopGame();
                    break;
            }

            if (Input.GetKeyUp(KeyCode.Escape) && !IsPaused)
            {
                TogglePause();
            }

            if (IsPaused)
                return;

            // Update score
            _textPointScore += Mathf.CeilToInt((_actualPointScore - _textPointScore)/2f);
            _scoreText.text = _textPointScore.ToString();

            // Update time
            if (IsStarted)
            {
                _timer += Time.deltaTime;
                var minutes = Mathf.Floor(_timer/60);
                var seconds = (_timer%60);
                _timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
            }
        }

        public void AddScore(int scoreToAdd)
        {
            _actualPointScore += scoreToAdd;
        }

        public void SetGameSpeed(float sliderValue)
        {
            GameSpeed = sliderValue / 100f;
            GameSpeedFactor = GameSpeed/DefaultGameSpeed;
        }

        public void StartGame()
        {
            ResetGame();
            _paddleManager.Reset();
            _paddleManager.CreateNewBall();
            _brickManager.StartUp();
            IsPaused = false;
            IsStarted = false;

            // Move from start menu to ingame
            _menuManager.StartMenuToGame();

            _currentState = 0;
        }

        public void StopGame()
        {
            ResetGame();

            // Disable the ingame menu and enable the start menu
            _menuManager.GameToStartMenu();

            _currentState = 0;
        }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
            _menuManager.SetInGameMenuActive(IsPaused);
        }

        public void QuitGame()
        {
            Application.Quit();
        }

        private void ResetGame()
        {
            _actualPointScore = _textPointScore = 0;

            _paddleManager.Reset();
            _brickManager.CleanUp();
            IsPaused = true;
        }

        public void SetWinLossState(bool state)
        {
            Debug.Log("Game " + (state? "Won" : "Lost"));
            _currentState = (state) ? 1 : -1;
        }
    }
}
