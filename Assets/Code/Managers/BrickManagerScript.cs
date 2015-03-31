using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        public int BrickHealth;
        private bool _brickPause;
        private bool _isShockwaving;
        private float _shockwaveTimer;
        private int _shockwaveCounter;
        private bool _powerMode;
        private float _powerModeTimer;
        private bool _isFlashing;

        /* Consts */
        private const int NumberOfRings = 3;

        private const float InitialScale = 0.20f;
        private readonly float[] _ringScaleLevels = { 1.7f, 1.22f, 0.74f };
        private const float ScaleUpSpeed = 0.02f;

        private const float RingRotationSpeed = 0.2f;

        private const float ShockwaveSpeed = 0.2f;

        private const float TimeForPowerMode = 5.0f;
        private const float TimeForPowerFlashing = 2.0f;

        // Use this for initialization
        void Start ()
        {
            _gameManager = GetComponent<GameManagerScript>();
            _brickAnchor = GameObject.FindGameObjectWithTag("BrickAnchor");
            _brickRings = new List<BrickRing>();
            BrickHealth = 1;
            _powerMode = false;
            _isFlashing = false;
        }
	
        // Update is called once per frame
        void Update ()
        {
            if (_gameManager.IsPaused || _brickPause)
                return;

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

            // Power Mode
            if (_powerMode)
            {
                _powerModeTimer += Time.smoothDeltaTime;
                if (_powerModeTimer > TimeForPowerMode - TimeForPowerFlashing && !_isFlashing)
                {
                    _isFlashing = true;
                    StartCoroutine(FlashBallColour(5, TimeForPowerFlashing/5/2, TimeForPowerFlashing/5/2));
                }

                if (_powerModeTimer > TimeForPowerMode)
                {
                    SetPowerModeEnabled(false);
                }
            }

            // shockwave
            if (_isShockwaving)
            {
                _shockwaveTimer += Time.smoothDeltaTime;

                if (_shockwaveTimer > ShockwaveSpeed)
                {
                    // Destroy ring
                    var ring = _brickRings[_shockwaveCounter-1];

                    foreach (Transform brick in ring.Anchor.transform)
                    {
                        if (brick.gameObject.activeSelf)
                            brick.GetComponent<BrickScript>().DestroyBrick();
                    }

                    // Reset Timer and Increment counter
                    _shockwaveTimer = 0;
                    _shockwaveCounter++;

                    if (_shockwaveCounter > _brickRings.Count)
                    {
                        _isShockwaving = false;
                        CheckRings();
                    }
                }
            }
        }

        public void CheckRings()
        {
            var ringsToRecycle = new List<BrickRing>(_brickRings.Count);

            for (var i = 0; i < _brickRings.Count; i++)
            {
                var ring = _brickRings[i];

                var count = ring.Anchor.transform.Cast<Transform>().Count(child => child.gameObject.activeSelf);

                if (count == 0) // If there are no active children, we reset the ring and shift it to the start of the list
                {
                    ringsToRecycle.Add(ring);
                }
            }

            foreach (var ring in ringsToRecycle)
            {
                ResetBrickRing(ring);

                _brickRings.Remove(ring);
                _brickRings.Add(ring);
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
            for (var i = 0; i < NumberOfRings; i++)
                CreateNewBrickRing();
        }

        public void StartShockwave()
        {
            _shockwaveCounter = 1;
            _isShockwaving = true;
            _shockwaveTimer = 0;
            _gameManager.StartShake();
        }

        public void SetPowerModeEnabled(bool powerModeIsEnabled)
        {
            foreach (var ring in _brickRings)
            {
                foreach (Transform brick in ring.Anchor.transform)
                {
                    brick.gameObject.GetComponent<Collider2D>().isTrigger = powerModeIsEnabled;
                }
            }

            _powerModeTimer = 0;
            _powerMode = powerModeIsEnabled;
            _isFlashing = !_powerMode;
            var balls = GameObject.FindGameObjectsWithTag("Ball");
            foreach (var ball in balls)
                ball.GetComponent<SpriteRenderer>().color = _powerMode ? Color.red : Color.white;
        }
        
        void CreateNewBrickRing()
        {
            var gO = new BrickRing(12, BrickHealth, this);
            var anchor = gO.Anchor;
            anchor.transform.position = new Vector3(0, 0, -2);
            anchor.transform.localScale = new Vector3(InitialScale, InitialScale, InitialScale);
            anchor.transform.parent = _brickAnchor.transform;

            _brickRings.Add(gO);
        }

        private IEnumerator FlashBallColour(int nTimes, float timeOn, float timeOff)
        {
            var balls = GameObject.FindGameObjectsWithTag("Ball");
            while (nTimes > 0)
            {
                foreach (var ball in balls.Where(ball => ball))
                    ball.GetComponent<SpriteRenderer>().color = Color.white;

                if (!_isFlashing)
                    yield break;

                yield return new WaitForSeconds(timeOn);

                foreach (var ball in balls.Where(ball => ball))
                    ball.GetComponent<SpriteRenderer>().color = Color.red;

                if (!_isFlashing)
                    yield break;

                yield return new WaitForSeconds(timeOff);
                nTimes--;
            }

            foreach (var ball in balls.Where(ball => ball))
                ball.GetComponent<SpriteRenderer>().color = Color.white;
        }

        void ResetBrickRing(BrickRing ring)
        {
            var anchor = ring.Anchor;
            anchor.transform.position = new Vector3(0, 0, -2);
            anchor.transform.localScale = new Vector3(InitialScale, InitialScale, InitialScale);
            anchor.transform.parent = _brickAnchor.transform;

            var list = anchor.GetComponentsInChildren<Transform>(true);

            foreach (var brick in list)
            {
                brick.gameObject.SetActive(true);
                var isBrick = brick.gameObject.GetComponent<BrickScript>();
                if (isBrick)
                    isBrick.SetBrickHealth(BrickHealth);
            }
        }

        public class BrickRing
        {
            public GameObject Anchor;
            public float Time;

            public BrickRing(int numberOfBricks, int brickHealth, BrickManagerScript brickManager)
            {
                Anchor = new GameObject {name = "Brick Ring Anchor"};

                var brickPrefab = brickManager.BrickPrefab;

                for (var i = 0; i < numberOfBricks; i++)
                {
                    var newBrick = Instantiate(brickPrefab, Vector3.zero, Quaternion.identity) as GameObject;

                    newBrick.transform.parent = Anchor.transform;
                    newBrick.transform.localEulerAngles = new Vector3(0, 0, 360.0f / numberOfBricks * i);
                    newBrick.transform.localPosition = (newBrick.transform.rotation * new Vector3(0, -1.7f, 0));

                    newBrick.GetComponent<BrickScript>().SetBrickHealth(brickHealth);
                }
            }
        }
    }
}
