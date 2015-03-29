using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class PowerupManagerScript : MonoBehaviour
    {
        /* Properties */
        public List<GameObject> PowerupTypes;

        private List<GameObject> _powerups; 
        private List<float> _percentages;

        /* References */
        private GameObject _powerupAnchor;

        /* Constants */
        private const int PowerupInstancesPerType = 2;

        // Use this for initialization
        void Start ()
        {
            _powerupAnchor = GameObject.Find("PowerupAnchor");

            _powerups = new List<GameObject>(PowerupTypes.Count*PowerupInstancesPerType);
            _percentages = new List<float>(PowerupTypes.Count);

            CreatePowerups();

            // Fill in percentages
            foreach (var powerup in PowerupTypes)
            {
                switch (powerup.name)
                {
                    case "Ball split":
                        _percentages.Add(02f);
                        break;
                    case "Extra paddle":
                        _percentages.Add(01f);
                        break;
                    case "Power ball":
                        _percentages.Add(0.5f);
                        break;
                    case "Slow ball":
                        _percentages.Add(03f);
                        break;
                    case "Laser gun":
                        _percentages.Add(01f);
                        break;
                    case "Wide paddle":
                        _percentages.Add(01f);
                        break;
                    case "Shockwave":
                        _percentages.Add(0.5f);
                        break;
                    case "Shield":
                        _percentages.Add(01f);
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

        void CreatePowerups()
        {
            foreach (var powerup in PowerupTypes)
            {
                for (var i = 0; i < PowerupInstancesPerType; i++)
                {
                    var obj = Instantiate(powerup);
                    obj.transform.parent = _powerupAnchor.transform;
                    obj.SetActive(false);
                    _powerups.Add(obj);
                }
            }
        }

        void Update()
        {
            
        }
    }
}
