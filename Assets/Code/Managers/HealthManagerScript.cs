using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Assets.Code
{
    public class HealthManagerScript : MonoBehaviour
    {
        /* References */
        private readonly GameObject[] _healthRing = new GameObject[2];
        private GameObject _outerLoseRing;

        /* Constants */

        // Use this for initialization
        void Awake ()
        {
            _healthRing[0] = GameObject.Find("Health Ring 1");
            _healthRing[1] = GameObject.Find("Health Ring 2");
            _outerLoseRing = GameObject.FindGameObjectWithTag("OuterRing");
        }

        void Start()
        {
            Reset();
        }

        public void Reset()
        {
            _healthRing[0].SetActive(true);
            _healthRing[0].transform.localScale = new Vector3(0.95f, 0.95f, 1);
            _healthRing[1].SetActive(true);
            _healthRing[1].transform.localScale = new Vector3(1, 1, 1);
            _outerLoseRing.SetActive(false);
        }

        public void HealthRingDestroyed (GameObject ring)
        {
            if (ring == _healthRing[0])
            {
                if (_healthRing[1].activeSelf)
                {
                    StartCoroutine(AdvanceHealthRing(_healthRing[1], 0.3f));
                }
            }
            else if (ring == _healthRing[1])
            {
                _outerLoseRing.SetActive(true);
            }

            StartCoroutine(FadeHealthRing(ring, 0.3f));
        }

        IEnumerator AdvanceHealthRing(GameObject ring, float time)
        {
            var deltaTime = 0f;
            while (deltaTime < time)
            {
                deltaTime += Time.smoothDeltaTime;
                ring.transform.localScale = Vector3.Lerp(new Vector3(1, 1, 1), new Vector3(0.95f, 0.95f, 1), deltaTime/time);
                yield return null;
            }

            ring.transform.localScale = new Vector3(0.95f, 0.95f, 1);
            yield return null;
        }

        IEnumerator FadeHealthRing(GameObject ring, float time)
        {
            var deltaTime = 0f;
            var Renderer = ring.GetComponent<SpriteRenderer>();
            while (deltaTime < time)
            {
                deltaTime += Time.smoothDeltaTime;
                Renderer.color = Color.Lerp(new Color(1, 1, 1, 1), new Color(1, 1, 1, 0), deltaTime / time);
                yield return null;
            }

            Renderer.color = new Color(1, 1, 1, 1);
            ring.SetActive(false);
            yield return null;
        }

    }
}
