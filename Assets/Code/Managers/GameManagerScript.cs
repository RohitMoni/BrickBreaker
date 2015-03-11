using System.Linq;
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
        public bool TimerStarted;

        private int _textPointScore;
        private int _actualPointScore;
        private float _timer;
        private float _speedIncreaseTimer;
        private bool _eventCreated;

        /* References */
        private PaddleManagerScript _paddleManager;
        private BrickManagerScript  _brickManager;
        private EventTextScript _eventManager;
        private GameObject _inGameMenu;
        private GameObject _controlSlider;
        private Text _scoreText;
        private Text _timeText;

        /* Constants */
        public const int BonusPointScore = 200;
        public const float TimeForSpeedIncrease = 45;
        public const float TimeForSpInEvent = 2.5f;

        // Use this for initialization
        void Start ()
        {
            IsPaused = true;
            IsStarted = false;
            TimerStarted = false;
            _currentState = 0;
            _textPointScore = _actualPointScore = 0;
            _timer = 0;
            _speedIncreaseTimer = 0;
            _eventCreated = false;

            _paddleManager = GetComponent<PaddleManagerScript>();
            _brickManager = GetComponent<BrickManagerScript>();
            _eventManager = GameObject.FindGameObjectWithTag("EventText").GetComponent<EventTextScript>();
            _inGameMenu = GameObject.FindGameObjectWithTag("InGameMenu");
            _controlSlider = GameObject.FindGameObjectWithTag("ControlSlider");
            _scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
            _timeText = GameObject.FindGameObjectWithTag("TimeText").GetComponent<Text>();
        }

        void Awake()
        {
            Application.targetFrameRate = 45;
        }
	
        // Update is called once per frame
        void Update ()
        {
            if (!IsStarted)
                StartGame();

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
            if (TimerStarted)
            {
                _timer += Time.smoothDeltaTime;
                var minutes = Mathf.Floor(_timer/60);
                var seconds = (_timer%60);
                _timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");

                // Update the timer that increases ball speed
                _speedIncreaseTimer += Time.smoothDeltaTime;
                if (!_eventCreated && _speedIncreaseTimer > TimeForSpeedIncrease - TimeForSpInEvent)
                {
                    _eventManager.CreateEvent("Speed++", TimeForSpInEvent);
                    _eventCreated = true;
                }
                else if (_speedIncreaseTimer >= TimeForSpeedIncrease)
                {
                    IncreaseBallSpeed();
                    _speedIncreaseTimer = 0;
                    _eventCreated = false;
                }
            }
        }

        public void AddScore(int scoreToAdd)
        {
            _actualPointScore += scoreToAdd;
        }

        public void IncreaseBallSpeed()
        {
            var currentSpeed = GameVariablesScript.BallSpeed*GameVariablesScript.BallSpeedCoeff;
            currentSpeed++;
            GameVariablesScript.BallSpeed = currentSpeed / GameVariablesScript.BallSpeedCoeff;
        }

        public void StartGame()
        {
            ResetGame();
            _paddleManager.CreateNewBall();
            _brickManager.StartUp();

            IsStarted = true;   // To make sure we don't run this over and over again
        }

        public void StopGame()
        {
            GameVariablesScript.LastScore = _actualPointScore;
            if (_actualPointScore > GameVariablesScript.HighScore)
                GameVariablesScript.HighScore = _actualPointScore;

            ResetGame();

            // Make the main menu scene load up the score screen first
            GameVariablesScript.ScreenToStartOn = 2;

            // Disable the ingame menu and enable the start menu
            Application.LoadLevel("MainMenuScene");
        }

        public void TogglePause()
        {
            IsPaused = !IsPaused;
            SetInGameMenuActive(IsPaused);
        }

        private void SetInGameMenuActive(bool active)
        {
            _inGameMenu.SetActive(active);
        }

        private void SetPaddleMovementSliderActive(bool active)
        {
            _controlSlider.SetActive(active);
        }

        private void ResetGame()
        {
            _actualPointScore = _textPointScore = 0;

            _paddleManager.Reset();
            _brickManager.CleanUp();
            IsPaused = false;
            TimerStarted = false;
            _currentState = 0;

            SetInGameMenuActive(false);
            SetPaddleMovementSliderActive(GameVariablesScript.SliderMovement);
        }

        public void SetWinLossState(bool state)
        {
            Debug.Log("Game " + (state? "Won" : "Lost"));
            _currentState = (state) ? 1 : -1;
        }
    }
}
