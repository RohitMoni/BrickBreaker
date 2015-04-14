using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code
{
    public class PowerupManagerScript : MonoBehaviour
    {
        /* Editor filled */
        public GameObject PowerupPrefab;

        public Sprite BallSplitSprite;
        public Sprite ExtraPaddleSprite;
        public Sprite PowerBallSprite;
        public Sprite ShockwaveSprite;
        public Sprite SlowBallSprite;
        public Sprite WidePaddleSprite;

        /* Properties */
        private List<string> _powerupTypes;
        private List<GameObject> _powerups; 
        private List<float> _percentages;
        private int _pseudoRandomMultiplier;

        /* References */
        private GameObject _powerupAnchor;
        private GameManagerScript _gameManager;
        private PaddleManagerScript _paddleManager;
        private BrickManagerScript _brickManager;

        /* Constants */
        private const int PowerupInstancesPerType = 2;
        private const float PowerupFallSpeed = 2f;

        // Use this for initialization
        void Start ()
        {
            _powerupAnchor = GameObject.Find("PowerupAnchor");
            _gameManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<GameManagerScript>();
            _paddleManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<PaddleManagerScript>();
            _brickManager = GameObject.FindGameObjectWithTag("Managers").GetComponent<BrickManagerScript>();

            _powerupTypes = new List<string>();

            SetUpPowerupTypes();

            _powerups = new List<GameObject>(_powerupTypes.Count*PowerupInstancesPerType);
            _percentages = new List<float>(_powerupTypes.Count);

            SetUpPercentages();

            CreatePowerups();
        }

        void SetUpPowerupTypes()
        {
            _powerupTypes.Add("Extra paddle");
            _powerupTypes.Add("Shockwave");
            _powerupTypes.Add("Power ball");
            _powerupTypes.Add("Wide paddle");
            _powerupTypes.Add("Slow ball");
        }

        void CreatePowerups()
        {
            foreach (var type in _powerupTypes)
            {
                for (var i = 0; i < PowerupInstancesPerType; i++)
                {
                    var obj = Instantiate(PowerupPrefab);
                    obj.transform.parent = _powerupAnchor.transform;
                    obj.GetComponent<PowerupScript>().PowerupTag = type;

                    switch (type)
                    {
                        case "Ball split":
                            obj.GetComponent<SpriteRenderer>().sprite = BallSplitSprite;
                            break;
                        case "Extra paddle":
                            obj.GetComponent<SpriteRenderer>().sprite = ExtraPaddleSprite;
                            break;
                        case "Power ball":
                            obj.GetComponent<SpriteRenderer>().sprite = PowerBallSprite;
                            break;
                        case "Slow ball":
                            obj.GetComponent<SpriteRenderer>().sprite = SlowBallSprite;
                            break;
                        case "Laser gun":
                            obj.GetComponent<SpriteRenderer>().sprite = BallSplitSprite;
                            break;
                        case "Wide paddle":
                            obj.GetComponent<SpriteRenderer>().sprite = WidePaddleSprite;
                            break;
                        case "Shockwave":
                            obj.GetComponent<SpriteRenderer>().sprite = ShockwaveSprite;
                            break;
                        case "Shield":
                            obj.GetComponent<SpriteRenderer>().sprite = BallSplitSprite;
                            break;
                    }

                    ResetPowerup(obj);
   
                    _powerups.Add(obj);
                }
            }
        }

        public void ResetPowerup(GameObject powerup)
        {
            powerup.transform.localPosition = new Vector3(0, 0, 0);
            powerup.transform.localScale = new Vector3(0.8f, 0.8f, 0.8f);
            powerup.SetActive(false);
        }

        void SetUpPercentages()
        {
            // Fill in percentages
            foreach (var type in _powerupTypes)
            {
                switch (type)
                {
                    case "Ball split":
                        _percentages.Add(2f);
                        break;
                    case "Extra paddle":
                        _percentages.Add(1.5f);
                        break;
                    case "Power ball":
                        _percentages.Add(0.5f);
                        break;
                    case "Slow ball":
                        _percentages.Add(1f);
                        break;
                    case "Laser gun":
                        _percentages.Add(1f);
                        break;
                    case "Wide paddle":
                        _percentages.Add(1f);
                        break;
                    case "Shockwave":
                        _percentages.Add(0.5f);
                        break;
                    case "Shield":
                        _percentages.Add(1f);
                        break;
                    default:
                        {
                            Debug.Log("Powerup not found in list!");
                            _percentages.Add(0);
                            break;
                        }
                }
            }
        }

        public string CalculatePowerup()
        {
            var randomNumber = Random.Range(1f, 100f);

            var sumPercent = 0f;
            for (var i = 0; i < _powerupTypes.Count; i++)
            {
                var powerup = _powerupTypes[i];
                var percentage = _percentages[i];

                sumPercent += percentage;
                if (randomNumber <= sumPercent * _pseudoRandomMultiplier)
                {
                    _pseudoRandomMultiplier = 1;
                    return (powerup);
                }
            }

            _pseudoRandomMultiplier++;
            return null;
        }

        public void DropPowerup(string powerupType, Transform initialTransform)
        {
            foreach (var powerup in _powerups.Where(powerup => powerup.GetComponent<PowerupScript>().PowerupTag == powerupType && !powerup.activeSelf))
            {
                var drop = powerup;
                drop.transform.position = initialTransform.position;
                drop.SetActive(true);
                break;
            }
        }

        public void TriggerPowerup(GameObject powerup)
        {
            switch (powerup.GetComponent<PowerupScript>().PowerupTag)
            {
                case "Ball split":
                    break;
                case "Extra paddle":
                    _paddleManager.CreateNewPaddle();
                    break;
                case "Power ball":
                    _brickManager.SetPowerModeEnabled(true);
                    break;
                case "Slow ball":
                    _gameManager.DecreaseBallSpeed();
                    break;
                case "Laser gun":
                    break;
                case "Wide paddle":
                    _paddleManager.WidenPaddles();
                    break;
                case "Shockwave":
                    _brickManager.StartShockwave();
                    break;
                case "Shield":
                    break;
            }

            ResetPowerup(powerup);
        }

        void Update()
        {
            foreach (var powerup in _powerups.Where(powerup => powerup.activeSelf))
            {
                powerup.transform.position -= new Vector3(0, PowerupFallSpeed, 0) * Time.smoothDeltaTime;
            }
        }
    }
}
