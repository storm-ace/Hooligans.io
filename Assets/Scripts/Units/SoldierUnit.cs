using System.Collections.Generic;
using UI;
using UnityEngine;

namespace Units
{
    public class SoldierUnit : MonoBehaviour
    {
        [HideInInspector] public Player player;
        [HideInInspector] public AI ai;
        [HideInInspector] public UnitManager unitManager;

        [SerializeField] Transform detectionLayer;

        [SerializeField] private GameObject bulletPrefab;

        [SerializeField] private float health = 10;
        public float Health { get => health;
            set => health = value;
        }

        [SerializeField] private float range = 5;
        public float Range { get => range;
            set => range = value;
        }
    
        [SerializeField] private float dmg;
        public float Dmg { get => dmg;
            set => dmg = value;
        }
    
        [SerializeField] private float rpm;
        public float Rpm { get => rpm;
            set => rpm = value;
        }
    
        private float _rpmDefault;

        public bool IsAI { get; set; }

        public List<GameObject> unitsDetected = new List<GameObject>();
        public GameObject commander;

        private GameObject _target;

        private void Start()
        {
            _rpmDefault = rpm;
            rpm = 0;
            detectionLayer.GetComponent<CircleCollider2D>().radius = range;
            if (player) unitManager.player.Units++;
        }

        private void Update()
        {
            if (health <= 0)
            {
                if (IsAI)
                {
                    unitManager.enemyUnits.Remove(gameObject);
                    unitManager.player.Deaths++;
                }
                else
                {
                    unitManager.playerUnits.Remove(gameObject);
                    player.Coins += 10;
                    player.TotalCoins += 10;
                    player.Kills++;
                    player.UpdateCoins();
                }

                Destroy(gameObject);
            }

            ScanForHostiles();
            FireOnUnit();
        }

        public void ScanForHostiles()
        {
            _target = null;

            foreach (GameObject unit in unitsDetected)
            {
                if (unit.gameObject && Vector3.Distance(unit.transform.position, transform.position) < range)
                {
                    _target = unit.gameObject;
                    break;
                }
            }
        }

        private void FireOnUnit()
        {
            //Shots between
            if (rpm > 0)
            {
                rpm -= Time.deltaTime;
            }
            else //FIRE!
            {
                if (commander && unitsDetected.Count == 0) _target = commander;
                if (_target == null) return;
                var bullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
                bullet.GetComponent<Bullet>().Instantiate(_target, dmg, unitManager);
                rpm = _rpmDefault;
            }
        }
    }
}
