using System.Linq;
using UnityEngine;

namespace Assets.Code
{
    public class GameManagerScript : MonoBehaviour
    {
        private int _currentState; // -1 = loss, 0 = playing, 1 = won
        public bool IsPaused;

        /* References */
        private GameObject _startMenu;
        private GameObject _paddleCentre;


        // Use this for initialization
        void Start ()
        {
            IsPaused = false;
            _startMenu = GameObject.FindGameObjectWithTag("StartMenu");
            _paddleCentre = GameObject.FindGameObjectWithTag("PaddleAnchor");
        }
	
        // Update is called once per frame
        void Update () {
            switch (_currentState)
            {
                case -1:    // Switch to lost screen
                    Debug.Log("Player Lost");
                    Application.Quit();
                    break;
                case 1:     // Switch to won screen
                    Debug.Log("Player Won");
                    break;
            }
        }

        public void StartGame()
        {
        }

        public void QuitGame()
        {
            
        }

        private void ResetGame()
        {
            
        }

        public void SetWinLossState(bool state)
        {
            _currentState = (state) ? 1 : -1;
        }
    }
}
