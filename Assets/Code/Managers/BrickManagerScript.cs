using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class BrickManagerScript : MonoBehaviour
    {

        /* References */
        private GameObject _brickAnchor;
        public GameObject BrickPrefab;

        /* Properties */ 

        /* Consts */
        private const float InitialScale = 1.00f;
        private const float FinalScale = 1.00f;
        private const float ScaleUpSpeed = 0.01f;

        // Use this for initialization
        void Start ()
        {
            _brickAnchor = GameObject.FindGameObjectWithTag("BrickAnchor");
        }
	
        // Update is called once per frame
        void Update () {
	        
        }

        void CreateNewBrickRing()
        {
            var gO = new BrickRing(12, this);
            var anchor = gO.Anchor;
            anchor.transform.position.Set(0, 0, -2);
            anchor.transform.localScale.Set(InitialScale, InitialScale, InitialScale);
            anchor.transform.parent = _brickAnchor.transform;
        }

        public void Reset()
        {
            CreateNewBrickRing();
        }

        public class BrickRing
        {
            public GameObject Anchor;
            public List<GameObject> Bricks;

            public BrickRing(int numberOfBricks, BrickManagerScript brickManager)
            {
                Anchor = new GameObject();
                Anchor.name = "Brick Ring Anchor";
                Bricks = new List<GameObject>();

                var brickPrefab = brickManager.BrickPrefab;

                for (var i = 0; i < numberOfBricks; i++)
                {
                    var newBrick = Instantiate(brickPrefab, Vector3.zero, Quaternion.identity) as GameObject;

                    Bricks.Add(newBrick);
                    newBrick.transform.parent = Anchor.transform;
                    newBrick.transform.localEulerAngles = new Vector3(0, 0, 360.0f / numberOfBricks * i);
                    newBrick.transform.localPosition = (newBrick.transform.rotation * new Vector3(0, -1.7f, 0));
                }
            }
        }
    }

    
}
