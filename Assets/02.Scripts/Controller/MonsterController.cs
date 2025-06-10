using System;
using FoodyGo.Database;
using FoodyGo.Mapping;
using FoodyGo.Services;
using FoodyGo.Services.NPCs;
using UnityEngine;

namespace FoodyGo.Controller
{
    public class MonsterController : MonoBehaviour
    {
        public MapLocation location;
        public MonsterService monsterService;
        public Monster monsterDataObject;
        public Animator animator;
        public float animatorSpeed;
        public float monsterWarmRate = .0001f;

        private float _hp;
        public float hp
        {
            get => _hp;
            set
            {
                _hp = value;
                animatorSpeed = _hp / maxHp;

                if (_hp <= 0)
                {
                    //TODO : 포획 성공 로직
                    Destroy(gameObject);
                }
            }
        }
        public float maxHp { get;} = 100f;
        private void Awake()
        {
            _hp = maxHp;
        }
        public void Damage(float amount)
        {
            hp -= amount;
        }

        // Use this for initialization
        void Start()
        {
            animator = GetComponent<Animator>();
            animator.speed = animatorSpeed;
        }
    
        // Update is called once per frame
        void Update()
        {
            if (animatorSpeed == 0)
            {
                //monster is frozen solid
                animatorSpeed = 0;
                return;
            }
            //if monster is moving they will warm up a bit
            animatorSpeed = Mathf.Clamp01(animatorSpeed + monsterWarmRate);
            animator.speed = animatorSpeed;
        }
    }
}
