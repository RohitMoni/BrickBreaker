﻿using System;
using UnityEngine;

namespace Assets.Code
{
    public class BrickScript : MonoBehaviour
    {
        /* Properties */
        public int PointValue;
        public int HealthTotal;
        public int CurrentHealth;
        public string Powerup;

        /* Sprites */
        public Sprite BrickHealth0, BrickHealth1, BrickHealth2, BrickHealth3;

        /* References */
        private GameObject _brickDestroyEffect;

        // Managers
        private GameManagerScript _gameManager;
        private BrickManagerScript _brickManager;
        private PowerupManagerScript _powerupManager;

        /* Constants */
        private const int DefaultBrickHealth = 1;

        // Use this for initialization
        void Start ()
        {
            var managers = GameObject.FindGameObjectWithTag("Managers");
            _gameManager = managers.GetComponent<GameManagerScript>();
            _brickManager = managers.GetComponent<BrickManagerScript>();
            _powerupManager = managers.GetComponent<PowerupManagerScript>();
            _brickDestroyEffect = GameObject.FindGameObjectWithTag("ParticleBrickDestroy");
            PointValue = 10;
            HealthTotal = DefaultBrickHealth;

            Reset();
        }

        private void Reset()
        {
            CurrentHealth = HealthTotal;
            GetComponent<SpriteRenderer>().sprite = BrickHealth0;
            CalculatePowerup();
        }

        private void CalculatePowerup()
        {
            Powerup = _powerupManager.CalculatePowerup();
        }

        public void SetBrickHealth(int value)
        {
            value = Math.Min(4, value);
            value = Math.Max(0, value);

            HealthTotal = value;
            CurrentHealth = value;

            var comp = GetComponent<SpriteRenderer>();
            switch (value)
            {
                case 1:
                    comp.color = Color.yellow;
                    break;
                case 2:
                    comp.color = Color.green;
                    break;
                case 3:
                    comp.color = Color.cyan;
                    break;
                case 4:
                    comp.color = Color.magenta;
                    break;
            }
        }

        public void DestroyBrick(Quaternion particleRotation =new Quaternion())
        {
            if (particleRotation.z == 0)
                particleRotation = Quaternion.identity;

            // Move and rotate particle effect
            var position = transform.position;
            position.z = -4;
            _brickDestroyEffect.transform.position = position;

            _brickDestroyEffect.transform.rotation = particleRotation;

            // Play particle effect
            _brickDestroyEffect.GetComponent<ParticleSystem>().startColor = GetComponent<SpriteRenderer>().color;
            _brickDestroyEffect.GetComponent<ParticleSystem>().Emit((int)(transform.parent.localScale.x * 8));

            // Drop powerup
            _powerupManager.DropPowerup(Powerup, transform);

            // Add points
            _gameManager.AddScore(PointValue * HealthTotal);
            _gameManager.AddToComboValue();

            // Reset and 'Destroy' brick
            Reset();
            gameObject.SetActive(false);
        }

        public void OnCollisionEnter2D(Collision2D collision2D)
        {
            if (collision2D.gameObject.tag == "Ball")
            {
                CurrentHealth--;

                switch (CurrentHealth)
                {
                    case 3:
                        GetComponent<SpriteRenderer>().sprite = BrickHealth1;
                        break;
                    case 2:
                        GetComponent<SpriteRenderer>().sprite = BrickHealth2;
                        break;
                    case 1:
                        GetComponent<SpriteRenderer>().sprite = BrickHealth3;
                        break;
                    case 0:
                        var rotationVel = -collision2D.gameObject.GetComponent<BallScript>().Velocity.normalized;
                        var angle = Mathf.Atan2(rotationVel.y, rotationVel.x)*Mathf.Rad2Deg;
                        angle += 90;
                        var rotation = Quaternion.Euler(0, 0, angle);

                        DestroyBrick(rotation);
                        _brickManager.CheckRings();
                        
                        break;
                }
            }
        }
    }
}
