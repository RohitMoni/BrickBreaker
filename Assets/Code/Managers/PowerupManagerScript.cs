using System.Collections.Generic;
using UnityEngine;

namespace Assets.Code
{
    public class PowerupManagerScript : MonoBehaviour
    {
        public List<GameObject> Powerups;
        public List<float> Percentages; 

        // Use this for initialization
        void Start ()
        {
            Percentages = new List<float>(Powerups.Count);

            // Fill in percentages
            foreach (var powerup in Powerups)
            {
                switch (powerup.name)
                {
                    case "Ball split":
                        Percentages.Add(02f);
                        break;
                    case "Extra paddle":
                        Percentages.Add(01f);
                        break;
                    case "Power ball":
                        Percentages.Add(0.5f);
                        break;
                    case "Slow ball":
                        Percentages.Add(03f);
                        break;
                    case "Laser gun":
                        Percentages.Add(01f);
                        break;
                    case "Wide paddle":
                        Percentages.Add(01f);
                        break;
                    case "Shockwave":
                        Percentages.Add(0.5f);
                        break;
                    case "Shield":
                        Percentages.Add(01f);
                        break;
                    default:
                    {
                        Debug.Log("Powerup not found in list!");
                        Percentages.Add(0);
                        break;
                    }
                }
            }
        }

    }
}
