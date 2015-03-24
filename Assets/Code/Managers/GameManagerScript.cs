using System;
using UnityEngine;
using UnityEngine.UI;

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
        private int _backgroundColourIndex;

        // Combo variables
        private float _comboValue;
        private float _comboTimer;

        // Camera shake variables
        private Vector3 _cameraOriginalPosition;
        private float _radius;
        private float _randomAngle;
        private Vector2 _offset;
        private bool _isCameraShaking;

        /* References */
        private Camera _camera;
        private PaddleManagerScript _paddleManager;
        private BrickManagerScript  _brickManager;
        private EventTextScript _eventManager;
        private GameObject _inGameMenu;
        private GameObject _controlSlider;
        private SpriteRenderer _backgroundImage;
        private Text _scoreText;
        private Text _timeText;
        private Text _comboText;
        private Text _debugText;

        /* Constants */
        public const int BonusPointScore = 200;
        public const float TimeForSpeedIncrease = 30;
        public const float TimeForSpeedIncreaseEvent = 5f;
        public const float TimeForComboTextShow = 2f;
        public Vector3 ComboTextInitialScale = new Vector3(.5f, .5f, .5f);
        public Vector3 ComboTextFinalScale = new Vector3(1f, 1f, 1f);
        public const float BackgroundColourChangeSpeed = 3;

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
            _comboTimer = 0;
            _backgroundColourIndex = 1;

            // Camera shake
            _camera = Camera.main;
            
            _paddleManager = GetComponent<PaddleManagerScript>();
            _brickManager = GetComponent<BrickManagerScript>();
            _eventManager = GameObject.FindGameObjectWithTag("EventText").GetComponent<EventTextScript>();
            _inGameMenu = GameObject.FindGameObjectWithTag("InGameMenu");
            _controlSlider = GameObject.FindGameObjectWithTag("ControlSlider");
            _scoreText = GameObject.FindGameObjectWithTag("ScoreText").GetComponent<Text>();
            _timeText = GameObject.FindGameObjectWithTag("TimeText").GetComponent<Text>();
            _comboText = GameObject.FindGameObjectWithTag("ComboText").GetComponent<Text>();
            _backgroundImage = GameObject.FindGameObjectWithTag("BackgroundImage").GetComponent<SpriteRenderer>();

            _comboText.enabled = false;

            var obj = GameObject.FindGameObjectWithTag("DebugText");
            if (obj)
                _debugText = obj.GetComponent<Text>();
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

            #region Editor only
#if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.Return))
            {
                StartShake();
            }

            if (Input.GetKeyUp(KeyCode.Escape) && !IsPaused)
            {
                TogglePause();
            }
