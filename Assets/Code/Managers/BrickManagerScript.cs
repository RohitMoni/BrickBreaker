using System;
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

        private bool _brickPause;

        /* Consts */
        private const int NumberOfRings = 3;

        private const float InitialScale = 0.20f;
        private readonly float[] _ringScaleLevels = { 1.7f, 1.22f, 0.74f };

        private const float ScaleUpSpeed = 0.02f;

        private const float RingRotationSpeed = 0.1f;

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
#if UNITY_EDITOR
            if (Input.GetKeyUp(KeyCode.Tab))
            {
                var ring = _brickRings[0];

                foreach (Transform brick in ring.Anchor.transform)
                    Destroy(brick.gameObject);
            }
#endif
            if (_gameManager.IsPaused || _brickPause)
                return;

            CheckRings();

            // Check to see if there are the requisite number of brick rings
            if (_brickRings.Count < NumberOfRings)
            {
                // If not, spawn an equivalent number of brick rings
                for (var i = 0; i < NumberOfRings - _brickRings.Count; i++)
                    CreateNewBrickRing();
            }
            
            // Update each brick ring
            var ringRotation = 1;
            for (var i = 0; i < _brickRings.Count; i++)
            {
                // Check to see if the brick ring is 'set up' = It's scale is the right level
                if (_brickRings[i].Anchor.transform.localScale.x < _ringScaleLevels[i])
                {
                    // If not, we need to push the ring out
                    _brickRings[i].Anchor.transform.localScale += new Vector3(ScaleUpSpeed, ScaleUpSpeed, ScaleUpSpeed);
                    var scaleVal = _brickRings[i].Anchor.transform.localScale.x;
                    foreach (Transform child in _brickRings[i].Anchor.transform)
                    {
                        var scale = child.localScale;
                        scale.y = 1 / scaleVal;
                        child.localScale = scale;
                    }
                }

                // Do other effects with brick rings

                // Rotate rings
                _brickRings[i].Anchor.transform.Rotate(Vector3.forward, RingRotationSpeed * ringRotation);
                ringRotation *= -1;
            }
        }

        public void CheckRings()
        {
            for (var i = 0; i < _brickRings.Count; i++)
            {
                var ring = _brickRings[i];

                var count = ring.Anchor.transform.childCount;

                if (count == 0)
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

            // reset brick pause
            _brickPause = false;
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
            public float Time;

            public BrickRing(int numberOfBricks, BrickManagerScript brickManager)
            {
                Anchor = new GameObject {name = "Brick Ring Anchor"};

                var brickPrefab = brickManager.BrickPrefab;

                for (var i = 0; i < numberOfBricks; i++)
                {
                    var newBrick = Instantiate(brickPrefab, Vector3.zero, Quaternion.identity) as GameObject;

                    newBrick.GetComponent<BrickScript>().Initialise();

                    newBrick.transform.parent = Anchor.transform;
                    newBrick.transform.localEulerAngles = new Vector3(0, 0, 360.0f / numberOfBricks * i);
                    newBrick.transform.localPosition = (newBrick.transform.rotation * new Vector3(0, -1.7f, 0));
                }
            }
        }
    }

    
}
