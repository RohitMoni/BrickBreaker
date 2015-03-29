using UnityEngine;

namespace Assets.Code.Powerups
{
    public class AddPaddlePU : MonoBehaviour {

        void OnTriggerCollide2D(Collider2D other)
        {
            if (other.tag == "Paddle")
            {
                GameObject.FindGameObjectWithTag("Managers").GetComponent<PaddleManagerScript>().CreateNewPaddle();
            }
        }
    }
}
