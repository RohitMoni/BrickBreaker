using System.Linq;
using UnityEngine;

namespace Assets.Code
{
    public class GameManagerScript : MonoBehaviour
    {
        /* Properties */
        private int _currentState; // -1 = loss, 0 = playing, 1 = won
        public float GameSpeed;
        public bool IsPaused;

        /* References */
        private MenuManagerScript   _menuManager;
        private PaddleManagerScript _paddleManager;
        private BrickManagerScript  _brickManager;

        private const float DefaultGameSpeed = 0.03f;
        public static float GameSpeedFactor = 1.0f;

        // Use this for initialization
        void Start ()
        {
            GameSpeed = DefaultGameSpeed;
            IsPaused = true;
            _currentState = 0;
            _menuManager = GetComponent<MenuManagerScript>();
            _paddleManager = GetComponent<PaddleManagerScript>();
            _brickManager = GetComponent<BrickManagerScript>();
        }
	
        // Update is called once per frame
        void Update () {
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
        }

        public void SetGameSpeed(float sliderValue)
        {
            GameSpeed = sliderValue / 100f;
            GameSpeedFactor = GameSpeed/DefaultGameSpeed;
        }

        public void StartGame()
        {
            _paddleManager.Reset();
            _paddleManager.CreateNewBall();
            _brickManager.StartUp();
            IsPaused = false;
            // Move from start menu to ingame
            _menuManager.StartMenuToGame();

            _currentState = 0;
        }

        public void StopGame()
        {
            _paddleManager.Reset();
            _brickManager.CleanUp();
            IsPaused = true;
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
            
        }

        public void SetWinLossState(bool state)
        {
            Debug.Log("Game " + (state? "Won" : "Lost"));
            _currentState = (state) ? 1 : -1;
        }
    }
}
