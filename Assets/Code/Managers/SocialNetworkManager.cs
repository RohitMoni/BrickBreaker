using Soomla.Profile;
using UnityEngine;

namespace Assets.Code
{
    public class SocialNetworkManager : MonoBehaviour {

        public static SocialNetworkManager Instance { get; private set; }

        /* Properties */

        /* References */
        private MainMenuManagerScript _mainMenuManager;

        /* Constants */
        

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
            _mainMenuManager = GameObject.Find("MainMenuManager").GetComponent<MainMenuManagerScript>();

            /* Register for social network events*/
            ProfileEvents.OnLoginFinished += LogInFinished;

            DontDestroyOnLoad(gameObject);
        }

        // Use this for initialization
        void Start () {

            SoomlaProfile.Initialize();
        }

        public void LogInToFacebook()
        {
            SoomlaProfile.Login(Provider.FACEBOOK);
        }

        public void LogInFinished(UserProfile userProfileJson, string payload)
        {
            _mainMenuManager.SetUserLoggedIn(true);
        }
    }
}
