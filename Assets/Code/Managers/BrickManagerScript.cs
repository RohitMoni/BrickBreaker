using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Assets.Code
{
    public class BrickManagerScript : MonoBehaviour
    {
        /* References */
        private GameManagerScript _gameManager;

        private GameObject _brickAnchor;
        public GameObject BrickPrefab;

        /* Properties */
        private List<BrickRing> _brickRings;

        private float _spawnTimer;
        private bool _brickPause;

        /* Consts */
        private const float InitialScale = 0.20f;
        private const float FinalScale = 0.80f;
        private const float ScaleUpSpeed = 0.0002f;
        private const float MaxScaleUp = 1.5f;

        private static readonly Vector3 InitialScaleVec = new Vector3(InitialScale, InitialScale, InitialScale);
        private static readonly Vector3 FinalScaleVec = new Vector3(FinalScale, FinalScale, FinalScale);

        private const float TimeToSetUp = 0.75f;
        private const float TimeToSpawn = 20.00f;

        // Use this for initialization
        void Start ()
        {
            _gameManager = GetComponent<GameManagerScript>();
            _brickAnchor = GameObject.FindGameObjectWithTag("BrickAnchor");
            _brickRings = new List<BrickRing>();
        }
	
        // Update is called once per frame
        void Update ()
        {
            if (_gameManager.IsPaused || _brickPause)
                return;

	        // Update each ring
            for (var i = 0; i < _brickRings.Count; i++)
            {
                var ring = _brickRings[i];

                // Update time
                ring.Time += Time.deltaTime;

                // check to see if time is less than set up time
                if (ring.Time <= TimeToSetUp)
                {
                    ring.Anchor.transform.localScale = Vector3.Lerp(InitialScaleVec, FinalScaleVec, ring.Time);
                }
                else
                {
                    ring.Anchor.transform.localScale += new Vector3(ScaleUpSpeed, ScaleUpSpeed, ScaleUpSpeed);
                }

                if (ring.Anchor.transform.localScale.x > MaxScaleUp)
                {
                    _brickPause = true;
                }
            }

            _spawnTimer += Time.deltaTime;
            if (_spawnTimer >= TimeToSpawn)
            {
                // Create new ring
                CreateNewBrickRing();

                // Reset spawn timer
                _spawnTimer = 0;
            }
        }

        public void CheckRings()
        {
            for (var i = 0; i < _brickRings.Count; i++)
            {
                var ring = _brickRings[i];

                if (ring.Bricks.Count == 0)
                {
                    Destroy(ring.Anchor);
                    _brickRings.RemoveAt(i);

                    if (i == 0)
                        _brickPause = false;
                }
            }
        }

        public void CleanUp()
        {
            // Clean up
            // Clean up brick rings
            foreach (var ring in _brickRings)
            {
                Destroy(ring.Anchor);
            }

            _brickRings.Clear();
        }

        public void StartUp()
        {
            // Create new ones
            CreateNewBrickRing();
        }

        void CreateNewBrickRing()
        {
            var gO = new BrickRing(12, this);
            var anchor = gO.Anchor;
            anchor.transform.position = new Vector3(0, 0, -2);
            anchor.transform.localScale = new Vector3(InitialScale, InitialScale, InitialScale);
            anchor.transform.parent = _brickAnchor.transform;

            _brickRings.Add(gO);
        }

        public class BrickRing
        {
            public GameObject Anchor;
            public List<GameObject> Bricks;

            public float Time;

            public BrickRing(int numberOfBricks, BrickManagerScript brickManager)
            {
                Anchor = new GameObject();
                Anchor.name = "Brick Ring Anchor";
                Bricks = new List<GameObject>();

                var brickPrefab = brickManager.BrickPrefab;

                for (var i = 0; i < numberOfBricks; i++)
                {
                    var newBrick = Instantiate(brickPrefab, Vector3.zero, Quaternion.identity) as GameObject;

                    newBrick.transform.parent = Anchor.transform;
                    newBrick.transform.localEulerAngles = new Vector3(0, 0, 360.0f / numberOfBricks * i);
                    newBrick.transform.localPosition = (newBrick.transform.rotation * new Vector3(0, -1.7f, 0));

                    Bricks.Add(newBrick);
                }
            }
        }
    }

    
}
