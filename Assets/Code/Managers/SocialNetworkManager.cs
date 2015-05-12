using Soomla.Profile;
using UnityEngine;

namespace Assets.Code
{
    public class SocialNetworkManager : MonoBehaviour {

        public static SocialNetworkManager Instance { get; private set; }

        void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;

            DontDestroyOnLoad(gameObject);
        }

        // Use this for initialization
        void Start () {

            SoomlaProfile.Initialize();
        }
	

    }
}
