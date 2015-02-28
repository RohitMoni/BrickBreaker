using System.Linq;
using UnityEngine;

namespace Assets.Code
{
    public class GameManagerScript : MonoBehaviour
    {
        private int _currentState; // -1 = loss, 0 = playing, 1 = won

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
            switch (_currentState)
            {
                case -1:    // Switch to lost screen
                    Debug.Log("Player Lost");
                    break;
                case 1:     // Switch to won screen
                    Debug.Log("Player Won");
                    break;
            }
        }

        public void setWinLossState(bool state)
        {
            _currentState = (state) ? 1 : -1;
        }
    }
}
