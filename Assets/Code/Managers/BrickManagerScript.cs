using UnityEngine;

namespace Assets.Code
{
    public class BrickManagerScript : MonoBehaviour
    {

        public GameObject BrickRingPrefab;

        private const float InitialScale = 0.15f;
        private const float FinalScale = 1.00f;
        private const float ScaleUpSpeed = 0.01f;

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
	
        }

        void CreateNewBrickRing()
        {
            var gO = Instantiate(BrickRingPrefab) as GameObject;
            gO.transform.parent = transform;

            gO.transform.position.Set(0, 0, 0);
            gO.transform.localScale.Set(InitialScale, InitialScale, InitialScale);
        }

        public void Reset()
        {
            
        }
    }
}
