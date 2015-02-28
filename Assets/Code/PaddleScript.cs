using UnityEngine;

namespace Assets.Code
{
    public class PaddleScript : MonoBehaviour
    {

        public int Speed;

        // Use this for initialization
        void Start () {
	
        }
	
        // Update is called once per frame
        void Update () {
            if (Input.GetKey(KeyCode.LeftArrow))
            {
                transform.Rotate(Vector3.forward, Speed);
            }
            if (Input.GetKey(KeyCode.RightArrow))
            {
                transform.Rotate(Vector3.back, Speed);
            }
        }

    
    }
}