#endif
            #endregion

            switch (_currentState)
            {
                case -1:    // Switch to lost screen
                    StopGame();
                    break;
                case 1:     // Switch to won screen
                    StopGame();
                    break;
            }

            if (IsPaused)
                return;

            // Update score
            _textPointScore += Mathf.CeilToInt((_actualPointScore - _textPointScore)/2f);
            _scoreText.text = _textPointScore.ToString();

            // Update time
            if (TimerStarted)
            {
                #region Timer Text
                _timer += Time.smoothDeltaTime;
                var minutes = Mathf.Floor(_timer/60);
                var seconds = (_timer%60);
                _timeText.text = minutes.ToString("00") + ":" + seconds.ToString("00");
                #endregion

                #region Ball Speed Increase
                // Update the timer that increases ball speed
                _speedIncreaseTimer += Time.smoothDeltaTime;
                if (!_eventCreated && _speedIncreaseTimer > TimeForSpeedIncrease - TimeForSpeedIncreaseEvent)
                {
                    _eventManager.CreateEvent("Speed++", TimeForSpeedIncreaseEvent, 1);
                    _eventCreated = true;
                }
                else if (_speedIncreaseTimer >= TimeForSpeedIncrease)
                {
                    IncreaseBallSpeed();
                    _speedIncreaseTimer = 0;
                    _eventCreated = false;
                }
                #endregion
            }

            #region Camera Shake
            if (_isCameraShaking)
                UpdateShake();
            #endregion

            #region Combo
            if (_comboText.enabled)
            {
                _comboTimer += Time.smoothDeltaTime;
                _comboText.transform.localScale = Vector3.Lerp(ComboTextInitialScale, ComboTextFinalScale,
                    _comboTimer/TimeForComboTextShow * 2);

                if (_comboText.transform.localScale.x > 1)
                    _comboText.transform.localScale = new Vector3(1, 1, 1);

                if (_comboTimer > TimeForComboTextShow)
                {
                    _comboTimer = 0;
                    _comboText.enabled = false;
                }
            }
            #endregion

            #region Background Colour Shift

            var colour = _backgroundImage.color;
            switch (_backgroundColourIndex)
            {
                case 0:
                    colour.r += BackgroundColourChangeSpeed / 255f;
                    if (colour.r >= 1)
                        _backgroundColourIndex++;
                    break;
                case 1:
                    colour.b -= BackgroundColourChangeSpeed / 255f;
                    if (colour.b <= 0)
                        _backgroundColourIndex++;
                    break;
                case 2:
                    colour.g += BackgroundColourChangeSpeed / 255f;
                    if (colour.g >= 1)
                        _backgroundColourIndex++;
                    break;
                case 3:
                    colour.r -= BackgroundColourChangeSpeed / 255f;
                    if (colour.r <= 0)
                        _backgroundColourIndex++;
                    break;
                case 4:
                    colour.b += BackgroundColourChangeSpeed / 255f;
                    if (colour.b >= 1)
                        _backgroundColourIndex++;
                    break;
                case 5:
                    colour.g -= BackgroundColourChangeSpeed / 255f;
                    if (colour.g <= 0)
                        _backgroundColourIndex = 0;
                    break;
            }
            _backgroundImage.color = colour;

            #endregion
        }

        public void StartShake()
        {
            _isCameraShaking = true;
            _cameraOriginalPosition = _camera.transform.position;
            _radius = 30.0f;
            _randomAngle = UnityEngine.Random.Range(0, 360);
            _offset = new Vector2((float)Math.Sin(_randomAngle) * _radius * 0.01f, (float)Math.Cos(_randomAngle) * _radius * 0.01f);
            _camera.transform.position += new Vector3(_offset.x, _offset.y, 0);
        }

        private void UpdateShake()
        {
            _radius *= 0.9f;
            _randomAngle += (180 + UnityEngine.Random.Range(0, 60));
            _offset = new Vector2((float)Math.Sin(_randomAngle) * _radius * 0.01f, (float)Math.Cos(_randomAngle) * _radius * 0.01f);
            _camera.transform.position += new Vector3(_offset.x, _offset.y, 0);

            if (_radius <= 2)
            {
                _isCameraShaking = false;
                _camera.transform.position = _cameraOriginalPosition;
            }
        }

        public void AddToComboValue()
        {
            _comboValue = Math.Min(_comboValue + 1, 20);

            switch ((int)_comboValue)
            {
                case 3:
                    StartComboText(3);
                    break;
                case 5:
                    StartComboText(5);
                    break;
                case 10:
                    StartComboText(10);
                    break;
                case 15:
                    StartComboText(15);
                    break;
                case 20:
                    StartComboText(20);
                    break;
            }
        }

        public void ResetComboValue()
        {
            _comboValue = 0;
        }

        public void StartComboText(int comboScore)
        {
            _comboText.enabled = true;
            _comboText.transform.localScale = ComboTextInitialScale;
            _comboText.text = "Combo\nx" + comboScore;
            _comboTimer = 0;
        }

        public void AddScore(int scoreToAdd)
        {
            _actualPointScore += scoreToAdd * (int)Math.Max(_comboValue, 1);
        }

        public void IncreaseBallSpeed()
        {
            var currentSpeed = GameVariablesScript.BallSpeed*GameVariablesScript.BallSpeedCoeff;
            currentSpeed++;

            currentSpeed = Math.Min(currentSpeed, GameVariablesScript.MaxBallSpeed);
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

            FileServices.SaveGame();

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
            SetPaddleMovementSliderActive(!IsPaused && GameVariablesScript.SliderMovement);
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
            _comboTimer = 0;

            SetInGameMenuActive(false);
            _comboText.enabled = false;
            SetPaddleMovementSliderActive(GameVariablesScript.SliderMovement);
        }

        public void SetWinLossState(bool state)
        {
            _currentState = (state) ? 1 : -1;
        }

        public void Debug(string text)
        {
            if (_debugText)
                _debugText.text = text;
        }
    }
}
